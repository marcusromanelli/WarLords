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
	public event HandleOnCardReleased OnCardReleasedOnSpawnArea;
	public event Action OnStartActionPhase;


	private const string playerPropertiesTag = "Player Properties";
	[BoxGroup(playerPropertiesTag), SerializeField] Card[] deckStartCards;

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
	protected bool HasUsedHability;
	protected GameController gameController;
	protected bool IsReadyToEndActionPhase = true;
	public bool IsOnActionPhase => !IsReadyToEndActionPhase;


	public virtual void PreSetup(Battlefield battlefield, GameController gameController, InputController inputController)
    {
		this.battlefield = battlefield;
		this.gameController = gameController;

		Life.Setup(GameConfiguration.startLife);

		habilityManager.Setup(this);

		ManaPool.Setup(this, CanPlayerSummonHero);

		Hand.PreSetup(this, battlefield, inputController, CanSummonHero);
		Hand.OnCardReleasedOnGraveyard += onCardReleasedOnGraveyard;
		Hand.OnCardReleasedOnManaPool += onCardReleasedOnManaPool;
		Hand.OnCardReleasedOnSpawnArea += onCardReleasedOnSpawnArea;
		Hand.OnHoldCard += OnCardBeingHeld;

		conditionManager.Setup(this);
	}

    public void SetupPlayDeck()
	{
		PlayDeck.Setup();

		PlayDeck.AddCards(deckStartCards);

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
		Hand.DiscardCard(currentCard);
		Graveyard.AddCard(currentCardData);

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
	public bool CanPlayerSummonHero(Card card)
    {
		return CanInteract() && IsOnActionPhase && ManaPool.HasAvailableMana(card.ManaCost);
	}
	public bool CanSummonHero(Card card)
	{
		return CanSummonHero(card, null);
	}
	public bool CanSummonHero(Card card, SpawnArea spawnArea = null)
    {
		var playerCan = CanPlayerSummonHero(card);

		var playerCanSummonHero = !battlefield.PlayerHasHeroSummoned(this, card);
		var canSummonOnPassedSpawnArea = spawnArea != null && battlefield.CanSummonOnTile(this, spawnArea);
		var canSummonOnSelectedTile = spawnArea == null && battlefield.CanSummonOnSelectedTile(this);

		var battleFieldCan = playerCanSummonHero && (canSummonOnSelectedTile || canSummonOnPassedSpawnArea);

		return playerCan && battleFieldCan;
	}
	protected void TrySummonHero(CardObject cardObject)
	{
		TrySummonHero(cardObject, null);
	}
	protected void TrySummonHero(CardObject cardObject, SpawnArea spawnArea)
	{
		var cardData = cardObject.Data;

		if (!CanSummonHero(cardData, spawnArea))
		{
			Debug.Log("You cannot summon this hero right now.");
			return;
		}

		ManaPool.SpendMana(cardData.CalculateSummonCost());

		Hand.DiscardCard(cardObject);

		gameController.Summon(this, cardData, spawnArea);
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
		Debug.Log(GetFormatedName() + " drawed " + number + " cards");

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
	void onCardReleasedOnGraveyard(CardObject cardObject)
	{
		OnCardReleasedOnGraveyard?.Invoke(cardObject);
	}
	void onCardReleasedOnSpawnArea(CardObject cardObject)
	{
		TrySummonHero(cardObject);
	}
	void onCardReleasedOnManaPool(CardObject cardObject)
	{
		OnCardReleasedOnManaPool?.Invoke(cardObject);
	}
	string GetFormatedName()
	{
		return "Player " + (name);
	}
}
