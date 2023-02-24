using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerCardDeck : MonoBehaviour
{
    [SerializeField] Card[] cards;

    public Card[] Cards => cards;

    private HashSet<AsyncOperationHandle<Card>> cardsDataHandler;

   /* public PlayerCardDeck(UserDeck deck)
    {
        StartCoroutine(StartDeckLoading(rawCards));
    }

    IEnumerator StartDeckLoading(UserDeck deck)
    {
        var cardList = new List<Card>();

        foreach (var cardData in rawCards)
        {
            var async = new AsyncOperationHandle<Card>();

            yield return LoadCardData(cardData.Key, async);

            for (int i = 0; i < cardData.Value; i++)
            {
                cardList.Add(async.Result);
            }
        }

        cards = cardList.ToArray();
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
    }*/
}
