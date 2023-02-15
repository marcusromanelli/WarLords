using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MenuCardList : MonoBehaviour
{
    [SerializeField] CardNameObject CardListObject;
    [SerializeField] CardContent UICard;
	[SerializeField] Transform CardListContainer;


    private CardNameObject[] loadedCards;
	private CivilizationData data;
	private AsyncOperationHandle<Card> cardDataHandler;
	private RuntimeCardData runtimeCardData;

	public void Setup(CivilizationData data)
    {
		this.data = data;

		Load();
	}
	public void Unload()
	{
		EraseAll();
		UICard.Hide();
	}
	public void OnClickCard(CivilizationData.CardNameAndBundle cardData)
	{
		if (cardDataHandler.IsValid())
			Addressables.Release(cardDataHandler);

		if (runtimeCardData != null && runtimeCardData.Name == cardData.Name)
		{
			runtimeCardData = null;
			UICard.Hide();
			return;
		}

		StartCoroutine(LoadCardData(cardData));
	}

	IEnumerator LoadCardData(CivilizationData.CardNameAndBundle cardData)
	{
		cardDataHandler = Addressables.LoadAssetAsync<Card>(cardData.Bundle);

		yield return cardDataHandler;

		if (cardDataHandler.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError("Error loadnig card data: " + cardDataHandler.OperationException.ToString());
			yield break;
		}

		var cardDataResult = cardDataHandler.Result;

		runtimeCardData = new RuntimeCardData(cardDataResult);

		UICard.Show();
		UICard.SetData(runtimeCardData, null);
	}

	void Load()
    {
		EraseAll();

		RenderCards();
	}
	void RenderCards()
    {
		var cards = data.GetAvailableCards();

		if (cards == null || cards.Length == 0)
		{
			Debug.Log("No cards found, or none loaded");

			loadedCards = new CardNameObject[1];

			loadedCards[0] = Instantiate(CardListObject, CardListContainer);
			loadedCards[0].Setup(this, null);
			return;
		}


		Debug.Log(data.GetName() + " loaded. There are " + cards.Length + " cards.");

		loadedCards = new CardNameObject[cards.Length];
		int i = 0;
		foreach (var card in cards)
		{
			CreateCard(card, i);

			i++;
		}
	}
	void CreateCard(CivilizationData.CardNameAndBundle assetData, int index)
	{
		loadedCards[index] = Instantiate(CardListObject, CardListContainer);
		loadedCards[index].Setup(this, assetData);
	}
	void EraseAll()
	{
		if (loadedCards == null)
			return;

		for (int i = 0; i < loadedCards.Length; i++)
		{
			Destroy(loadedCards[i].gameObject);
		}

		loadedCards = null;
	}
}
