using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HandleOnVIsualizeCard(Player player, RuntimeCardData cardObject);
public delegate bool HandleIsHoldingCard();
public delegate void HandleCardAction(int number);
public delegate void HandleOnPickSpawnArea();

public class Player : MonoBehaviour, IAttackable
{
	public event HandleCardAction OnDrawCard;
	public event HandleCardAction OnDiscardCard;
	public event HandleCardAction OnSendManaCreation;
	public event HandleOnVIsualizeCard OnVisualizeCard;
	public event HandleOnCardReleased OnCardReleasedOnGraveyard;
	public event HandleOnCardReleased OnCardReleasedOnManaPool;
	public event Action OnStartActionPhase;


	private const string playerPropertiesTag = "Player Properties";
	[BoxGroup(playerPropertiesTag), SerializeField] PlayerCardDeck startDeck;

	private const string gameLogicTag = "Game Logic";
	[BoxGroup(gameLogicTag), SerializeField] protected CardVisualizer uiCard;
	[BoxGroup(gameLogicTag), SerializeField] protected Life life;
	[BoxGroup(gameLogicTag), SerializeField] protected CardDeck playDeck;
	[BoxGroup(gameLogicTag), SerializeField] protected CardDeck graveyard;
	[BoxGroup(gameLogicTag), SerializeField] protected ManaPool manaPool;
	[BoxGroup(gameLogicTag), SerializeField] protected PlayerHand hand;
	[BoxGroup(gameLogicTag), SerializeField] protected NextPhaseButton nextPhaseButton;
	[BoxGroup(gameLogicTag), SerializeField] protected MandatoryConditionManager conditionManager;
	[BoxGroup(gameLogicTag), SerializeField] protected HabilityManager habilityManager;

	private const string debugTag = "Debug";
	[BoxGroup(debugTag), SerializeField] protected bool infinityHabilitiesPerTurn;

	protected Battlefield battlefield;
	protected MatchController gameController;
	protected InputController inputController;
	protected bool HasUsedHability;
	protected bool IsReadyToEndActionPhase = true;
	public bool IsOnActionPhase => !IsReadyToEndActionPhase;

#if UNITY_EDITOR
	public virtual void PreSetup(Card[] deckCards, Battlefield battlefield, MatchController gameController, InputController inputController, DataReferenceLibrary dataReferenceLibrary)
	{
		this.inputController = inputController;
		this.battlefield = battlefield;
		this.gameController = gameController;

		life.Setup(GameRules.startLife);

		habilityManager.Setup(this);

		manaPool.Setup(this, CanPlayerSummonToken);

		hand.PreSetup(this, battlefield, inputController, CanSummonToken);
		hand.OnCardReleasedOnGraveyard += onCardReleasedOnGraveyard;
		hand.OnCardReleasedOnManaPool += onCardReleasedOnManaPool;
		hand.OnCardReleasedOnSpawnArea += onCardReleasedOnSpawnArea;

		if (uiCard != null)
		{
			hand.OnCardStartHover += onCardStartHover;
			hand.OnCardEndHover += onCardEndHover;
		}

		conditionManager.Setup(this);

		startDeck.Setup(dataReferenceLibrary, deckCards);
	}
#endif

	public virtual void PreSetup(UserDeck deckData, Battlefield battlefield, MatchController gameController, InputController inputController, DataReferenceLibrary dataReferenceLibrary)
    {
		this.inputController = inputController;
		this.battlefield = battlefield;
		this.gameController = gameController;

		life.Setup(GameRules.startLife);

		habilityManager.Setup(this);

		manaPool.Setup(this, CanPlayerSummonToken);

		hand.PreSetup(this, battlefield, inputController, CanSummonToken);
		hand.OnCardReleasedOnGraveyard += onCardReleasedOnGraveyard;
		hand.OnCardReleasedOnManaPool += onCardReleasedOnManaPool;
		hand.OnCardReleasedOnSpawnArea += onCardReleasedOnSpawnArea;

		conditionManager.Setup(this);

		startDeck.Setup(dataReferenceLibrary, deckData);
	}
    public void SetupPlayDeck()
	{
		playDeck.Setup(startDeck.Cards[0].Graphics);

		playDeck.AddCards(startDeck.Cards);

		playDeck.Shuffle();
	}
    public void SetupGraveyard()
	{
		graveyard.Setup(startDeck.Cards[0].Graphics);
	}
	public bool IsLoading()
    {
		return startDeck.IsLoading();
	}


