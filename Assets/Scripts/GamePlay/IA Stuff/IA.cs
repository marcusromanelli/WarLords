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
		var AIWillPlay = !player.hasConditions() && !player.IsDrawing() && GameController.Singleton.MatchHasStarted && GameController.Singleton.currentPlayer == player;

		if (!AIWillPlay)
			return;


		switch (GameController.Singleton.currentPhase) {
		case Phase.Draw:
			//if(!player.hasDrawnCard){
				//player.DrawCard ();
			//}
			break;
		case Phase.Action:
			if (!player.HasUsedHability()) {
				if (player.GetCurrentHandNumber() <= 2) {
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

				CardObject cad = player.GetHandObject().cards.Find (a => a.cardData.PlayID == cd.PlayID);
				Vector3 pos = test [Random.Range (0, test.Count)].transform.position;

				player.Summon (cad, pos);
			}

			player.EndPhase ();
			break;
		}
	}

	void solveCondition()	{
		var AIWillPlay = !player.hasConditions() || player.IsDrawing();

		if (!AIWillPlay)
			return;

		var conditions = player.GetConditionList();
		Condition condition = null;

		if (conditions.Count <= 0)
			return;

		condition = conditions.First ();

		switch (condition.Type) {
			case ConditionType.DrawCard:
				player.DrawCard ();
				break;
			case ConditionType.DiscartCard:
				player.DiscartCard (getRandomCardFromHand());
				break;
			case ConditionType.SendCardToManaPool:
				player.SendCardToManaPool (getRandomCardFromHand());
				break;
		}
	}

	Card getRandomCardFromHand()
	{
		var hand = player.GetHand();
		return hand[Random.Range(0, hand.Count - 1)];
	}
}
