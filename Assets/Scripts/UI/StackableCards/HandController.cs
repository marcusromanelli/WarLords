using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class HandController : MonoBehaviour{

	[SerializeField] GameController gameController;
	[SerializeField] Battlefield battlefield;
	[SerializeField] DeckController deckController;


	public float cardFoldSpeed = 600f;
	protected CardObject cardTemplate;
	public List<CardObject> cards;
	protected Renderer[] renderers;
	Player player;

	void Awake(){
		cards = new List<CardObject> ();
		cardTemplate = Resources.Load<CardObject> ("Prefabs/Card");
	}

	Vector3 aux;
	void Update () {
		int c = 1;
		foreach (CardObject card in cards) {
			if (card != null) {
				aux = card.transform.rotation.eulerAngles;
				aux.y = getAngleByCardNumber (c);
				if (!card.isBeingHeld && !card.isBeingVisualized) {
					if(player.GetPlayerType() == PlayerType.Remote){
						aux.z = -180;
						aux.x = 90;
					}else{
						aux.x = 270;
						aux.z = 0;
					}

					card.transform.localRotation = Quaternion.RotateTowards (card.transform.localRotation, Quaternion.Euler (aux), Time.deltaTime * cardFoldSpeed); 

					card.transform.localPosition = Vector3.MoveTowards (card.transform.localPosition, calculatePosition(c), Time.deltaTime * cardFoldSpeed/50); 
				}
				c++;
			}
		}

		updateCardList ();
	}

	public void AddCard(Card card){
		GameObject aux = (GameObject)Instantiate (cardTemplate.gameObject, deckController.GetTopPosition(), deckController.GetTopRotation());
		aux.transform.SetParent(transform, true);

		var cardObj = aux.GetComponent<CardObject>();

		cardObj.Setup (card, player, battlefield, this, gameController);
		cardObj.originalPosition = aux.transform.localPosition;
		updateCardList ();
	}

	public void RemoveCard(int PlayID){
		CardObject cd = cards.Find (a => a.cardData.PlayID == PlayID);
		if (cd!=null) {
			cd.becameMana ();
			cards.Remove (cd);
			updateCardList ();
		} else {
			Debug.Log ("Trying to remove a card that isn't in hand");
		}
	}
	public void setPlayer(Player player){
		this.player = player;
	}

	public Vector3 calculatePosition(int number){
		int count = cards.Count;
		Vector3 pos = Vector3.zero;//transform.position;
		float cardSize = 0.3f;
		pos.y = number*0.015f;
		pos.x = -((count*cardSize)/2) + cardSize * number;

		return pos;
	}

	void updateRendererList(){
		renderers = transform.GetComponentsInChildren<Renderer> ();
	}
	void updateCardList(){
		if (cards.Count != transform.GetComponentsInChildren<CardObject> ().Length) {
			cards = transform.GetComponentsInChildren<CardObject> ().ToList ();
		}
		updateRendererList ();
	}
	float getAngleByCardNumber(int number){
		int retur = 0;
		switch (cards.Count) {
			default:
			case 1: retur = 0; break;
			case 2: 
				switch(number){
				case 1:
					retur = 5; break;
				case 2:
					retur = -5;break;
				}
				break;
			case 3: 
				switch(number){
				case 1:
					retur = 10; break;
				case 2:
					retur = 0; break;
				case 3:
					retur = -10;break;
				}
				break;
			case 4: 
				switch(number){
				case 1:
					retur = 10; break;
				case 2:
					retur = 5; break;
				case 3:
					retur = -5;break;
				case 4:
					retur = -10;break;
				}
				break;
			case 5: 
				switch(number){
				case 1:
					retur = 15; break;
				case 2:
					retur = 10; break;
				case 3:
					retur = 0;break;
				case 4:
					retur = -10;break;
				case 5:
					retur = -15;break;
				}
				break;
			case 6: 
				switch(number){
				case 1:
					retur = 20; break;
				case 2:
					retur = 10; break;
				case 3:
					retur = 5;break;
				case 4:
					retur = -5;break;
				case 5:
					retur = -10;break;
				case 6:
					retur = -20;break;
				}
				break;
			case 7: 
				switch(number){
				case 1:
					retur = 20; break;
				case 2:
					retur = 15; break;
				case 3:
					retur = 5;break;
				case 4:
					retur = 0;break;
				case 5:
					retur = -5;break;
				case 6:
					retur = -15;break;
				case 7:
					retur = -20;break;
				}
				break;
			case 8: 
				switch(number){
				case 1:
					retur = 25; break;
				case 2:
					retur = 15; break;
				case 3:
					retur = 5;break;
				case 4:
					retur = 0;break;
				case 5:
					retur = 0;break;
				case 6:
					retur = -5;break;
				case 7:
					retur = -15;break;
				case 8:
					retur = -25;break;
				}
				break;
		}
		return retur;
	}
}