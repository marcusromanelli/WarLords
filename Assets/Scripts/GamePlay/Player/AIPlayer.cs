using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
	[BoxGroup("AI Behavior"), SerializeField] protected AIHabilityManager aiHabilityManager;
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
	public override bool CanInteract()
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
		CardObject cardObject = GetRandomCardFromHand();

		if (cardObject == null)
			return;

		if (tile != null && CanSummonHero(cardObject, tile))
			TrySummonHero(cardObject, tile);
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
		ActionOnRandomCard(card => { DiscardCurrentHoldingCard(); });
	}
	void GenerateManaFromRandomCard()
	{
		ActionOnRandomCard(card => { aiHabilityManager.UseManaHability(card); });
	}
	void ActionOnRandomCard(Action<CardObject> Action)
	{
		var currentCard = GetRandomCardFromHand();

		if (currentCard == null)
			return;

		Hand.HoldCard(currentCard);

		Action(currentCard);

		Hand.CancelHandToCardInteraction();
	}
	CardObject GetRandomCardFromHand()
	{
		var handCards = Hand.GetCards();

		if (handCards.Count <= 0)
			return null;

		return handCards[UnityEngine.Random.Range(0, handCards.Count - 1)];
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
