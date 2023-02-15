using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserDeck
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public Dictionary<string, int> Cards { get; private set; }

    public string GetId() => Id;
    public string GetName() => Name;
    public void SetName(string name)
    {
        Name = name;
    }
    public Dictionary<string, int> GetCards() => Cards;
    public UserDeck()
    {
        Id = Guid.NewGuid().ToString();
        Name = "New Deck";
        Cards = new Dictionary<string, int>();
    }
}
