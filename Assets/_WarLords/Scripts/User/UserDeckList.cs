using Newtonsoft.Json;
using System;

[Serializable]
public struct UserDeckList
{
    [JsonProperty] public string CivilizationId { get; private set; }
    [JsonProperty] public UserDeck[] decks { get; private set; }

    public UserDeckList(UserDeckList userDeckList)
    {
        this.CivilizationId = userDeckList.CivilizationId;

        if(userDeckList.decks == null)
        {
            decks = new UserDeck[0];
            return;
        }

        decks = new UserDeck[userDeckList.decks.Length];

        int i = 0;
        foreach(var deck in userDeckList.decks)
        {
            decks[i++] = new UserDeck(deck);
        }
    }
    public UserDeckList(string civId)
    {
        CivilizationId = civId;
        decks = new UserDeck[0];
    }
    public UserDeck[] GetDecks()
    {
        if (decks == null)
            decks = new UserDeck[0];

        var deckArray = new UserDeck[decks.Length];

        decks.CopyTo(deckArray, 0);

        return deckArray;
    }
    public void Update(UserDeck newDeckData)
    {
        if (decks == null)
            decks = new UserDeck[0];

        var found = false;
        for (int i = 0; i < decks.Length; i++)
        {
            var deck = decks[i];

            if (deck.Id == newDeckData.Id)
            {
                decks[i] = newDeckData;
                found = true;
            }
        }




        if (!found)
        {
            var newTargetSize = decks.Length + 1;
            var deckArray = new UserDeck[newTargetSize];

            deckArray[deckArray.Length - 1] = newDeckData;

            decks = new UserDeck[deckArray.Length];
            deckArray.CopyTo(decks, 0);
        }
    }

    public static implicit operator UserDeck[](UserDeckList list)
    {
        var array = new UserDeck[list.decks.Length];

        int i = 0;
        foreach (var deck in list.decks)
            array[i++] = deck;

        return array;
    }
    public static implicit operator UserDeckList(UserDeck[] deckList)
    {
        UserDeckList aux = new UserDeckList(deckList);

        return aux;
    }
}