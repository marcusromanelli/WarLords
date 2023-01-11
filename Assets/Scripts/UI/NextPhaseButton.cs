using UnityEngine;
using System.Collections;

public class NextPhaseButton : MonoBehaviour {

	Vector3 originalPosition = new Vector3(0, 4, -3);
	Vector3 hiddenPosition = new Vector3(2, 4, -3);

	Player player;
	public bool isHidden;

	void Start () {
		player = GetComponentInParent<Player>();	
		isHidden = true;
		transform.localPosition = hiddenPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if(GameController.Singleton.MatchHasStarted && GameController.Singleton.getCurrentPlayer() == player && GameController.Singleton.currentPhase == Phase.Action){
			isHidden = false;
		}else{
			isHidden = true;
		}

		if(isHidden){
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, hiddenPosition, Time.deltaTime * 10);
		}else{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPosition, Time.deltaTime * 10);
		}
	}

	void OnMouseDown(){
		player.EndPhase();
	}
}