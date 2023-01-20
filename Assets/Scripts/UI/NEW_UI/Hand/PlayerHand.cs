using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerHand<T>
{
    [SerializeField] List<T> Cards;
    public int Count => Cards.Count;

    public PlayerHand()
    {
        Cards = new List<T>();
    }
    public void AddCard(T card)
    {
        if (Cards == null)
            return;

        Cards.Add(card);
    }
    public void AddCards(T[] cards)
    {
        if (Cards == null)
            return;

        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            Cards.Add(card);
        }
    }
    public T DiscardCard()
    {
        if (Cards.Count <= 0)
            return default(T);


        T card = Cards[0];

        Cards.RemoveAt(0);


        return card;
    }
    public T[] DiscardCards(int number)
    {
        if (Cards.Count <= 0)
            return new T[0];

        T[] cards = new T[number];

        for (int i = 0; i < number; i++)
        {
            T card = Cards[0];

            Cards.RemoveAt(0);

            cards[i] = card;
        }


        return cards;
    }
    public T[] GetCards()
    {
        return Cards.ToArray();
    }
}
