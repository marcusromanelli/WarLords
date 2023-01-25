using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HandleOnDrawCard(int number);
public delegate void HandleOnDiscardCard(int number);
public delegate void HandleOnSendCardToManaPool(int number);
public delegate void HandleOnPickSpawnArea();

public class Player : MonoBehaviour
{
	public event HandleOnDrawCard OnDrawCard;
	public event HandleOnDiscardCard OnDiscardCard;
	public event HandleOnSendCardToManaPool OnSendManaCreation;
	public event HandleOnPickSpawnArea OnPickSpawnArea;



	//In Game ONLY
	private const string playerPropertiesTag = "Player Properties";
	[BoxGroup(playerPropertiesTag), SerializeField] Civilization civilization;

	private const string gameLogicTag = "Game Logic";
	[BoxGroup(gameLogicTag), SerializeField] protected CardDeck<Card> PlayDeck;
	[BoxGroup(gameLogicTag), SerializeField] protected CardDeck<Card> Graveyard;
	[BoxGroup(gameLogicTag), SerializeField] protected ManaPool ManaPool;
	[BoxGroup(gameLogicTag), SerializeField] protected PlayerHand Hand;
	[BoxGroup(gameLogicTag), SerializeField] protected MandatoryConditionManager conditionManager;

	private const string debugTag = "Debug";
	[BoxGroup(debugTag), SerializeField] protected bool infinityHabilitiesPerTurn;


	protected bool HasUsedHability;
	protected GameController gameController;


