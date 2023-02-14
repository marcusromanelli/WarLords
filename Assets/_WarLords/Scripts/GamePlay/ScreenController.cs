using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenController: MonoBehaviour {
	private static ScreenController __singleton;
	public static ScreenController singleton{
		get{
			if(__singleton==null){
				if(GameObject.FindObjectOfType<ScreenController>()){
					__singleton = GameObject.FindObjectOfType<ScreenController>();
					__singleton.name = "-(Screen Controller)-";
				}else{
					GameObject aux = new GameObject();
					aux.name = "-(Screen Controller)-";
					__singleton = aux.AddComponent<ScreenController>();
					DontDestroyOnLoad(aux);
				}
			}
			return __singleton;
		}
	}
	[HideInInspector]
	public float currentFade;
	private bool 
		_fading;
	public static bool fading{
		get{
			return singleton._fading;
		}
	}


	Color loadingColor;
	private GameObject _loadingBG;
	protected GameObject loadingBG{
		get{
			if(_loadingBG==null){
				_loadingBG = (GameObject)Instantiate(Resources.Load<GameObject>("Misc/LoadingBG"), Vector3.zero, Quaternion.identity);
				_loadingBG.transform.SetParent(createCanvas(), false);
				_loadingBG.transform.SetAsLastSibling();
			}
			return _loadingBG;
		}
		private set{}
	}

	Transform canvas;
	public static bool showLoadingIcon;
	
	
	private void Awake(){
		DontDestroyOnLoad(this);
		__singleton = this;
	}

	void Start () {
	}

	void Update () {

		if(canvas!=null){
			loadingColor = loadingBG.GetComponent<Image>().color;
			loadingColor.a = currentFade;
			loadingBG.GetComponent<Image>().color = loadingColor;
			loadingBG.SetActive(true);
		}else{
			createCanvas();
		}
	}
	private Transform createCanvas(){
		if(!GameObject.Find("Canvas - Level Controller")){
			canvas = (new GameObject("Canvas - Level Controller", typeof(Canvas), typeof(CanvasScaler))).transform;
			canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.GetComponent<Canvas>().sortingLayerName = "Default";
			canvas.GetComponent<Canvas>().sortingOrder = 10;	
			if(GameObject.Find("HUDCamera")){
				canvas.GetComponent<Canvas>().worldCamera = GameObject.Find("HUDCamera").GetComponent<Camera>();
			}
		}else{
			canvas = GameObject.FindObjectOfType<Canvas>().transform;
		}
		DontDestroyOnLoad(canvas);
		return canvas;
	}
	public static void Clear(){
		singleton.currentFade = 0;
	}

	public static void SetFadeColor(Color color){
		color.a = 0;
		singleton.loadingBG.GetComponent<Image>().color = color;
	}
	public static void Blink(Color color){
		singleton.blinkScreen(color);
	}
	protected void blinkScreen(Color color){
		StartCoroutine(_blink(color));
	}
	IEnumerator _blink(Color color){
		SetFadeColor(color);
		FadeOut(0.1f);
		while(currentFade<1){
			yield return null;
		}
		FadeIn(0.1f);
	}
	public static void FadeOut(float tempoFadeOut=2f){
		singleton.doFadeOut(tempoFadeOut);
	}
	private void doFadeOut(float tempoFadeOut){
		SetFadeColor(loadingBG.GetComponent<Image>().color);
		StartCoroutine(_fadeOut(tempoFadeOut));
	}
	IEnumerator _fadeOut(float tempoFadeOut){
		loadingBG.transform.SetAsLastSibling();
		_fading=true;
		currentFade=0;
		while(currentFade<1.0f){
			yield return new WaitForEndOfFrame();
			currentFade = Mathf.Clamp01(currentFade + Time.deltaTime / tempoFadeOut);
		}
		_fading=false;
		currentFade=1;
	}
	
	
	//Fade In
	
	public static void FadeIn(float tempoFadeIn=2f){
		singleton.doFadeIn(tempoFadeIn);
	}
	private void doFadeIn(float tempoFadeIn){
		StartCoroutine(_fadeIn(tempoFadeIn));
	}
	IEnumerator _fadeIn(float tempoFadeIn){
		loadingBG.transform.SetAsLastSibling();
		while(currentFade<1){
			yield return null;
		}
		_fading=true;
		currentFade=1;
		while(currentFade>0){
			yield return new WaitForEndOfFrame();
			currentFade = Mathf.Clamp01(currentFade - Time.deltaTime / tempoFadeIn);
		}
		currentFade=0;
		_fading=false;
	}
}
