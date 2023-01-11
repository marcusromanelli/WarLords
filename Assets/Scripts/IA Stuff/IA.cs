using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class IA : MonoBehaviour {
	Player player;

	void Start () {
		player = GetComponent<Player> ();
		InvokeRepeating("solveCondition", 0, 0.5f);
		InvokeRepeating("solvePhase", 0, 4f);
	}


	void solvePhase(){
		if (!player.hasConditions () && !player.isDrawing && GameController.Singleton.MatchHasStarted) {
			if (GameController.Singleton.MatchHasStarted && GameController.Singleton.currentPlayer == ((int)player.civilization)) {
				switch (GameController.Singleton.currentPhase) {
				case Phase.Draw:
					//if(!player.hasDrawnCard){
						//player.DrawCard ();
					//}
					break;
				case Phase.Action:
					if (!player.hasUsedHability) {
						if (player.Hand.Count <= 2) {
							player.DiscartCardToDrawTwo (getRandomCardFromHand ());
						} else {
							player.SendCardToManaPool (getRandomCardFromHand ());
						}
					}

					Card cd = getRandomCardFromHand ();
					int cost = cd.calculateCost ();
					if (player.canSpendMana (cost)) {
						List<SpawnArea> test = GameObject.FindObjectsOfType<SpawnArea> ().ToList ();
						test.RemoveAll (a => a.LocalPlayer == true || a.doesHaveHero);
						CardObject cad = player.HandObject.cards.Find (a => a.card.PlayID == cd.PlayID);
						Vector3 pos = test [Random.Range (0, test.Count)].transform.position;
						player.Summon (cad, pos);
					}

					player.EndPhase ();
					break;
				}
			}
		}
	}

	void solveCondition(){
		if (player.hasConditions() && !player.isDrawing) {
			Condition condition = player.Conditions [0];
			switch (condition.Type) {
			case ConditionType.DrawCard:
				player.DrawCard ();
				break;
			case ConditionType.DiscartCard:
				player.DiscartCard (player.Hand [Random.Range (0, player.Hand.Count - 1)]);
				break;
			case ConditionType.SendCardToManaPool:
				player.SendCardToManaPool (getRandomCardFromHand());
				break;
			}
		}
	}

	Card getRandomCardFromHand(){
		return player.Hand [Random.Range (0, player.Hand.Count - 1)];
	}
}
