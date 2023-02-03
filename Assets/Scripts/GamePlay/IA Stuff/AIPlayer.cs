﻿using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
	[BoxGroup("AI Behavior"), SerializeField] protected UIBattlefield uiBattlefield;
	[BoxGroup("AI Behavior"), SerializeField] protected PhaseManager phaseManager;
	[BoxGroup("AI Behavior"), SerializeField] protected float awaitBetweenConditionSolving = 1f;
	[BoxGroup("AI Behavior"), SerializeField] protected bool isActive = true;


	private bool DoAction = false;
	public override void PreSetup(Battlefield battlefield, GameController gameController, InputController inputController)
    {
        base.PreSetup(battlefield, gameController, inputController);

		phaseManager.OnPhaseChange += HandlePhaseChange;
	}
	protected override bool CanInteract()
	{
		return isActive && base.CanInteract();
	}

	void HandlePhaseChange(PhaseType currentPhase)
    {
		StopAllCoroutines();
		StartCoroutine(SolvePhase(currentPhase));
	}
    IEnumerator SolvePhase(PhaseType currentPhase)
	{
		var IsAITurn = CanInteract();

		if (!IsAITurn)
			yield break;

		switch (currentPhase)
        {
			case PhaseType.PreGame:
				yield return ResolvePreGame();
				break;
			case PhaseType.Action:
				yield return ResolveActionPhase();
				break;
			case PhaseType.End:
				yield return ResolveConditions();
				break;
		}
	}

	IEnumerator ResolveActionPhase()
    {
		if (!HasUsedHability)
			if (GetHandCardsNumber() <= 2)
				DiscardRandomCard();
			else
				GenerateManaFromRandomCard();

		yield return new WaitForSeconds(awaitBetweenConditionSolving);

		TrySummoning();

		OnClickNextPhase();
	}
	void TrySummoning() {
		var tile = GetRandomTile();
		Card card = GetRandomCardFromHand();

		if (tile != null && CanSummonHero(card, tile))
			TrySummonHero(card, tile);
	}
	SpawnArea GetRandomTile()
    {
		List<SpawnArea> emptyTiles = uiBattlefield.GetEmptyFields(this);
		emptyTiles.RemoveAll(spawnArea => spawnArea.Hero != null);

		return emptyTiles[UnityEngine.Random.Range(0, emptyTiles.Count)];
	}
	IEnumerator ResolveConditions()
    {
		var hasConditions = HasConditions();

		while (true)
		{
			hasConditions = HasConditions();

			if (!hasConditions)
			{
				yield return null;
				continue;
			}

			if (!isActive)
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
	}
	IEnumerator ResolvePreGame()
    {
		yield return Hand.IsUIUpdating();

		yield return ResolveConditions();
	}
	void SolveCondition(MandatoryCondition condition)	{
		switch (condition.Type) {
			case MandatoryConditionType.DrawCard:
				TryDrawCards ();
				break;
			case MandatoryConditionType.DiscartCard:
				DiscardRandomCard();
				break;
			case MandatoryConditionType.SendCardToManaPool:
				GenerateManaFromRandomCard();
				break;
		}
	}
	void DiscardRandomCard()
	{
		ActionOnRandomCard(() => { DiscardCurrentHoldingCard(); });
	}
	void GenerateManaFromRandomCard()
	{
		ActionOnRandomCard(() => { UseManaHability(); });
	}
	void ActionOnRandomCard(Action Action)
	{
		var currentCard = GetRandomCardFromHand();

		Hand.HoldCard(currentCard);

		Action();

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
