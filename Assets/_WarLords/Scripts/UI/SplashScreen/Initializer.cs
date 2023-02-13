using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Initializer : MonoBehaviour {
	public Sprite[] icons;
	bool fadingIn, fadingOut;
	float fade, count;
	int indexer;
	Image image;
	bool choose, fading, acabou;
	float speed;

	// Use this for initialization
	void Start () {
		fadingIn=true;
		image = transform.Find("Icones").GetComponent<Image>();
		speed=0.01f;
		choose=false;
		Invoke("start", 2);
		Application.targetFrameRate = 60;
	}
	void start(){
		choose=true;
	}

	void finish(){
		SceneController.Singleton.LoadLevel(MenuScreens.Menu, 1f, 3f);
	}
	IEnumerator fadeIn(){
		fading=true;
		while(fade<1){
			fade+=speed;
			yield return null;
		}
		while(count<1){
			count+=speed;
			yield return new WaitForSeconds(Time.deltaTime/2);
		}
		fadingIn=false;
		fadingOut=true;
		
		fading=false;
	}
	IEnumerator fadeOut(){
		fading=true;
		while(fade>0){
			fade-=speed;
			yield return null;
		}
		while(count<1){
			count+=speed;
			yield return new WaitForSeconds(Time.deltaTime/2);
		}
		count=0;
		fadingIn=true;
		fadingOut=false;
		indexer++;
		fading=false;
	}
	// Update is called once per frame
	void Update () {
		if(choose){
			Sprite aux=null;
			try{
				aux = icons[indexer];
			}catch(Exception){
				if(!acabou){
					acabou=true;
					finish();
				}
			}
			if(image==null){
				image = GameObject.Find("Icones").GetComponent<Image>();
			}
			if(aux!=null){
				image.sprite = aux;
				if(fadingIn && !fading){
					StartCoroutine(fadeIn());
				}else if(fadingOut && !fading){
					StartCoroutine(fadeOut());
				}
			}

			image.color = new Color(image.color.r, image.color.b, image.color.g, fade);


			if(Input.GetMouseButtonDown(0)){
				speed*=1.2f;
			}
		}
	}
}
