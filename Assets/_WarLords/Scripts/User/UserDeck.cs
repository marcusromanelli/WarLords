
using System;
using System.Collections.Generic;

[Serializable]
public class UserDeck
{
    Dictionary<Card, int> cards;

    public UserDeck()
    {
        cards = new Dictionary<Card, int>();
    }
}
