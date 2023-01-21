using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardDeck<T>
{
    [SerializeField] protected UICardDeck uiCardDeck;
    [SerializeField] protected Stack<T> Cards = new Stack<T>();
    [SerializeField] protected int NumberOfShuffles = 1;
    public int Count {  get {  return Cards.Count; } }

    protected Civilization civilization;

    public void Setup(Civilization civilization)
    {
        Cards = new Stack<T>();
        uiCardDeck.Setup(civilization);
    }
    public void AddCard(T card)
    {
        if (card == null)
            return;

        Cards.Push(card);
    }
    public void AddCards(T[] cards)
    {
        if (cards == null)
            return;

        for (int i = 0; i < cards.Length; i++) {
            var card = cards[i];
            Cards.Push(card);
        }

        UpdateUI();
    }
    public T DrawCard()
    {
        if (Cards.Count <= 0)
            return default(T);

        return Cards.Pop();
    }
    public T[] DrawCards(int number)
    {
        if (Cards.Count < number)
            return new T[0];

        T[] cardList = new T[number];

        for(int i = 0; i < number; i++)
        {
            cardList[i] = Cards.Pop();
        }

        return cardList;
    } 
    public int GetCardCount()
    {
        return Cards.Count;
    }
    public T[] GetAllCards()
    {
        return Cards.ToArray();
    }
    public void Empty()
    {
        Cards.Clear();
    }
    public T GetRandomCard()
    {
        Shuffle();

        return DrawCard();
    }
    public void Shuffle()
    {
        T[] tempList = Cards.ToArray();

        for (int shuffle = 0; shuffle < NumberOfShuffles; shuffle++)
        {
            for (int i = 0; i < tempList.Length; i++)
            {
                T temp = tempList[i];

                int randomIndex = UnityEngine.Random.Range(i, tempList.Length);

                tempList[i] = tempList[randomIndex];

                tempList[randomIndex] = temp;
            }
        }

        Cards.Clear();

        AddCards(tempList);
    }
    public System.Collections.IEnumerator IsUIUpdating()
    {
        yield return uiCardDeck.IsResolving();
    }
    void UpdateUI()
    {
        uiCardDeck.UpdateDeckSize(GetCardCount());
    }
}
