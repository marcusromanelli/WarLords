using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HandleCardAction(int number);
public delegate void HandleOnPickSpawnArea();

public class Player : MonoBehaviour
{
	public event HandleCardAction OnDrawCard;
	public event HandleCardAction OnDiscardCard;
	public event HandleCardAction OnSendManaCreation;
	public event HandleOnPickSpawnArea OnPickSpawnArea;



	//In Game ONLY
	private const string playerPropertiesTag = "Player Properties";
	[BoxGroup(playerPropertiesTag), SerializeField] Civilization civilization;

	private const string gameLogicTag = "Game Logic";
	[BoxGroup(gameLogicTag), SerializeField] protected Battlefield Battlefield;
	[BoxGroup(gameLogicTag), SerializeField] protected CardDeck<Card> PlayDeck;
	[BoxGroup(gameLogicTag), SerializeField] protected CardDeck<Card> Graveyard;
	[BoxGroup(gameLogicTag), SerializeField] protected ManaPool ManaPool;
	[BoxGroup(gameLogicTag), SerializeField] protected PlayerHand Hand;
	[BoxGroup(gameLogicTag), SerializeField] protected NextPhaseButton nextPhaseButton;
	[BoxGroup(gameLogicTag), SerializeField] protected MandatoryConditionManager conditionManager;

	private const string debugTag = "Debug";
	[BoxGroup(debugTag), SerializeField] protected bool infinityHabilitiesPerTurn;


	protected bool HasUsedHability;
	protected GameController gameController;
	protected bool IsReadyToEndActionPhase = true;
	protected bool IsOnActionPhase => !IsReadyToEndActionPhase;


	public virtual void Setup(GameController gameController, InputController inputController)
    {
		this.gameController = gameController;

		ManaPool.Setup();

		Hand.PreSetup(inputController, CanDiscardCard, CanGenerateMana, CanSummonHero);
		Hand.OnCardReleasedOnGraveyard += OnCardReleasedOnGraveyard;
		Hand.OnCardReleasedOnManaPool += OnCardReleasedOnManaPool;
		Hand.OnCardReleasedOnSpawnArea += OnCardReleasedOnSpawnArea;
	}

    public void SetupPlayDeck()
	{
		PlayDeck.Setup(civilization);

		PlayDeck.Empty();

		var civCards = CardsLibrary.Singleton.Cards.Cards.FindAll(a => a.civilization == civilization).ToArray();

		PlayDeck.AddCards(civCards);

		PlayDeck.Shuffle();
	}
	public void SetupHand()
	{
		Hand.Setup(civilization);
	}
	public void SetupConditions()
	{
		conditionManager.Setup(this);
	}
	public IEnumerator IsInitialized()
    {
		yield return PlayDeck.IsUIUpdating();
	}
	public int GetHandCardsNumber()
    {
		return Hand.Count;
    }
	protected virtual bool CanInteract()
    {
		return gameController.CanPlayerInteract(this);
    }

	#region ACTIONS

	#region DRAW_PHASE
	public void StartDrawPhase()
    {
		TryDrawCards();
	}
	public IEnumerator IsResolvingDrawPhase()
	{
		yield return null;
	}
	#endregion DRAW_PHASE

