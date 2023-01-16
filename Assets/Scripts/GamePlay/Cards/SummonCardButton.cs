using UnityEngine;
using System.Collections;

public class SummonCardButton : MonoBehaviour {

	Player player;
	CardObject card;

	void Start () {
		card = transform.GetComponentInParent<CardObject>();
		player = card.player;
	}
	
	void OnMouseDown(){
		player.Summon(card);
	}
}
