using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerHand
{
    [SerializeField] UIPlayerHand uiPlayerHand;
    [SerializeField, ReadOnly] List<Card> Cards;
    public int Count => Cards.Count;
    protected Civilization civilization;

    [SerializeField, ReorderableList] List<CardObject> hoveringCards = new List<CardObject>();
    [SerializeField] CardObject currentHoveringCard => hoveringCards.Count > 0 ? hoveringCards[hoveringCards.Count - 1] : null;

    public PlayerHand()
    {
        Cards = new List<Card>();
    }
    public void PreSetup(InputController inputController)
    {
        uiPlayerHand.PreSetup(inputController, OnDownCard, OnPressedCard, OnUpCard);
    }
    public void Setup(Civilization civilization)
    {
        uiPlayerHand.Setup(civilization);
    }
    public void AddCard(Card card)
    {
        if (Cards == null)
            return;

        Cards.Add(card);

        uiPlayerHand.AddCard(card);
    }
    public void AddCards(Card[] cards)
    {
        if (Cards == null)
            return;

        for (int i = 0; i < cards.Length; i++)
            AddCard(cards[i]);
    }
    public Card DiscardCard()
    {
        if (Cards.Count <= 0)
            return default(Card);


        Card card = Cards[0];

        Cards.RemoveAt(0);

        return card;
    }
    public Card[] DiscardCards(int number)
    {
        if (Cards.Count <= 0)
            return new Card[0];

        Card[] cards = new Card[number];

        for (int i = 0; i < number; i++)
        {
            Card card = Cards[0];

            Cards.RemoveAt(0);

            cards[i] = card;
        }


        return cards;
    }
    public Card[] GetCards()
    {
        return Cards.ToArray();
    }
    public System.Collections.IEnumerator IsUIUpdating()
    {
        yield return uiPlayerHand.IsResolving();
    }
    void OnClickCard(GameObject cardObject)
    {
        var card = cardObject.GetComponent<CardObject>();
        Debug.Log("Clicked " + card.name);

        if (currentHoveringCard != null && currentHoveringCard.gameObject == cardObject)
            return;

        hoveringCards.Add(cardObject.GetComponent<CardObject>());
    }
    void OnDownCard(GameObject cardObject)
    {
        var card = cardObject.GetComponent<CardObject>();
        Debug.Log("Mouse DOWN " + card.name);
    }
    void OnUpCard(GameObject cardObject)
    {
        var card = cardObject.GetComponent<CardObject>();
        Debug.Log("Mouse UP " + card.name);
    }
    void OnPressedCard(GameObject cardObject)
    {
        var card = cardObject.GetComponent<CardObject>();
        Debug.Log("Mouse PRESSED " + card.name);
    }
}
