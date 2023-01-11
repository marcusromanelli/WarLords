using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhasesTitle : MonoBehaviour {

	private static PhasesTitle __singleton;
	public static PhasesTitle Singleton{
		get{
			if(__singleton==null){
				if(GameObject.FindObjectOfType<PhasesTitle>()){
					__singleton = GameObject.FindObjectOfType<PhasesTitle> ();
				}else{
					GameObject aux = new GameObject();
					aux.name = "-(Phases Tile Controller)-";
					__singleton = aux.AddComponent<PhasesTitle>();
				}
			}
			return __singleton;
		}
	}
	public Sprite Combat, Enemy, Movement, Your, Win, Lose;

	Image image;
	Color aux;
	float alpha;
	public static bool isFading;
	bool finish;

	void Start () {
		aux = Color.white;
		aux.a = 0;

		image = GetComponent<Image>();
		image.color = aux;
	}

	void Update(){
		if(isFading){
			aux.a = alpha;
			image.color = aux;
		}
	}


	public static void setWinner(Player player){
		if(!Singleton.finish){
			if(player.civilization == GameController.GetLocalPlayer().civilization){
				Singleton.image.sprite = Singleton.Win;
			}else{
				Singleton.image.sprite = Singleton.Lose;
			}
			Singleton.finish = true;
			Singleton.StartCoroutine("fade");
		}
	}

	public static void ChangePhase(Phase next){
		if(!Singleton.finish){
			Singleton.updateImage(next);
			Singleton.StartCoroutine("fade");
		}
	}

	IEnumerator fade(){
		isFading = true;
		if(image.sprite!=null){
			alpha = 0;
			while(alpha<1){
				alpha +=0.05f;
				yield return null;
			}
			yield return new WaitForSeconds(1f);

			if(!finish){
				while(alpha>0){
					alpha -=0.05f;
					yield return null;
				}
			}
			isFading = false;
		}else{
			isFading = false;
			yield return null;
		}
	}

	void updateImage(Phase next){
		switch(next){
		case Phase.Action:
			if(GameController.Singleton.Players[GameController.Singleton.currentPlayer].isRemotePlayer){
				image.sprite = Singleton.Enemy;
			}else{
				image.sprite = Singleton.Your;
			}
			break;
		case Phase.Movement:
			image.sprite = Movement;
			break;
		case Phase.Attack:
			image.sprite = Combat;
			break;
		default:
			image.sprite = null;;
			break;
		}
	}
}
