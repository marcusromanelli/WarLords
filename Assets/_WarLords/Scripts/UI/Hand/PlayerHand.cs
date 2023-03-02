using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void HandleOnCardReleased(CardObject card, GameObject releasedArea);
public delegate bool HandleCanReleaseCard();
public delegate bool HandleCanSummonToken(CardObject cardObject, SpawnArea targetArea, bool isSkillOnly);
public delegate bool HandleCanPlayerSummonToken(CardObject cardObject, bool isSkillOnly);

[Serializable]
public class PlayerHand
{
    public event HandleOnCardReleased OnCardReleasedOnGraveyard;
    public event HandleOnCardReleased OnCardReleasedOnManaPool;
    public event HandleOnCardReleased OnCardReleasedOnSpawnArea;


    [SerializeField] UIPlayerHand uiPlayerHand;
    [SerializeField, ReadOnly] List<Card> Cards;
    public int Count => Cards.Count;

    public PlayerHand()
    {
        Cards = new List<Card>();
    }
    public void PreSetup(Player player, Battlefield battlefield, InputController inputController, HandleCanSummonToken canSummonHero)
    {
        uiPlayerHand.PreSetup(player, battlefield, inputController, onCardReleasedOnGraveyard, onCardReleasedOnManaPool, onCardReleasedOnSpawnArea, canSummonHero);
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
    public void RemoveCard(CardObject cardObject, bool destroyObject = true)
    {
        if (Cards.Count <= 0)
            return;

        Cards.Remove(cardObject.Data);

        uiPlayerHand.Remove(cardObject, destroyObject);
    }
    public void TurnCardIntoMana(CardObject cardObject, Action onFinishAnimation)
    {
        if (Cards.Count <= 0)
            return;

        Cards.Remove(cardObject.Data);

        uiPlayerHand.TurnCardIntoMana(cardObject, onFinishAnimation);
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
    public List<CardObject> GetCards()
    {
        return uiPlayerHand.GetCards();
    }
    public System.Collections.IEnumerator IsUIUpdating()
    {
        yield return uiPlayerHand.IsResolving();
    }
    public CardObject GetHoldingCard()
    {
        return uiPlayerHand.CurrentHoldingCard;
    }
    public void HoldCard(CardObject cardObject)
    {
        uiPlayerHand.HoldCard(cardObject);
    }
    public void CancelHandToCardInteraction()
    {
        uiPlayerHand.CancelHandToCardInteraction();
    }
    void onCardReleasedOnGraveyard(CardObject cardObject, GameObject releasedArea)
    {
        OnCardReleasedOnGraveyard?.Invoke(cardObject, releasedArea);
    }
    void onCardReleasedOnManaPool(CardObject cardObject, GameObject releasedArea)
    {
        OnCardReleasedOnManaPool?.Invoke(cardObject, releasedArea);
    }
    void onCardReleasedOnSpawnArea(CardObject cardObject, GameObject releasedArea)
    {
        OnCardReleasedOnSpawnArea?.Invoke(cardObject, releasedArea);
    }
}
