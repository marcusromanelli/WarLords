using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HandleOnHoldingCard(Player player, CardObject cardObject);
public delegate bool HandleIsHoldingCard();
public delegate void HandleCardAction(int number);
public delegate void HandleOnPickSpawnArea();

public class Player : MonoBehaviour, IAttackable
{
	public event HandleCardAction OnDrawCard;
	public event HandleCardAction OnDiscardCard;
	public event HandleCardAction OnSendManaCreation;
	public event HandleOnHoldingCard OnHoldCard;
	public event HandleOnCardReleased OnCardReleasedOnGraveyard;
	public event HandleOnCardReleased OnCardReleasedOnManaPool;
	public event Action OnStartActionPhase;


	private const string playerPropertiesTag = "Player Properties";
	[BoxGroup(playerPropertiesTag), SerializeField, Expandable] PlayerCardDeck startDeck;

	private const string gameLogicTag = "Game Logic";
	[BoxGroup(gameLogicTag), SerializeField] protected Life Life;
	[BoxGroup(gameLogicTag), SerializeField] protected CardDeck PlayDeck;
	[BoxGroup(gameLogicTag), SerializeField] protected CardDeck Graveyard;
	[BoxGroup(gameLogicTag), SerializeField] protected ManaPool ManaPool;
	[BoxGroup(gameLogicTag), SerializeField] protected PlayerHand Hand;
	[BoxGroup(gameLogicTag), SerializeField] protected NextPhaseButton nextPhaseButton;
	[BoxGroup(gameLogicTag), SerializeField] protected MandatoryConditionManager conditionManager;
	[BoxGroup(gameLogicTag), SerializeField] protected HabilityManager habilityManager;

	private const string debugTag = "Debug";
	[BoxGroup(debugTag), SerializeField] protected bool infinityHabilitiesPerTurn;

	protected Battlefield battlefield;
	protected GameController gameController;
	protected InputController inputController;
	protected bool HasUsedHability;
	protected bool IsReadyToEndActionPhase = true;
	public bool IsOnActionPhase => !IsReadyToEndActionPhase;


	public virtual void PreSetup(Battlefield battlefield, GameController gameController, InputController inputController)
    {
		this.inputController = inputController;
		this.battlefield = battlefield;
		this.gameController = gameController;

		Life.Setup(GameConfiguration.startLife);

		habilityManager.Setup(this);

		ManaPool.Setup(this, CanPlayerSummonToken);

		Hand.PreSetup(this, battlefield, inputController, CanSummonToken);
		Hand.OnCardReleasedOnGraveyard += onCardReleasedOnGraveyard;
		Hand.OnCardReleasedOnManaPool += onCardReleasedOnManaPool;
		Hand.OnCardReleasedOnSpawnArea += onCardReleasedOnSpawnArea;
		Hand.OnHoldCard += OnCardBeingHeld;

		conditionManager.Setup(this);
	}
    public void SetupPlayDeck()
	{
		PlayDeck.Setup(inputController);

		PlayDeck.AddCards(startDeck.Cards);

		PlayDeck.Shuffle();
	}
	public IEnumerator IsInitialized()
    {
		yield return PlayDeck.IsUIUpdating();
	}

    #region INTERACTION
	void OnCardBeingHeld(Player player, CardObject cardObject)
    {
		OnHoldCard?.Invoke(this, cardObject);
	}
	public void DiscardCurrentHoldingCard()
	{
		var currentCard = Hand.GetHoldingCard();
		var currentCardData = currentCard.Data;
		Hand.RemoveCard(currentCard);
		Graveyard.AddCard(currentCardData);


		LogController.LogDiscard(this);

		OnDiscardCard?.Invoke(1);
	}
	public bool HasManaSpace()
    {
		return ManaPool.HasManaSpace();
    }
	public void TurnHoldinCardIntoMana()
    {
		var currentCard = Hand.GetHoldingCard();
		var currentCardData = currentCard.Data;
		Hand.TurnCardIntoMana(currentCard, () => {
			Graveyard.AddCard(currentCardData);
		});

		ManaPool.IncreaseMaxMana();

		LogController.LogManaGeneration(this);

		TriggerManaCreation();
	}
	public int GetHandCardsNumber()
	{
		return Hand.Count;
	}
	public virtual bool CanInteract()
	{
		return gameController.CanPlayerInteract(this) && enabled;
	}
	public void OnTokenDied(CardObject cardObject, SpawnArea tile)
	{
		Graveyard.SendCardToDeckFromPosition(cardObject, CardPositionData.Create(tile.GetTopCardPosition(), tile.GetRotationReference()));
	}
	#endregion INTERACTION

