using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UserDeck
{
    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string[] Cards { get; private set; }

    private bool isSaved;

    public string[] GetCards() => Cards;
    public bool IsSaved() => isSaved;
    public string GetId() => Id;
    public string GetName() => Name;
    public UserDeck(UserDeck deckData)
    {
        this.Id = deckData.Id;
        this.Name = deckData.Name;
        this.Cards = deckData.Cards;

        if (deckData.Cards != null)
        {
            this.Cards = new string[deckData.Cards.Length];
            deckData.Cards.CopyTo(this.Cards, 0);
        }
        else
            this.Cards = new string[0];

        isSaved = false;
    }
    public UserDeck(string Id, string Name, string[] Cards)
    {
        this.Id = Id;
        this.Name = Name;
        this.Cards = Cards;
        isSaved = false;
    }
    public static UserDeck New()
    {
        var deck = new UserDeck();

        deck.Id = Guid.NewGuid().ToString();
        deck.Name = "New Deck";
        deck.Cards = new string[0];
        deck.isSaved = false;

        return deck;
    }
    public void SetName(string name)
    {
        Name = name;
        isSaved = false;
    }
    public bool HasCard(Card card)
    {
        return GetCardIndex(card.Id) >= 0;
    }
    public void AddCard(Card card)
    {
        var targetSize = Cards.Length + 1;

        var newDeck = new string[targetSize];

        newDeck[newDeck.Length - 1] = card.Id;

        Cards.CopyTo(newDeck, 0);

        Cards = newDeck;
    }
    public void RemoveCard(Card cardToRemove)
    {
        var index = GetCardIndex(cardToRemove.Id);

        if (index < 0)
            return;

        int numIdx = Array.IndexOf(Cards, cardToRemove.Id);
        List<string> tmp = new List<string>(Cards);
        tmp.RemoveAt(numIdx);
        Cards = tmp.ToArray();
    }
    int GetCardIndex(string id)
    {
        for(int i = 0; i < Cards.Length; i++)
            if (Cards[i] == id)
                return i;

        return -1;
    }
    public static bool operator ==(UserDeck a, UserDeck b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(UserDeck a, UserDeck b)
    {
        return !a.Equals(b);
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
