﻿using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class AIPlayer : Player
{
	[BoxGroup("AI Behavior"), SerializeField] protected float awaitBetweenConditionSolving = 1f;
	[BoxGroup("AI Behavior"), SerializeField] protected bool isActive = true;


	private bool DoAction = false;
	public override void Setup(GameController gameController, InputController inputController)
    {
        base.Setup(gameController, inputController);

		gameController.OnPhaseChange += HandlePhaseChange;
	}
	protected override bool CanInteract()
	{
		return isActive && base.CanInteract();
	}

	void HandlePhaseChange(Phase currentPhase)
    {
		StartCoroutine(SolvePhase(currentPhase));
	}
    IEnumerator SolvePhase(Phase currentPhase)
	{
		var IsAITurn = CanInteract();

		if (!IsAITurn)
		{
			Debug.Log("Its not my turn. Ignoring");
			yield break;
		}

		switch (currentPhase)
        {
			case Phase.PreGame:
				yield return Hand.IsUIUpdating();

				var hasConditions = HasConditions();

				while (hasConditions)
				{
					if(!isActive)
						while (!DoAction)
						{
							yield return null;
						}

					ExecuteAction(() =>
					{
						var condition = GetConditions()[0];

						SolveCondition(condition);

						hasConditions = HasConditions();
					});

					yield return new WaitForSecondsRealtime(awaitBetweenConditionSolving);
				}


				break;
        }

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

	void SolveCondition(MandatoryCondition condition)	{
		switch (condition.Type) {
			case MandatoryConditionType.DrawCard:
				TryDrawCards ();
				break;
			case MandatoryConditionType.DiscartCard:
				throw new NotImplementedException("AI cannot discard cards yet");
			case MandatoryConditionType.SendCardToManaPool:
				GenerateManaFromRandomCard();
				break;
		}
	}
	void GenerateManaFromRandomCard()
	{
		var currentCard = GetRandomCardFromHand();

		Hand.HoldCard(currentCard);

		UseManaHability();

		Hand.CancelHandToCardInteraction();
	}

	Card GetRandomCardFromHand()
	{
		var handCards = Hand.GetCards();

		return handCards[UnityEngine.Random.Range(0, handCards.Length - 1)];
	}
	[Button]
	void Action()
    {
		DoAction = true;
    }
	void ExecuteAction(Action Action)
	{
		Action();

		DoAction = false;
	}
}
