using UnityEngine;
using System.Collections;

public class StatusScript : MonoBehaviour {

	CardObject card;
	TextWrapper text;
	// Use this for initialization
	void Start () {
		card = GetComponentInParent<CardObject>();
		text = GetComponentInChildren<TextWrapper>();
	}

	// Update is called once per frame
	void Update () {
		if (card.Character != null) {
			text.text = "Life:        Attack:        Speed:\n   " + card.cardData.life + "               " + card.Character.calculateAttackPower () + "              " + card.Character.calculateWalkSpeed () + " Sqr";
		}
	}
}
