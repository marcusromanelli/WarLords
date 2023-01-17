﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class IA : MonoBehaviour {

	[SerializeField] GameController gameController;
	[SerializeField] Battlefield battlefield;
	Player player;

	void Start () {
		player = GetComponent<Player> ();
		InvokeRepeating("solveCondition", 0, 0.5f);
		InvokeRepeating("solvePhase", 0, 4f);
	}


	void solvePhase(){
		var AIWillPlay = !player.HasConditions() && !player.IsDrawing() && GameController.Singleton.MatchHasStarted && gameController.GetCurrentPlayer() == player;

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

				if (player.CanSpendMana (cost)) {
					List<SpawnArea> test = battlefield.GetEmptyFields(gameController.GetLocalPlayer());
					//test.RemoveAll (spawnArea => spawnArea.player.GetPlayerType() == PlayerType.Remote || spawnArea.Hero == null);

					CardObject cad = player.GetHandObject().cards.Find (a => a.cardData.PlayID == cd.PlayID);

					var tile = test[Random.Range(0, test.Count)];

					player.Summon (cad, tile);
				}

				player.EndPhase ();
				break;
		}
	}

	void solveCondition()	{
		var AIWillPlay = !player.HasConditions() || player.IsDrawing();

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
