using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{


	[SerializeField] List<int> RemainingCards;
	[SerializeField] List<int> Deck;


	//In Game ONLY
	[SerializeField] Civilization civilization;
	[SerializeField] int life;
	[SerializeField] PlayerType Type;
	[SerializeField] Stack<Card> PlayDeck;
	[SerializeField] Stack<Card> Graveyard;
	[SerializeField] List<Card> Hand;
	[SerializeField] List<Card> Field;
	[SerializeField] List<Card> ManaPool;
	[SerializeField] List<CardObject> Battlefield;
	[SerializeField] List<Condition> Conditions;


	[SerializeField] HandController HandObject;
	[SerializeField] DeckController DeckController;
	[SerializeField] ManaPoolController ManaPoolController;
	[SerializeField] GraveyardController GraveyardController;
	[SerializeField] BattlefieldController BattlefieldController;
	[SerializeField] LifePointsController LifePointsController;

	//Toss 1 card to draw 2
	//or
	//1 card = 1 mana
	[SerializeField] bool hasUsedHability;
	[SerializeField] bool hasDrawnCard;
	[SerializeField] bool isSelectingCard;
	[SerializeField] bool isDrawing;

	Action auxiliarAction;

	void Awake()
	{
		PlayDeck = new Stack<Card>();
		Graveyard = new Stack<Card>();
		ManaPool = new List<Card>();
		Field = new List<Card>();
		Hand = new List<Card>();
		life = GameConfiguration.startLife;
		Battlefield = new List<CardObject>();

		HandObject.setPlayer(this);
	}

	public void Setup()
	{
		DeckController.Setup(this, GetCurrentPlayDeckCount);
		GraveyardController.Setup(this, GetCurrentGraveyardCount);
		ManaPoolController.Setup(this, GetCurrentManaPoolCount);

		var startLife = GameConfiguration.startLife;
		LifePointsController.Setup(startLife);


	}

	public void StartGame()
	{
		Initialize();
	}

	public void StartTurn()
	{
		ResetHabilities();
		RecoverMana(ManaPool.Count);
		ResetPreviewMana();

		DrawCard();
		hasDrawnCard = true;
		EndPhase();
	}

	void Initialize()
	{
		PlayDeck = new Stack<Card>();
		int c = 1;
		Card aux2;

		////placeholder
		Deck.Clear();
		foreach (Card aux in CardsLibrary.Singleton.Cards.Cards.FindAll(a => a.civilization == civilization))
		{
			Deck.Add(aux.CardID);
			Deck.Add(aux.CardID);
			Deck.Add(aux.CardID);
		}
		////placeholder

		Deck.ForEach(delegate (int card) {
			aux2 = CardCollection.FindCardByID(card);
			aux2.PlayID = c;

			PlayDeck.Push(aux2);
			c++;
		});
		ShuffleDeck();
	}

	void ShuffleDeck()
	{
		Debug.Log(GetFormatedName() + " shuffled the deck");
		PlayDeck = shuffleCards(PlayDeck);
	}

	public void DrawCard(int number = 1)
	{
		if (number < 0)
			number = 1;

		if (PlayDeck.Count >= number)
		{
			StartCoroutine("drawCards", number);
		}
		else
		{
			if (Graveyard.Count + PlayDeck.Count < number)
			{
				number = Graveyard.Count + PlayDeck.Count;
			}
			TurnGraveyardIntoDeck();
			DrawCard(number);
		}
		LogController.Log(Action.DrawCard, this, number);
		Debug.Log(GetFormatedName() + " drawed " + number + " cards");
	}

	IEnumerator drawCards(int number)
	{
		isDrawing = true;
		for (int i = 0; i < number; i++)
		{
			Card card = PlayDeck.Pop();

			Hand.Add(card);
			HandObject.AddCard(card);
			GameConfiguration.PlaySFX(GameConfiguration.drawCard);

			if (i - 1 < number)
			{
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				yield return null;
			}
		}
		isDrawing = false;
	}

	public void DiscartCard(Card nCard)
	{
		Card card = Hand.Find(a => a.PlayID == nCard.PlayID);
		if (card.CardID > 0)
		{
			Hand.Remove(card);
			HandObject.RemoveCard(card.PlayID);
			Graveyard.Push(card);
			LogController.Log(Action.DiscartCard, this, 1);
		}
	}

	void ResetHabilities()
	{
		hasUsedHability = false;
		hasDrawnCard = false;
	}

	public void SendCardToManaPool(Card card)
	{
		if (hasUsedHability)
		{
			if (!hasCondition(ConditionType.SendCardToManaPool))
			{
				GameConfiguration.PlaySFX(GameConfiguration.denyAction);
				Debug.Log(GetFormatedName() + " cannot use his hability because he already used this turn");
				return;
			}
		}
		if (ManaPool.Count >= 12)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.Log("You already have the maximum mana permitted.");
			return;
		}
		if (Hand.RemoveAll(a => a.PlayID == card.PlayID) > 0)
		{
			HandObject.RemoveCard(card.PlayID);
			AddMana();
			hasUsedHability = true;
			Debug.Log("Card ID nº " + card.PlayID + " became a Mana");
		}
		else
		{
			Debug.LogError("Tried to remove a card from hand of " + GetFormatedName() + " that doesn't is there");
		}
	}


	public void AddMana(int number = 1)
	{
		GameConfiguration.PlaySFX(GameConfiguration.cardToEnergy);
		LogController.Log(Action.CreateEnergy, this, number);
		for (int i = 0; i < number; i++)
		{
			ManaPool.Add(new Card());
		}
	}

	void TurnGraveyardIntoDeck()
	{
		List<Card> aux = (new Stack<Card>(Graveyard)).ToList();
		aux.AddRange(PlayDeck);

		PlayDeck = new Stack<Card>(aux);
		Graveyard.Clear();
		ShuffleDeck();
	}

	public void DiscartCardToDrawTwo(Card card)
	{
		if (!hasUsedHability)
		{
			if (Hand.RemoveAll(a => a.PlayID == card.PlayID) > 0)
			{
				Graveyard.Push(card);
				HandObject.RemoveCard(card.PlayID);
				DrawCard(2);
				hasUsedHability = true;
				LogController.Log(Action.ChangeCard, this);
				Debug.Log("Card ID nº " + card + " has gone to Graveyard and " + GetFormatedName() + " drawed 2 cards");
			}
			else
			{
				Debug.LogError("Tried to remove a card from hand that doesn't is there");
			}
		}
		else
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.Log(GetFormatedName() + " cannot use his hability because he already used this turn");
		}
	}

	string GetFormatedName()
	{
		return "Player " + (civilization + 1);
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

	public bool canSpendMana(int number)
	{
		int manaNumber = ManaPool.FindAll(a => a.manaStatus != ManaStatus.Used).Count;
		if (number > manaNumber)
		{
			Debug.LogWarning("Not have enough mana. Requested: " + number + " - You have: " + manaNumber);
			return false;
		}
		return true;
	}

	public void previewSpendMana(int number)
	{
		int c = 0;
		Card aux;
		for (int i = 0; i < ManaPool.Count; i++)
		{
			if (ManaPool[i].manaStatus == ManaStatus.Active && c < number)
			{
				aux = ManaPool[i];
				aux.manaStatus = ManaStatus.Preview;
				ManaPool[i] = aux;
				c++;
			}
		}
		ManaPoolController.previewMana(number);
	}
	public void ResetPreviewMana()
	{
		Card aux;
		for (int i = 0; i < ManaPool.Count; i++)
		{
			if (ManaPool[i].manaStatus == ManaStatus.Preview)
			{
				aux = ManaPool[i];
				aux.manaStatus = ManaStatus.Active;
				ManaPool[i] = aux;
			}
		}
		ManaPoolController.recoverPreviewMana();
	}
	public void SpendMana(int number)
	{
		if (canSpendMana(number))
		{
			int c = 0;
			Card aux;
			for (int i = 0; i < ManaPool.Count; i++)
			{
				if (ManaPool[i].manaStatus != ManaStatus.Used && c != number)
				{
					aux = ManaPool[i];
					aux.manaStatus = ManaStatus.Active;
					ManaPool[i] = aux;
					c++;
				}
			}
			GameConfiguration.PlaySFX(GameConfiguration.useEnergy);
			Debug.Log("Spended " + number + " mana");
			ManaPoolController.spendMana(number);
		}
	}
	public void RecoverMana(int number)
	{
		int c = 0;
		Card aux;
		for (int i = 0; i < ManaPool.Count; i++)
		{
			if (ManaPool[i].manaStatus == ManaStatus.Used && c != number)
			{
				aux = ManaPool[i];
				aux.manaStatus = ManaStatus.Active;
				ManaPool[i] = aux;
				c++;
			}
		}
		GameConfiguration.PlaySFX(GameConfiguration.energyToCard);
		Debug.Log("Recovered " + number + " mana");
		ManaPoolController.recoverMana(number);
	}
	public void AddCondition(ConditionType Type, int number = -1)
	{
		Condition aux = gameObject.AddComponent<Condition>();
		aux.SetValues(Type, number);
		Conditions.Add(aux);
		if (!Conditions[0].isChecking)
		{
			Conditions[0].setActive();
		}
	}

	public void removeCondition(Condition condition)
	{
		Conditions.Remove(condition);
		if (Conditions.Count > 0)
			Conditions[0].setActive();
	}

	public bool HasConditions()
	{
		return (Conditions.Count > 0);
	}

	public bool hasCondition(ConditionType condition)
	{
		return (Conditions.Count > 0 && Conditions[0].Type == condition);
	}

	public void setPlayerCivilization(Civilization civ)
	{
		this.civilization = civ;
	}

	public void EndPhase()
	{
		GameController.Singleton.NextPhase();
	}

	public bool Summon(CardObject card, SpawnArea area)
	{
		int value = card.cardData.calculateCost();
		List<Hero> heroes = GameObject.FindObjectsOfType<Hero>().ToList();
		heroes.RemoveAll(a => a.card.CardID != card.cardData.CardID);
		if (heroes.Count <= 0)
		{
			if (canSpendMana(value))
			{
				SpendMana(value);

				card.SummonType = SummonType.Monster;
				BattlefieldController.Summon(card, area);
				Battlefield.Add(card);
				Hand.Remove(card.cardData);

				Debug.Log(GetFormatedName() + " is summoning " + card.name);
				return true;
			}
			else
			{
				GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			}
		}
		else
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("you can only have one type of the same hero in the field.");
		}
		return false;
	}
	public bool Summon(CardObject card)
	{
		int value = card.cardData.calculateCost();
		List<Hero> heroes = GameObject.FindObjectsOfType<Hero>().ToList();
		heroes.RemoveAll(a => a.card.CardID != card.cardData.CardID);
		if (heroes.Count <= 0)
		{
			if (canSpendMana(value))
			{
				SpendMana(value);

				card.SummonType = SummonType.Monster;
				BattlefieldController.Summon(card);
				Battlefield.Add(card);
				Hand.Remove(card.cardData);

				Debug.Log(GetFormatedName() + " is summoning " + card.cardData.name);
				return true;
			}
			else
			{
				GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			}
		}
		else
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("you can only have one type of the same hero in the field.");
		}
		return false;
	}

	public void TakeDamage(int value)
	{
		if (value < 0)
			value = 0;

		life -= value;
		if (life < 0) life = 0;
		LifePointsController.SetLife(life);
	}

	public void AddLife(int value)
	{
		if (value < 0)
			value = 0;
		life += value;

		GameConfiguration.PlaySFX(GameConfiguration.heal);
		LifePointsController.SetLife(life);
	}

	public void killCard(CardObject card)
	{
		BattlefieldController.Kill(card);
		Battlefield.Remove(card);
		Graveyard.Push(card.cardData);
	}

	public int getNumberOfHeroes()
	{
		return Battlefield.Count;
	}

	public void GetRandomCardFromGraveyard()
	{
		if (Graveyard.Count > 0)
		{
			Graveyard.Reverse();
			Card card = Graveyard.Pop();
			Graveyard.Reverse();
			Hand.Add(card);
			HandObject.AddCard(card);
			GameConfiguration.PlaySFX(GameConfiguration.drawCard);
		}
	}

	public void GetRandomCardFromDeck(int number = 1)
	{
		DrawCard(number);
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
		return Hand.Count;
	}

	public int GetCurrentManaPoolCount()
	{
		return ManaPool.Count;
	}

	public int GetEmptyBattleFieldNumber()
	{
		return Battlefield.FindAll(a => a.Character == null).Count;
	}

	public int GetCurrentGraveyardCount()
	{
		return Graveyard.Count;
	}
	public int GetCurrentPlayDeckCount()
	{
		return PlayDeck.Count;
	}

	public bool IsDeckFull()
	{
		return DeckController.GetNumberOfCards() == PlayDeck.Count;
	}

	public int GetCurrentLife()
	{
		return life;
	}

	public PlayerType GetPlayerType()
	{
		return Type;
	}

	public bool HasDrawnCard()
	{
		return hasDrawnCard;
	}

	public void SetDrawnCard(bool value)
	{
		hasDrawnCard = value;
	}

	public List<Condition> GetConditionList()
	{
		return Conditions;
	}

	public bool IsDrawing()
	{
		return isDrawing;
	}

	public bool HasUsedHability()
	{
		return hasUsedHability;
	}

	public HandController GetHandObject()
	{
		return HandObject;
	}

	public DeckController GetDeckController()
	{
		return DeckController;
	}

	public BattlefieldController GetBattlefieldController()
	{
		return BattlefieldController;
	}

	public ManaPoolController GetManaPoolController()
	{
		return ManaPoolController;
	}

	public GraveyardController GetGraveyardController()
	{
		return GraveyardController;
	}

	public List<Card> GetHand()
	{
		return Hand;
	}

	public List<CardObject> GetBattlefieldList()
	{
		return Battlefield;
	}

    public override string ToString()
    {
		return Type.ToString();
    }
}
