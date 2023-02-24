using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class PlayerCardDeck : MonoBehaviour
{
    struct CardIdData
    {
        public RawBundleData Raw;
        public AsyncOperationHandle<Card> CardOperation;
        public Card Card => CardOperation.Result;

        public IEnumerator Load() {
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
            }
        }

        public override int GetHashCode()
        {
            return Raw.Id.GetHashCode();
        }
    }

    [SerializeField] Card[] cards;

    public Card[] Cards => cards;

    private HashSet<CardIdData> loadedCards;
    private DataReferenceLibrary dataReferenceLibrary;

    public void Setup(DataReferenceLibrary dataReferenceLibrary)
    {
        this.dataReferenceLibrary = dataReferenceLibrary;
    }
    public void SetDeck(UserDeck deck)
    {
        StartCoroutine(StartDeckLoading(deck));
    }

    IEnumerator StartDeckLoading(UserDeck deck)
    {
        var cardsString = deck.Cards;
        var cardList = new List<Card>();
        loadedCards = new HashSet<CardIdData>();

        foreach (var cardId in cardsString)
        {
            var rawCard = dataReferenceLibrary.GetCard(cardId);
            var cardIdData = new CardIdData() { Raw = rawCard };

            if (loadedCards.Contains(cardIdData))
            {
                CardIdData aux;
                loadedCards.TryGetValue(cardIdData, out aux);

                cardList.Add(aux.Card);
                continue;
            }

            yield return cardIdData.Load();

            cardList.Add(cardIdData.Card);
        }

        cards = cardList.ToArray();
    }
}
