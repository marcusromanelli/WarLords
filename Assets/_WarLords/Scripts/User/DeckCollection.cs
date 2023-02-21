using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public struct DeckCollection
{
    [JsonProperty] private UserDeckList[] civilizationDecks;

    /*public UserDeck[] this[string civId]
    {
        get
        {
            if (civilizationDecks == null)
                civilizationDecks = new UserDeckList[0];

            var index = GetCivilizationDeckIndex(civId);

            if (index < 0)
                index = AddCivilizationDeck(civId);

            return civilizationDecks[index];
        }
    }*/
    public DeckCollection(UserDeckList[] civilizationDecks)
    {
        this.civilizationDecks = null;

        SetData(civilizationDecks);
    }
    public DeckCollection(DeckCollection deckCollection)
    {
        civilizationDecks = null;

        SetData(deckCollection.GetAllCivilizationDecks());
    }
    public UserDeckList[] GetAllCivilizationDecks()
    {
        if (civilizationDecks == null)
            civilizationDecks = new UserDeckList[0];

        var deckList = new UserDeckList[civilizationDecks.Length];

        civilizationDecks.CopyTo(deckList, 0);

        return deckList;
    }
    public bool HasCivilizationDeckList(string civId)
    {
        return GetCivilizationDeckListIndex(civId) >= 0;
    }
    public int GetCivilizationDeckListIndex(string civId)
    {
        if (civilizationDecks == null)
            return -1;

        for (int i = 0; i < civilizationDecks.Length; i++)
        {
            var deckList = civilizationDecks[i];
            if (deckList.CivilizationId == civId)
                return i;
        }

        return -1;
    }
    public void SetData(UserDeckList[] newDeckList)
    {
        civilizationDecks = new UserDeckList[newDeckList.Length];

        int i = 0;
        foreach(var deckList in newDeckList)
        {
            civilizationDecks[i++] = new UserDeckList(deckList);
        }
    }
    public void AddNewDeck(string civilization)
    {
        UserDeck[] civDecks;
        var civIndex = GetCivilizationDeckListIndex(civilization);

        if(civIndex < 0)
            civIndex = AddCivilizationDeck(civilization);

        var deckList = civilizationDecks[civIndex];

        var newDeck = UserDeck.New();

        deckList.Update(newDeck);

        civilizationDecks[civIndex] = deckList;
    }
    int AddCivilizationDeck(string civId)
    {
        if (civilizationDecks == null)
            civilizationDecks = new UserDeckList[0];

        var newTargetSize = civilizationDecks.Length + 1;
        var civDeckArray = new UserDeckList[newTargetSize];

        civilizationDecks.CopyTo(civDeckArray, 0);

        var deckList = new UserDeckList(civId);
        var indexer = civDeckArray.Length - 1;

        civDeckArray[indexer] = deckList;

        civilizationDecks = civDeckArray;

        return indexer;
    }
    public void UpdateDeck(string civilizationid, UserDeck deck)
    {
        var hasCiv = HasCivilizationDeckList(civilizationid);


        if (!hasCiv)
            AddCivilizationDeck(civilizationid);

        var civIndex = GetCivilizationDeckListIndex(civilizationid);
        var deckList = civilizationDecks[civIndex];

        deckList.Update(deck);

        civilizationDecks[civIndex] = deckList;
    }
    public UserDeckList GetCivilizationDeck(string civilizationid)
    {
        if (!HasCivilizationDeckList(civilizationid))
            AddCivilizationDeck(civilizationid);

        var civIndex = GetCivilizationDeckListIndex(civilizationid);

        return civilizationDecks[civIndex];
    }

}
