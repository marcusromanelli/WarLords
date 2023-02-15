using System.Collections.Generic;
using UnityEngine;

public class User : LoadableObject<User>
{
    [SerializeField] List<UserDeck> userDecks;
    public List<UserDeck> GetDecks() => userDecks;

    public User()
    {
        userDecks = new List<UserDeck>();
    }

    public void AddNewDeck()
    {
        userDecks.Add(new UserDeck());
    }
}