using System.Collections.Generic;
using UnityEngine;

public class User : LoadableObject<User>
{
    public Dictionary<string, List<UserDeck>> userDecks { get; private set; }
    public List<UserDeck> GetDecks(string civilizationid)
    {
        if(userDecks.ContainsKey(civilizationid))
            return userDecks[civilizationid];

        return new List<UserDeck>();
    }

    public User()
    {
        userDecks = new Dictionary<string, List<UserDeck>>();
    }

    public void AddNewDeck(string civilization)
    {
        if (!userDecks.ContainsKey(civilization))
            userDecks[civilization] = new List<UserDeck>();

        userDecks[civilization].Add(new UserDeck());
    }
}