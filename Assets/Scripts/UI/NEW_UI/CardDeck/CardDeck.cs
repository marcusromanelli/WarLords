using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardDeck
{
    [SerializeField] protected UICardDeck uiCardDeck;
    [SerializeField] protected Stack<Card> Cards = new Stack<Card>();
    [SerializeField] protected int NumberOfShuffles = 1;
    public int Count {  get {  return Cards.Count; } }

    public void Setup()
    {
        Cards = new Stack<Card>();
    }
    public void AddCard(Card card)
    {
        if (card == null)
            return;

        Cards.Push(card);

        uiCardDeck.Add(card);
    }
    public void AddCards(Card[] cards)
    {
        if (cards == null)
            return;

        for (int i = 0; i < cards.Length; i++) {
            var card = cards[i];

            AddCard(card);        
        }
    }
    public Card DrawCard()
    {
        if (Cards.Count <= 0)
            return default(Card);

        return Cards.Pop();
    }
    public Card[] DrawCards(int number)
    {
        if (Cards.Count < number)
            return new Card[0];

        Card[] cardList = new Card[number];

        for(int i = 0; i < number; i++)
        {
            cardList[i] = DrawCard();
        }

        uiCardDeck.RemoveCards(number);

        return cardList;
    }
    public Card[] GetAllCards()
    {
        return Cards.ToArray();
    }
    public void Empty()
    {
        Cards.Clear();

        uiCardDeck.RemoveAll();
    }
    public void Shuffle()
    {
        Card[] tempList = Cards.ToArray();

        for (int shuffle = 0; shuffle < NumberOfShuffles; shuffle++)
        {
            for (int i = 0; i < tempList.Length; i++)
            {
                Card temp = tempList[i];

                int randomIndex = UnityEngine.Random.Range(i, tempList.Length);

                tempList[i] = tempList[randomIndex];

                tempList[randomIndex] = temp;
            }
        }

        Empty();

        AddCards(tempList);
    }
    public System.Collections.IEnumerator IsUIUpdating()
    {
        yield return uiCardDeck.IsResolving();
    }
}