	#region ACTION_PHASE
	public void StartActionPhase()
	{
		OnStartActionPhase?.Invoke();
		ManaPool.RestoreSpentMana();		
		IsReadyToEndActionPhase = false;
		nextPhaseButton?.Show();
	}
	public IEnumerator IsResolvingActionPhase()
	{
		while (!IsReadyToEndActionPhase)
		{
			yield return null;
		}
	}
	public void OnClickNextPhase()
	{
		if (IsReadyToEndActionPhase)
			return;

		IsReadyToEndActionPhase = true;
		nextPhaseButton?.Hide();
	}
	public void TakeDamage(uint damage)
	{
		Life.TakeDamage(damage);
	}
	public void Heal(uint health)
	{
		Life.Heal(health);
	}
	#endregion ACTION_PHASE

	#region SUMMON_HERO
	public CardObject IsHoldingCard()
    {
		return Hand.GetHoldingCard();
	}
	public bool CanPlayerSummonToken(CardObject cardObject, bool isSkillsOnly)
    {
		return CanInteract() && IsOnActionPhase && ManaPool.HasAvailableMana(cardObject.CalculateSummonCost(isSkillsOnly));
	}
	public bool CanSummonToken(CardObject cardObject, SpawnArea spawnArea, bool isSkillOnly)
    {
		var playerCan = CanPlayerSummonToken(cardObject, isSkillOnly);
		var playerCanSummonHero = !battlefield.PlayerHasTokenSummoned(this, cardObject);

		var canSummonOnSpawnArea = battlefield.CanSummonOnTile(this, spawnArea);

		var battleFieldCan = playerCanSummonHero && (canSummonOnSpawnArea);

		return playerCan && battleFieldCan;
	}
	public void SummonCostChanged(uint newCost)
    {
		ManaPool.RefreshPreviewedMana(newCost);
    }
	protected void TrySummonToken(CardObject cardObject, SpawnArea spawnArea)
	{
		if (!CanPlayerSummonToken(cardObject, spawnArea))
		{
			Debug.Log("You cannot summon this hero right now.");
			return;
		}

		var isSkillOnly = spawnArea.Token != null;

		Hand.RemoveCard(cardObject, false);

		ManaPool.SpendMana(cardObject.CalculateSummonCost(isSkillOnly));

		gameController.Summon(this, cardObject, spawnArea);
	}
	#endregion SUMMON_HERO

	#region CARD_DRAW
	public void TryDrawCards(int number = 1)
	{
		if (!CanInteract())
			return;

		number = Mathf.Clamp(number, 0, int.MaxValue);

		var hasCardsOndeck = PlayDeck.Count >= number;

        if (!hasCardsOndeck)
        {
			if (Graveyard.Count + PlayDeck.Count < number)
			{
				number = Graveyard.Count + PlayDeck.Count;
			}
			TurnGraveyardIntoDeck();
			TryDrawCards(number);
			return;
		}

		DoDrawCards(number);
	}
	void DoDrawCards(int number)
	{
		LogController.LogCardDrawn(this, number);

		Card[] cards = PlayDeck.DrawCards(number);

		Hand.AddCards(cards);
		TriggerCardDraw(number);
	}
	protected void TriggerCardDraw(int number)
	{
		OnDrawCard?.Invoke(number);
	}
	void TurnGraveyardIntoDeck()
	{
		PlayDeck.AddCards(Graveyard.GetAllCards());

		Graveyard.Empty();
	}
	#endregion CARD_DRAW

	#region MANDATORY_CONDITIONS
	public void AddCondition(MandatoryConditionType Type, int targetNumber = -1)
	{
		conditionManager.AddCondition(Type, targetNumber);
	}
	public List<MandatoryCondition> GetConditions()
	{
		return conditionManager.Conditions;
	}
	public bool HasConditions()
	{
		return conditionManager.HasAny();
	}
	public bool HasCondition(MandatoryConditionType condition, int targetNumber = -1)
	{
		return conditionManager.Has(condition, targetNumber);
	}
	public void RemoveCurrentCondition()
	{
		conditionManager.RemoveCurrent();
	}
	public void RemoveCondition(MandatoryCondition condition)
	{
		conditionManager.Remove(condition);
	}
	public void RemoveCondition(MandatoryConditionType condition)
	{
		conditionManager.Remove(condition);
	}
	#endregion MANDATORY_CONDITIONS


	protected void TriggerManaCreation()
	{
		OnSendManaCreation?.Invoke(1);
	}
	void onCardReleasedOnGraveyard(CardObject cardObject, GameObject releasedArea)
	{
		OnCardReleasedOnGraveyard?.Invoke(cardObject, releasedArea);
	}
	void onCardReleasedOnSpawnArea(CardObject cardObject, GameObject releasedArea)
	{
		var spawnArea = releasedArea.GetComponent<SpawnArea>();

		if (spawnArea == null)
			return;

		TrySummonToken(cardObject, spawnArea);
	}
	void onCardReleasedOnManaPool(CardObject cardObject, GameObject releasedArea)
	{
		OnCardReleasedOnManaPool?.Invoke(cardObject, releasedArea);
	}
    public uint GetLife()
    {
		return Life.Current;
    }
    public string GetName()
    {
		return name;
    }
}
