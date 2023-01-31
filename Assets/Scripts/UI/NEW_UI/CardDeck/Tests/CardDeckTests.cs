using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardDeckTests
{
    /*Card GetRandomCard()
    {
        var card = new Card();

        card.CardID = UnityEngine.Random.Range(0, 9999);

        return card;
    }

    [Test]
    public void CardDeck_Add1Card()
    {
        Card toAdd = GetRandomCard();

        CardDeck<Card> deck = new CardDeck<Card>();

        deck.AddCard(toAdd);

        var deckCount = deck.Count;

        Assert.AreEqual(1, deckCount);
    }

    [Test]
    public void CardDeck_Add2Cards()
    {
        Card toAdd1 = GetRandomCard();

        Card toAdd2 = GetRandomCard();

        CardDeck<Card> deck = new CardDeck<Card>();

        deck.AddCard(toAdd1);
        deck.AddCard(toAdd2);

        var deckCount = deck.Count;

        Assert.AreEqual(2, deckCount);
    }


    [Test]
    public void CardDeck_Draw1Card()
    {
        Card toAdd = GetRandomCard();

        CardDeck<Card> deck = new CardDeck<Card>();

        deck.AddCard(toAdd);

        var deckCount = deck.Count;

        Card drawn = deck.DrawCard();

        Assert.AreEqual(toAdd, drawn);

        Assert.AreEqual(1, deckCount);
    }

    [Test]
    public void CardDeck_Draw2Cards()
    {
        Card toAdd1 = GetRandomCard();

        Card toAdd2 = GetRandomCard();

        CardDeck<Card> deck = new CardDeck<Card>();

        deck.AddCard(toAdd1);
        deck.AddCard(toAdd2);

        var deckCount = deck.Count;

        Card drawn1 = deck.DrawCard();
        Card drawn2 = deck.DrawCard();

        Assert.AreEqual(toAdd2, drawn1);

        Assert.AreEqual(toAdd1, drawn2);

        Assert.AreEqual(2, deckCount);
    }



    [Test]
    public void CardDeck_Shuffle()
    {
        CardDeck<Card> deck = new CardDeck<Card>();

        List<Card> pileOfCards = new List<Card>();
        for (int i = 0; i < 100; i++)
        {
            pileOfCards.Add(GetRandomCard());
        }

        deck.AddCards(pileOfCards.ToArray());

        deck.Shuffle();

        Card[] allCards = deck.DrawCards(pileOfCards.Count);

        float equalPercentage = 0;
        float cardPrecentage = 1 / allCards.Length;

        for(int i = 0; i < allCards.Length; i++)
        {
            var newValue = allCards[i].CardID;
            var oldValue = pileOfCards[i].CardID;

            var isEqual = newValue == oldValue;

            if (isEqual) 
                equalPercentage += cardPrecentage;
        }

        //Acceptable percentage of non-shuffled cards is 5%
        var acceptablePercentage = 0.05f;
        var totalPercentage = equalPercentage / allCards.Length;

        Assert.LessOrEqual(totalPercentage, acceptablePercentage);
    }


    [Test]
    public void CardDeck_GetAllCards()
    {
        Card toAdd1 = GetRandomCard();

        Card toAdd2 = GetRandomCard();

        Card toAdd3 = GetRandomCard();

        CardDeck<Card> deck = new CardDeck<Card>();

        deck.AddCard(toAdd1);
        deck.AddCard(toAdd2);
        deck.AddCard(toAdd3);

        var Cards = deck.GetAllCards();

        Assert.AreEqual(toAdd3, Cards[0]);
        Assert.AreEqual(toAdd2, Cards[1]);
        Assert.AreEqual(toAdd1, Cards[2]);
    }*/
}
