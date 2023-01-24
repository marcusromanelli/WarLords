using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class IA : Player {
	void Start () {
	}


	void solvePhase(){
		//var AIWillPlay = !player.HasConditions() && !player.IsDrawing() && GameController.MatchHasStarted && gameController.GetCurrentPlayer() == player;

		//if (!AIWillPlay)
		//	return;

/*
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
				int cost = cd.CalculateSummonCost ();

				if (player.CanSpendMana (cost)) {
					List<SpawnArea> emptyTiles = battlefield.GetEmptyFields(gameController.GetLocalPlayer());
					//test.RemoveAll (spawnArea => spawnArea.player.GetPlayerType() == PlayerType.Remote || spawnArea.Hero == null);

					/*CardObject cardObject = player.GetHandObject().cards.Find (a => a.GetCardData().PlayID == cd.PlayID);

					var tile = emptyTiles[Random.Range(0, emptyTiles.Count)];
					
					battlefield.Summon (cardObject, tile);*
				}

				player.EndPhase ();
				break;
		}*/
	}

	void solveCondition()	{
		/*var AIWillPlay = !player.HasConditions();// || player.IsDrawing();

		if (!AIWillPlay)
			return;

		var conditions = player.GetConditionList();
		Condition condition = null;

		if (conditions.Count <= 0)
			return;

		condition = conditions.First ();

		switch (condition.Type) {
			case ConditionType.DrawCard:
				player.TryDrawCards ();
				break;
			case ConditionType.DiscartCard:
				player.DiscartCardFromHand (getRandomCardFromHand());
				break;
			case ConditionType.SendCardToManaPool:
				player.SendCardToManaPool (getRandomCardFromHand());
				break;
		}*/
	}

	Card getRandomCardFromHand()
	{
		/*var hand = player.GetHand();
		return hand[Random.Range(0, hand.Count - 1)];*/
		return default(Card);
	}
}
