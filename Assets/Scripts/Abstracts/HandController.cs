using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class HandController : MonoBehaviour{
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
					if(player.isRemotePlayer){
						aux.z = -180;
						aux.x = 90;
					}else{
						aux.x = 270;
						aux.z = 0;
					}

					card.transform.localRotation = Quaternion.RotateTowards (card.transform.localRotation, Quaternion.Euler (aux), Time.deltaTime * cardFoldSpeed); 
					//if(Vector3.Distance(card.transform.localPosition, calculatePosition(c))>0.1f){
					//-	Debug.Log(card.transform.localPosition+ " - "+ calculatePosition(c));
						card.transform.localPosition = Vector3.MoveTowards (card.transform.localPosition, calculatePosition(c), Time.deltaTime * cardFoldSpeed/50); 
					//}
				}
				c++;
			}
		}

		updateCardList ();
	}

	Vector3 aux2;
	public void AddCard(Card card){
		GameObject aux = (GameObject)Instantiate (cardTemplate.gameObject, player.DeckController.getTopPosition(), player.DeckController.getTopRotation());
		aux.transform.SetParent(transform, true);
		//aux.transform.localPosition = Vector3.zero + Vector3.up * (0.05f * cards.Count);
		//aux.transform.localRotation = Quaternion.Euler (Vector3.right*270);
		if(player.isRemotePlayer){
			//Destroy(aux.GetComponent<Collider>());
			//aux2 = aux.transform.localScale;
			//aux2.z*=-1;
			//aux.transform.localScale = aux2;
		}

		aux.GetComponent<CardObject> ().setCard (card.CardID, card.PlayID, player);
		aux.GetComponent<CardObject> ().originalPosition = aux.transform.localPosition;
		updateCardList ();
	}

	public void RemoveCard(int PlayID){
		CardObject cd = cards.Find (a => a.card.PlayID == PlayID);
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