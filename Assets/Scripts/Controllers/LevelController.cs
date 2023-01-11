using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelController: MonoBehaviour {
	private static LevelController __singleton;
	public static LevelController singleton{
		get{
			if(__singleton==null){
				if(GameObject.Find("-(Level Loader Controller)-")){
					__singleton = GameObject.Find("-(Level Loader Controller)-").GetComponent<LevelController>();
				}else{
					GameObject aux = new GameObject();
					aux.name = "-(Level Loader Controller)-";
					__singleton = aux.AddComponent<LevelController>();
					DontDestroyOnLoad(aux);
				}
			}
			return __singleton;
		}
	}
	private AsyncOperation _async;
	public AsyncOperation Async{
		get{
			if(__singleton._async==null){
				__singleton._async = new AsyncOperation();
			}
			return __singleton._async;
		}
		protected set{
			_async = value;
		}
	}

	public static bool doneLoading{
		get{
			return singleton.Async.isDone;
		}
		private set{}
	}
	
	private void Awake(){
		DontDestroyOnLoad(this);
		__singleton = this;
	}


	public static void AsyncPreLoadLevel(int newFase){
		singleton._AsyncPreLoadLevel(newFase);
	}

	void _AsyncPreLoadLevel(int newFase){
		Debug.LogWarning("Started Loading.");
		Async = SceneManager.LoadSceneAsync (newFase);
		Async.allowSceneActivation = false;
	}

	public static void AsyncLoadLevel(){
		singleton._AsyncLoadLevel();
	}
	void _AsyncLoadLevel(){
		Async.allowSceneActivation = true;
	}
}
