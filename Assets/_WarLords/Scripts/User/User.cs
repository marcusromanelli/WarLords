using UnityEngine;

public class User : LoadableObject<User>
{
    [SerializeField] UserDeck[] userDecks;

    public User()
    {
        userDecks = new UserDeck[0];
    }
}