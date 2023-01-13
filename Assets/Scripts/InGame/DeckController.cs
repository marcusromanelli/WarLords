using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DeckController : MonoBehaviour {

	public float distanceBetweenCards = 0.02f;
	public bool isMouseOver;

	TextMesh Counter;
	GameObject coverTemplate;
	Player player;
	Stack<GameObject> DeckCards;
	GameObject aux;
	float value = 0;
	RaycastHit[] results;
	int layerMask;

	Civilization currentCivilization;

	void Start(){

		DeckCards = new Stack<GameObject> ();
		player = GetComponentInParent<Player> ();
		currentCivilization = player.GetCivilization();

		coverTemplate = Resources.Load<GameObject> ("Prefabs/CardBackCover"+((int)currentCivilization));

		Counter = transform.GetComponentInChildren<TextMesh> ();
	}
	void Update () {
		if (player.GetCurrentPlayDeckCount() < getNumberOfCards()) {
			Destroy (DeckCards.Pop ().gameObject);
		} else if (player.GetCurrentPlayDeckCount() > getNumberOfCards()) {
			(aux = (GameObject)Instantiate (coverTemplate, Vector3.zero, Quaternion.Euler (90, 0, 0))).transform.position = transform.position + Vector3.up * (distanceBetweenCards * DeckCards.Count);
			DeckCards.Push (aux);
			aux.transform.SetParent (transform, true);
		}
		value = getNumberOfCards();

		Counter.text = value + "\n"+((value>1)?"Cards":"Card");

		if (isMouseOver && Input.GetMouseButtonDown (0)) {
			if (GameController.Singleton.currentPhase == Phase.Draw && GameController.Singleton.currentPlayerNumber == ((int)currentCivilization)) {
				if (!player.HasDrawnCard()) {				
					player.SetDrawnCard(true);
					player.DrawCard ();

					GameController.Singleton.goToPhase (Phase.Action, player.GetCivilization());
				} else {
					if(!player.hasCondition(ConditionType.DrawCard) && !Application.isEditor){
						Debug.LogWarning ("You already drawn your card this turn");
					}else{
						player.DrawCard ();
					}
				}
			} else {
				if(!player.hasCondition(ConditionType.DrawCard) && !Application.isEditor){
					GameConfiguration.PlaySFX(GameConfiguration.denyAction);
					Debug.LogWarning ("You only draw a card in your Drawn Phase");
				}else{
					player.DrawCard ();
				}
			}
		}
			
		layerMask = 1 << gameObject.layer;
		results = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition), 1000, layerMask);
		if (results.ToList().FindAll(a => a.collider.gameObject == this.gameObject).Count>0) {
			isMouseOver = true;
		}else{
			isMouseOver = false;
		}
	}

	public int getNumberOfCards(){
		if (DeckCards == null) {
			DeckCards = new Stack<GameObject> ();
		}
		return DeckCards.Count;
	}

	public Vector3 getTopPosition(){
		return transform.position + Vector3.up * (distanceBetweenCards * ((getNumberOfCards()>0)?getNumberOfCards():1));
	}

	public Quaternion getTopRotation(){
		return Quaternion.Euler (90, 0, 0);
	}

	public Vector3 getTopScale(){
		return Vector3.one;
	}
}
