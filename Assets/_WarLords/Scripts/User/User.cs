using System;

[Serializable]
public struct User : ILoadable
{
    public DeckCollection userDecks;

    public User(DeckCollection userDecks)
    {
        this.userDecks = userDecks;
    }

    public void Initialize()
    {
        userDecks = new DeckCollection();
    }
    public DeckCollection GetDecks()
    {
        return userDecks;
    }
    public void SetDeckData(DeckCollection deckCollection)
    {
        userDecks = deckCollection;
    }

}