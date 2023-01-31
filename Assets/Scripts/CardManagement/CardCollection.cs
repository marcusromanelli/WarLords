using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CardCollection : ISaveable {

	/*public List<Card> Cards;

	public static CardCollection Load(bool loadFromBuild = false){
		CardCollection aux=CardCollection.Load<CardCollection>(loadFromBuild);

		int c = 1;
		Card aux2;

		for(int i=0;i<aux.Cards.Count; i++){

			aux2 = aux.Cards [i];
			//aux2.CardID = c;
			//aux2.updateData ();

			aux.Cards [i] = aux2;
			c++;
		}


		return aux; 
	}
	public void Save(bool saveOutsideBuild = true){
		base.Write<CardCollection>(saveOutsideBuild);
	}

	public static Card FindCardByID(int id){
		
		return CardsLibrary.Singleton.Cards.Cards.Find (a => a.CardID == id);
	}*/
}
