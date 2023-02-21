using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UserDeck
{
    [JsonProperty] public string Id { get; private set; }
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public int Number { get; private set; }
    [JsonProperty] public string[] Cards { get; private set; }


    private Dictionary<string, int> cardsDic;


    private bool isSaved;
    private bool initialized;


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

        this.Number = 0;
        this.cardsDic = new Dictionary<string, int>();
        isSaved = false;
        initialized = false;
    }
    public UserDeck(string Id, string Name, string[] Cards)
    {
        this.Id = Id;
        this.Name = Name;
        this.Cards = Cards;
        this.Number = 0;
        this.cardsDic = new Dictionary<string, int>();
        isSaved = false;
        initialized = false;
    }
    public static UserDeck New()
    {
        var deck = new UserDeck();

        deck.Id = Guid.NewGuid().ToString();
        deck.Name = "New Deck";
        deck.Cards = new string[0];
        deck.isSaved = false;
        deck.Number = 0;

        return deck;
    }
    public void SetName(string name)
    {
        Name = name;
        Number++;
        isSaved = false;
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
