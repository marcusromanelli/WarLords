using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class PlayerCardDeck : MonoBehaviour
{
    class CardIdDataComparer : IEqualityComparer<CardIdData>
    {
        public bool Equals(CardIdData x, CardIdData y)
        {
            return x.Raw.Id == y.Raw.Id;
        }

        public int GetHashCode(CardIdData obj)
        {
            return obj.Raw.Id.GetHashCode();
        }
    }
    class CardIdData
    {
        public RawBundleData Raw;
        public int number;
        public AsyncOperationHandle<Card> CardOperation;
        public Card Card => CardOperation.Result;


        private Action<Card> onLoadFinishes;

        public IEnumerator Load(Action<Card> onLoadFinishes)
        {
            this.onLoadFinishes = onLoadFinishes;

            CardOperation = new AsyncOperationHandle<Card>();

            yield return LoadCardData(Raw, CardOperation);
        }

        IEnumerator LoadCardData(RawBundleData cardData, AsyncOperationHandle<Card> asyncOperation)
        {
            asyncOperation = Addressables.LoadAssetAsync<Card>(cardData.Bundle);

            yield return asyncOperation;

            if (asyncOperation.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Error loading card data: " + asyncOperation.OperationException.ToString());
                yield break;
            }

            CardOperation = asyncOperation;

            onLoadFinishes?.Invoke(asyncOperation.Result);
        }

        public override int GetHashCode()
        {
            return Raw.Id.GetHashCode();
        }
    }

    [SerializeField] Card[] cards = null;

    public Card[] Cards => cards;

    private HashSet<CardIdData> loadedCards;
    private DataReferenceLibrary dataReferenceLibrary;

#if UNITY_EDITOR
    public void Setup(DataReferenceLibrary dataReferenceLibrary, Card[] userCards)
    {
        this.dataReferenceLibrary = dataReferenceLibrary;

        cards = userCards;
    }
#endif

    public void Setup(DataReferenceLibrary dataReferenceLibrary, UserDeck userDeck)
    {
        this.dataReferenceLibrary = dataReferenceLibrary;

        SetDeck(userDeck);
    }
    public bool IsLoading()
    {
#if UNITY_EDITOR
        if(loadedCards == null && cards == null)
            return true;        

        return false;
#endif

        foreach (var cardData in loadedCards)
        {
            if (!cardData.CardOperation.IsValid() || cardData.CardOperation.IsValid() && cardData.CardOperation.Status != AsyncOperationStatus.Succeeded)
                return true;
        }

        return false;
    }
    void SetDeck(UserDeck deck)
    {
        StartDeckLoading(deck);
    }
    void StartDeckLoading(UserDeck deck)
    {
        var cardsString = deck.Cards;

        loadedCards = new HashSet<CardIdData>(new CardIdDataComparer());

        foreach (var cardId in cardsString)
        {
            var rawCard = dataReferenceLibrary.GetCard(cardId);
            var cardIdData = new CardIdData() { Raw = rawCard };

            if (loadedCards.Contains(cardIdData))
            {
                CardIdData aux;
                loadedCards.TryGetValue(cardIdData, out aux);
                aux.number++;

                loadedCards.Add(aux);
                continue;
            }

            cardIdData.number++;
            loadedCards.Add(cardIdData);

            StartCoroutine(cardIdData.Load(OnLoadFinishes));
        }
    }
    void OnLoadFinishes(Card card)
    {
        if (IsLoading())
            return;

        BuildDeck();
    }
    void BuildDeck()
    {
        List<Card> finalDeck = new List<Card>();

        foreach (var cardIdData in loadedCards)
        {
            for (int i = 0; i < cardIdData.number; i++)
            {
                finalDeck.Add(cardIdData.Card);
            }
        }

        cards = finalDeck.ToArray();
    }
}