    #region INTERACTION
	public void DiscardCurrentHoldingCard()
	{
		var currentCard = hand.GetHoldingCard();
		var currentCardData = currentCard.Data;
		hand.RemoveCard(currentCard);
		graveyard.AddCard(currentCardData);


		LogController.LogDiscard(this);

		OnDiscardCard?.Invoke(1);
	}
	public bool HasManaSpace()
	{
		return manaPool.HasManaSpace();
	}
	public void PreviewMana(uint mana)
	{
		manaPool.PreviewMana(mana);
	}
	public bool HasAvailableMana(uint mana)
	{
		return manaPool.HasAvailableMana(mana);
	}
	public void TurnHoldinCardIntoMana()
    {
		var currentCard = hand.GetHoldingCard();
		var currentCardData = currentCard.Data;
		hand.TurnCardIntoMana(currentCard, () => {
			graveyard.AddCard(currentCardData);
		});

		manaPool.IncreaseMaxMana();

		LogController.LogManaGeneration(this);

		TriggerManaCreation();
	}
	public int GetHandCardsNumber()
	{
		return hand.Count;
	}
	public virtual bool CanInteract()
	{
		return gameController.CanPlayerInteract(this) && enabled;
	}
	public void OnTokenDied(CardObject cardObject, SpawnArea tile)
	{
		graveyard.SendCardToDeckFromPosition(cardObject, CardPositionData.Create(tile.GetTopCardPosition(), tile.GetRotationReference()));
	}
	public void SpendMana(uint value)
    {
		manaPool.SpendMana(value);
	}
	#endregion INTERACTION

	#region ACTION_PHASE
	public void StartActionPhase()
	{
		OnStartActionPhase?.Invoke();
		manaPool.RestoreSpentMana();		
		IsReadyToEndActionPhase = false;
		nextPhaseButton?.gameObject.SetActive(true);
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
		nextPhaseButton?.gameObject.SetActive(false);
	}
	public void TakeDamage(uint damage)
	{
		life.TakeDamage(damage);
	}
	public void Heal(uint health)
	{
		life.Heal(health);
	}
	#endregion ACTION_PHASE

	#region SUMMON_HERO
	public CardObject IsHoldingCard()
    {
		return hand.GetHoldingCard();
	}
	public bool CanPlayerSummonToken(CardObject cardObject, bool isSkillsOnly)
	{
		return CanPlayerSummonToken(cardObject.RuntimeCardData, isSkillsOnly);
	}
	public bool CanPlayerSummonToken(RuntimeCardData cardObject, bool isSkillsOnly)
    {
		return CanInteract() && IsOnActionPhase && manaPool.HasAvailableMana(cardObject.CalculateSummonCost(isSkillsOnly));
	}
	public bool CanSummonToken(CardObject cardObject, SpawnArea spawnArea, bool isSkillOnly)
    {
		return CanSummonToken(cardObject.RuntimeCardData, spawnArea, isSkillOnly);
    }
	public bool CanSummonToken(RuntimeCardData cardObject, SpawnArea spawnArea, bool isSkillOnly)
    {
		var playerCan = CanPlayerSummonToken(cardObject, isSkillOnly);
		var playerCanSummonHero = !battlefield.PlayerHasTokenSummoned(this, cardObject);

		var canSummonOnSpawnArea = battlefield.CanSummonOnTile(this, spawnArea);

		var battleFieldCan = playerCanSummonHero && (canSummonOnSpawnArea);

		return playerCan && battleFieldCan;
	}
	public void SummonCostChanged(uint newCost)
    {
		manaPool.PreviewMana(newCost);
    }
	protected void TrySummonToken(CardObject cardObject, SpawnArea spawnArea)
	{
		if (!CanPlayerSummonToken(cardObject, spawnArea))
		{
			Debug.Log("You cannot summon this hero right now.");
			return;
		}

		var isSkillOnly = spawnArea.Token != null;

		hand.RemoveCard(cardObject, false);

		SpendMana(cardObject.CalculateSummonCost(isSkillOnly));

		cardObject.SetVisualizing(false);

		gameController.Summon(this, cardObject, (totalCost) => {
			SpendMana(totalCost);

		}, spawnArea);
	}
	#endregion SUMMON_HERO

	#region CARD_DRAW
	public void TryDrawCards(int number = 1)
	{
		if (!CanInteract())
			return;

		number = Mathf.Clamp(number, 0, int.MaxValue);

		var hasCardsOndeck = playDeck.Count >= number;

        if (!hasCardsOndeck)
        {
			if (graveyard.Count + playDeck.Count < number)
			{
				number = graveyard.Count + playDeck.Count;
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

		Card[] cards = playDeck.DrawCards(number);

		hand.AddCards(cards);
		TriggerCardDraw(number);
	}
	protected void TriggerCardDraw(int number)
	{
		OnDrawCard?.Invoke(number);
	}
	void TurnGraveyardIntoDeck()
	{
		playDeck.AddCards(graveyard.GetAllCards());

		graveyard.Empty();
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


	void onCardStartHover(RuntimeCardData runtimeCardData)
    {
		OnVisualizeCard?.Invoke(this, runtimeCardData);	
		uiCard.Show(runtimeCardData);
    }
	void onCardEndHover(RuntimeCardData runtimeCardData)
	{
		OnVisualizeCard?.Invoke(this, null);
		uiCard.Hide(runtimeCardData);
    }
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
		return life.Current;
    }
    public string GetName()
    {
		return name;
    }
}
