using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	/*public delegate void ReleaseCard(CardObject card);
	public event ReleaseCard OnReleaseCard;

	public delegate void HoldCard(CardObject card);
	public event HoldCard OnHoldCard;

	public delegate void ClickCard(CardObject card);
	public event ClickCard OnClickCard;*/



	//[SerializeField] List<int> RemainingCards;
	//[SerializeField] List<int> Deck;


	//In Game ONLY
	[Header("Player Properties")]
	[SerializeField] Civilization civilization;
	//[SerializeField] int life;
	//[SerializeField] PlayerType Type;
	[Header("Game Logic")]
	[SerializeField] CardDeck<Card> PlayDeck;
	[SerializeField] CardDeck<Card> Graveyard;
	[SerializeField] PlayerHand Hand;


	private InputController inputController;	
	//[SerializeField] ManaPool ManaPool;
	//[SerializeField] List<Condition> Conditions;


	//[SerializeField] HandController HandObject;
	//[SerializeField] UICardDeck DeckController;
	//[SerializeField] ManaPool ManaPoolController;
	//[SerializeField] GraveyardController GraveyardController;
	//[SerializeField] LifePointsController LifePointsController;
	//[SerializeField] Battlefield battlefield;

	//Toss 1 card to draw 2
	//or
	//1 card = 1 mana
	/*[SerializeField] bool hasUsedHability;
	[SerializeField] bool hasDrawnCard;
	[SerializeField] bool isSelectingCard;
	[SerializeField] bool isDrawing;*/

	//Action auxiliarAction;

	bool isFillingDeck;

	public void Setup(InputController inputController)
    {
		this.inputController = inputController;

		Hand.PreSetup(inputController);
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
	public IEnumerator IsInitialized()
    {
		yield return PlayDeck.IsUIUpdating();
	}
	public void TryDrawCards(int number = 1)
	{
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
		LogController.Log(Action.DrawCard, this, number);
		Debug.Log(GetFormatedName() + " drawed " + number + " cards");

		Card[] cards = PlayDeck.DrawCards(number);

		Hand.AddCards(cards);
	}
	void TurnGraveyardIntoDeck()
	{
		PlayDeck.AddCards(Graveyard.GetAllCards());

		Graveyard.Empty();
	}
	string GetFormatedName()
	{
		return "Player " + (civilization + 1);
	}
	public bool HasConditions()
	{
		return true;// (Conditions.Count > 0);
	}
	public void AddCondition(ConditionType Type, int number = -1)
	{
		/*Condition aux = gameObject.AddComponent<Condition>();
		aux.SetValues(Type, number);
		Conditions.Add(aux);
		if (!Conditions[0].isChecking)
		{
			Conditions[0].setActive();
		}*/
	}

	public void removeCondition(Condition condition)
	{
		/*Conditions.Remove(condition);
		if (Conditions.Count > 0)
			Conditions[0].setActive();*/
	}

	public bool hasCondition(ConditionType condition)
	{
		return true;// (Conditions.Count > 0 && Conditions[0].Type == condition);
	}



		/*void Start()
		{
			//ManaPool = new ManaPool();
			//Graveyard = new CardDeck<Card>(civilization);
			//Hand = new PlayerHand<Card>();

			//life = GameConfiguration.startLife;
		}

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
