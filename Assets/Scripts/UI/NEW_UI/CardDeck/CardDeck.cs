using System.Collections.Generic;
using UnityEngine;

public class CardDeck<T>
{
    [SerializeField] UICardDeck uiCardDeck;
    [SerializeField] Stack<T> Cards;
    [SerializeField] int NumberOfShuffles = 1;

    public int Count {  get {  return Cards.Count; } }

    public CardDeck()
    {
        Cards = new Stack<T>();
    }
    public void AddCard(T card)
    {
        if (card == null)
            return;

        Cards.Push(card);
    }
    public void AddCards(T[] cards, bool shuffle = true)
    {
        if (cards == null)
            return;

        for (int i = 0; i < cards.Length; i++) {
            var card = cards[i];
            Cards.Push(card);
        }

        if(shuffle)
            Shuffle();
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

                int randomIndex = Random.Range(i, tempList.Length);

                tempList[i] = tempList[randomIndex];

                tempList[randomIndex] = temp;
            }
        }

        Cards.Clear();

        AddCards(tempList, false);
    }
}