	public virtual void Setup(GameController gameController, InputController inputController)
    {
		this.gameController = gameController;

		ManaPool.Setup();

		Hand.PreSetup(inputController);
		Hand.OnCardReleasedOnGraveyard += OnCardReleasedOnGraveyard;
		Hand.OnCardReleasedOnManaPool += OnCardReleasedOnManaPool;
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
	protected virtual bool CanInteract()
    {
		return gameController.CanPlayerInteract(this);
    }

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

	bool CanUseHability()
	{
		return infinityHabilitiesPerTurn || (CanInteract() && (ManaPool.HasManaSpace() && !HasUsedHability || HasCondition(MandatoryConditionType.SendCardToManaPool)));
	}

	#region CARD_TO_MANA_HABILITY

	bool CanGenerateMana()
	{
		return CanUseHability() || ManaPool.HasManaSpace() || HasCondition(MandatoryConditionType.SendCardToManaPool);
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
        if (!HasUsedHability)
			HasUsedHability = true;

		GenerateManaFromCurrentHoldingCard();
	}
	void GenerateManaFromCurrentHoldingCard()
    {
		var currentCard = Hand.GetHoldingCard();
		var currentCardData = currentCard.CardData;
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

	void OnCardReleasedOnGraveyard(CardObject card)
	{
		UseDiscardOneToDrawTwoHability();
	}
	void UseDiscardOneToDrawTwoHability()
    {
		if (!CanUseHability())
		{
			Debug.Log("You cannot use this hability right now.");
			return;
		}

		HasUsedHability = true;

		DiscardCurrentHoldingCard();

		TryDrawCards(2);
	}
	void DiscardCurrentHoldingCard()
    {
		var currentCard = Hand.GetHoldingCard();
		var currentCardData = currentCard.CardData;
		Hand.DiscardCard(currentCard);
		Graveyard.AddCard(currentCardData);

		OnSendManaCreation?.Invoke(1);
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
	public void Setup()
	{
		//var startLife = GameConfiguration.startLife;
		//LifePointsController.Setup(startLife);
	}

	public void StartTurn()
	{
		//ResetHabilities();
		//RecoverMana();
		//ResetPreviewMana();

		//TryDrawCards();
		//hasDrawnCard = true;
		//EndPhase();
	}
	public void DiscartCardFromHand(Card nCard)
	{
		return;/*
		Card card = Hand.Find(a => a.PlayID == nCard.PlayID);
		if (card.CardID > 0)
		{
			Hand.Remove(card);
			HandObject.RemoveCard(card.PlayID);
			Graveyard.AddCard(card);
			LogController.Log(Action.DiscartCard, this, 1);
		}*
	}

	void ResetHabilities()
	{
		//hasUsedHability = false;
		//hasDrawnCard = false;
	}

	public void SendCardToManaPool(Card card)
	{
		/*if (hasUsedHability)
		{
			if (!hasCondition(ConditionType.SendCardToManaPool))
			{
				GameConfiguration.PlaySFX(GameConfiguration.denyAction);
				Debug.Log(GetFormatedName() + " cannot use his hability because he already used this turn");
				return;
			}
		}
		if (!ManaPool.HasManaSpace())
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.Log("You already have the maximum mana permitted.");
			return;
		}*/

	/*var removedCard = Hand.RemoveAll(a => a.PlayID == card.PlayID) > 0;
	if (!removedCard)
	{
		Debug.LogError("Tried to remove a card from hand of " + GetFormatedName() + " that doesn't is there");
		return;

	}

	HandObject.RemoveCard(card.PlayID);*/
	/*ManaPool.IncreaseMaxMana();
	hasUsedHability = true;
	Debug.Log("Card ID nº " + card.PlayID + " became a Mana");*
}

public void DiscartCardToDrawTwo(Card card)
{
	/*if (hasUsedHability)
	{
		GameConfiguration.PlaySFX(GameConfiguration.denyAction);
		Debug.Log(GetFormatedName() + " cannot use his hability because he already used this turn");
		return;
	}*/

	/*var hasDiscarted = Hand.RemoveAll(a => a.PlayID == card.PlayID) > 0;


	if (!hasDiscarted)
	{
		Debug.LogError("Tried to remove a card from hand that doesn't is there");
		return;
	}*/

	/*Graveyard.AddCard(card);
	//HandObject.RemoveCard(card.PlayID);
	TryDrawCards(2);
	hasUsedHability = true;
	LogController.Log(Action.ChangeCard, this);
	Debug.Log("Card ID nº " + card + " has gone to Graveyard and " + GetFormatedName() + " drawed 2 cards");*
}

List<Card> shuffleCards(List<Card> cards)
{
	Card temp;
	for (int i = 0; i < cards.Count; i++)
	{
		temp = cards[i];
		int randomIndex = UnityEngine.Random.Range(i, cards.Count);
		cards[i] = cards[randomIndex];
		cards[randomIndex] = temp;
	}
	return cards;
}

Stack<Card> shuffleCards(Stack<Card> cards)
{
	List<Card> aux = new List<Card>(cards);
	Card temp;
	for (int i = 0; i < aux.Count; i++)
	{
		temp = aux[i];
		int randomIndex = UnityEngine.Random.Range(i, aux.Count);
		aux[i] = aux[randomIndex];
		aux[randomIndex] = temp;
	}
	return new Stack<Card>(aux);
}

public bool CanSpendMana(int number)
{
	return false;
	//return ManaPool.HasAvailableMana(number);
}

public void PreviewSpendMana(int number)
{
	//ManaPool.PreviewMana(number);
}
public void ResetPreviewMana()
{
	//ManaPool.RestorePreviewedMana();
}
public void AddMaxMana(int number)
{
	Debug.Log("Added " + number + " mana");

	GameConfiguration.PlaySFX(GameConfiguration.cardToEnergy);

	//ManaPool.SpendMana(number);
}
public void SpendMana(int number)
{
	if (!CanSpendMana(number))
	{
		//Debug.LogWarning("Not have enough mana. Requested: " + number + " - You have: " + ManaPool.CurrentMana);
		return;
	}


	Debug.Log("Spended " + number + " mana");
	//ManaPool.SpendMana(number);
}
public void RecoverMana(int number = -1)
{
	Debug.Log("Recovered " + number + " mana");
	//ManaPool.RestoreSpentMana(number);
}

public void setPlayerCivilization(Civilization civ)
{
	this.civilization = civ;
}

public void EndPhase()
{
	GameController.Singleton.NextPhase();
}
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

public void GetRandomCardFromGraveyard()
{
	/*if (Graveyard.Count > 0)
	{
		Card card = Graveyard.GetRandomCard();

		Hand.AddCard(card);

		GameConfiguration.PlaySFX(GameConfiguration.drawCard);
	}*
}

public void GetRandomCardFromDeck(int number = 1)
{
	TryDrawCards(number);
}

public string getName()
{
	return civilization.ToString();
}

public void SetCivilization(Civilization civilization)
{
	this.civilization = civilization;
}

public Civilization GetCivilization()
{
	return civilization;
}

public int GetCurrentHandNumber()
{
	return 1;// Hand.Count;
}

public int GetCurrentManaPoolCount()
{
	return 1;// ManaPool.CurrentMana;
}

public int GetCurrentGraveyardCount()
{
	return 1;// Graveyard.Count;
}
public int GetCurrentPlayDeckCount()
{
	return PlayDeck.Count;
}

public int GetCurrentLife()
{
	return 1;// life;
}

public PlayerType GetPlayerType()
{
	return PlayerType.None;// Type;
}

public bool HasDrawnTurnCard()
{
	return false;// hasDrawnCard;
}

public void SetDrawnCard(bool value)
{
	//hasDrawnCard = value;
}

public List<Condition> GetConditionList()
{
	return null;// Conditions;
}
public bool HasUsedHability()
{
	return false;// hasUsedHability;
}

public override string ToString()
{
	return "".ToString();
}

public void Summon(CardObject cardObject)
{
	//Hand.DiscardCard(cardObject.GetCardData());

	//Debug.Log(GetFormatedName() + " is summoning " + cardObject.GetCardData().name);
}

public void SetReleaseCard(CardObject card)
{
	OnReleaseCard?.Invoke(card);
}

public void SetHoldCard(CardObject card)
{
	OnHoldCard?.Invoke(card);
}

public void SetClickCard(CardObject card)
{
	OnClickCard?.Invoke(card);
}*/
}
