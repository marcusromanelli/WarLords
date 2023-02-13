using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManual : MonoBehaviour {

	public GameObject image;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick(){
		bool active = image.activeSelf;

		if(active){
			image.SetActive(false);
		}else{
			image.SetActive(true);
		}
	}

	public void Close(){
		image.SetActive(false);
	}
}
