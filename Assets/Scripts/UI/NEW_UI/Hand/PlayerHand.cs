using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void HandleOnCardReleased(CardObject card);
public delegate bool HandleCanReleaseCard();
public delegate bool HandleCanSummonHero(Card cardData);

[Serializable]
public class PlayerHand
{
    public event HandleOnCardReleased OnCardReleasedOnGraveyard;
    public event HandleOnCardReleased OnCardReleasedOnManaPool;
    public event HandleOnCardReleased OnCardReleasedOnSpawnArea;


    [SerializeField] UIPlayerHand uiPlayerHand;
    [SerializeField, ReadOnly] List<Card> Cards;
    public int Count => Cards.Count;
    protected Civilization civilization;

    public PlayerHand()
    {
        Cards = new List<Card>();
    }
    public void PreSetup(InputController inputController, HandleCanReleaseCard canDiscardCard, HandleCanReleaseCard canGenerateMana, HandleCanSummonHero canSummonHero)
    {
        uiPlayerHand.PreSetup(inputController, onCardReleasedOnGraveyard, onCardReleasedOnManaPool, onCardReleasedOnSpawnArea, canDiscardCard, canGenerateMana, canSummonHero);
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
    public void DiscardCard(CardObject cardObject)
    {
        if (Cards.Count <= 0)
            return;

        Cards.Remove(cardObject.Data);

        uiPlayerHand.Discard(cardObject);
    }
    public void TurnCardIntoMana(CardObject cardObject)
    {
        if (Cards.Count <= 0)
            return;

        Cards.Remove(cardObject.Data);

        uiPlayerHand.TurnCardIntoMana(cardObject);
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
    public CardObject GetHoldingCard()
    {
        return uiPlayerHand.CurrentHoldingCard;
    }
    public void HoldCard(Card card)
    {
        uiPlayerHand.HoldCard(card);
    }
    public void CancelHandToCardInteraction()
    {
        uiPlayerHand.CancelHandToCardInteraction();
    }
    void onCardReleasedOnGraveyard(CardObject cardObject)
    {
        OnCardReleasedOnGraveyard?.Invoke(cardObject);
    }
    void onCardReleasedOnManaPool(CardObject cardObject)
    {
        OnCardReleasedOnManaPool?.Invoke(cardObject);
    }
    void onCardReleasedOnSpawnArea(CardObject cardObject)
    {
        OnCardReleasedOnSpawnArea?.Invoke(cardObject);
    }
}
