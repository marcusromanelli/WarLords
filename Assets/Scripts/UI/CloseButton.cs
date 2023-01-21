using UnityEngine;
using System.Collections;

public class CloseButton : MonoBehaviour {

	CardObject card;

	// Use this for initialization
	void Start () {
		card = GetComponentInParent<CardObject>();
	}
	
	void OnMouseDown(){
		//card.Close();
	}
}
