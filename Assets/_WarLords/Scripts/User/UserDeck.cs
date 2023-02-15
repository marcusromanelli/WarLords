using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserDeck
{
    [SerializeField] string Id;
    [SerializeField] string Name;
    Dictionary<Card, int> Cards;

    public string GetId() => Id;
    public string GetName() => Name;
    public UserDeck()
    {
        Id = Guid.NewGuid().ToString();
        Name = "New Deck";
        Cards = new Dictionary<Card, int>();
    }
}
