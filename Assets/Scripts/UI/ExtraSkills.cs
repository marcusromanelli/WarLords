﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ExtraSkills : MonoBehaviour {

	CardObject card;
	List<TextWrapper> text;
	// Use this for initialization
	void Start () {
		card = GetComponentInParent<CardObject>();
		text = GetComponentsInChildren<TextWrapper>().ToList();

		text.ForEach(delegate(TextWrapper obj) {
			obj.transform.parent.gameObject.SetActive(false);
		});
	}
	
	// Update is called once per frame
	void Update () {
		if(card.card.Skills.Count>2){
			int found=0;
			for(int i=2;i<card.card.Skills.Count;i++){
				if(text[i-2].transform.parent.gameObject.activeSelf==false){
					text[i-2].transform.parent.gameObject.SetActive(true);
					text[i-2].text = card.card.Skills[i].name+" - "+card.card.Skills[i].description;
					found++;
				}
			}
			for(int i=0;i<found;i++){
				text[i+2].transform.parent.gameObject.SetActive(false);
			}
		}
	}
}