	#region ACTION_PHASE
	public void StartActionPhase()
	{
		SetUsedHability(false);
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
    #endregion ACTION_PHASE

    #endregion ACTIONS

    #region SUMMON_HERO
	protected bool CanSummonHero(Card card)
    {
		var playerCan = CanInteract() && IsOnActionPhase && ManaPool.HasAvailableMana(card.manaCost);
		var battleFieldCan = !Battlefield.PlayerHasHero(this, card) && Battlefield.CanSummonOnSelectedTile(this);
		return playerCan && battleFieldCan;
	}
	void OnCardReleasedOnSpawnArea(CardObject card)
    {
		if(!CanSummonHero(card.Data))
		{
			Debug.Log("You cannot summon this hero right now.");
			return;
		}


		Debug.Log("Summoned!");
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

	#region HABILITIES

	void SetUsedHability(bool value)
    {
		HasUsedHability = value;
    }
	bool CanUseHability()
	{
		return (infinityHabilitiesPerTurn || !HasUsedHability) && IsOnActionPhase;
	}

	#region CARD_TO_MANA_HABILITY

	bool CanGenerateMana()
	{
		var hasHabilityOrCondition = CanUseHability() || HasCondition(MandatoryConditionType.SendCardToManaPool);
		var canHeUse = CanInteract() && ManaPool.HasManaSpace() && hasHabilityOrCondition;

		return infinityHabilitiesPerTurn || canHeUse;
	}
	protected void OnCardReleasedOnManaPool(CardObject card)
	{
		UseManaHability();
	}
	protected void UseManaHability()
    {
		if (!CanGenerateMana())
		{
			Debug.Log("You cannot create mana right now.");
			return;
		}

		CreateMana();
	}
	void CreateMana()
    {
		SetUsedHability(true);

		GenerateManaFromCurrentHoldingCard();
	}
	void GenerateManaFromCurrentHoldingCard()
    {
		var currentCard = Hand.GetHoldingCard();
		var currentCardData = currentCard.Data;
		Hand.TurnCardIntoMana(currentCard);
		Graveyard.AddCard(currentCardData);

		ManaPool.IncreaseMaxMana();

		TriggerManaCreation();
	}
	protected void TriggerManaCreation()
    {
		OnSendManaCreation?.Invoke(1);
	}

	#endregion CARD_TO_MANA_HABILITY

	#region 1_CARD_TO_TWO_HABILITY
	public bool CanDiscardCard()
	{
		var hasHability = CanUseHability();
		var hasCondition = HasCondition(MandatoryConditionType.DiscartCard);

		return hasHability || hasCondition;
	}
	void OnCardReleasedOnGraveyard(CardObject card)
	{
		CardOnGraveyardLogic();
	}
	void CardOnGraveyardLogic()
    {
        if (!IsReadyToEndActionPhase) {
			//If is on action phase

			UseDiscardOneToDrawTwoHability();
			return;
		}

		if (CanDiscardCard())
		{
			DiscardCurrentHoldingCard();
			return;
		}

		Debug.Log("You cannot discard a card right now.");
	}
	void UseDiscardOneToDrawTwoHability()
    {
		if (!CanUseHability())
		{
			Debug.Log("You cannot use this hability right now.");
			return;
		}

		SetUsedHability(true);

		DiscardCurrentHoldingCard();

		TryDrawCards(2);
	}
	public void DiscardCurrentHoldingCard()
    {
		var currentCard = Hand.GetHoldingCard();
		var currentCardData = currentCard.Data;
		Hand.DiscardCard(currentCard);
		Graveyard.AddCard(currentCardData);

		OnDiscardCard?.Invoke(1);
	}

    #endregion 1_CARD_TO_TWO_HABILITY

    #endregion	HABILITIES

    #region MANDATORY_CONDITIONS
    public void AddCondition(MandatoryConditionType Type, int number = -1)
	{
		conditionManager.AddCondition(Type, number);
	}
	public List<MandatoryCondition> GetConditions()
	{
		return conditionManager.Conditions;
	}
	public bool HasConditions()
	{
		return conditionManager.HasAny();
	}
	public bool HasCondition(MandatoryConditionType condition)
	{
		return conditionManager.Has(condition);
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




	string GetFormatedName()
	{
		return "Player " + (civilization + 1);
	}
	

	/*

public void TakeDamage(int value)
{
	/*if (value < 0)
		value = 0;

	life -= value;
	if (life < 0) life = 0;
	LifePointsController.SetLife(life);*
}

public void AddLife(int value)
{
	/*if (value < 0)
		value = 0;
	life += value;

	GameConfiguration.PlaySFX(GameConfiguration.heal);
	LifePointsController.SetLife(life);*
}

public void killCard(CardObject cardObject)
{
	//battlefield.Kill(cardObject);
	//Graveyard.AddCard(cardObject.GetCardData());
}

public int getNumberOfHeroes()
{
	return 0;
	//return battlefield.GetHeroes(this).Count;
}

public string getName()
{
	return civilization.ToString();
}


public void Summon(CardObject cardObject)
{
	//Hand.DiscardCard(cardObject.GetCardData());

	//Debug.Log(GetFormatedName() + " is summoning " + cardObject.GetCardData().name);
}
*/
}
