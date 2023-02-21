using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardsPanel : MonoBehaviour
{
    [SerializeField] SimpleListObject CardListObject;
    [SerializeField] CardContent UICard;
	[SerializeField] Transform CardListContainer;


    private SimpleListObject[] loadedCards;
	private CivilizationData civilizationData;
	private CivilizationData.CardNameAndBundle[] cardList;
	private AsyncOperationHandle<Card> cardDataHandler;
	private RuntimeCardData runtimeCardData;

	public void Setup(CivilizationData civilizationData)
	{
		this.civilizationData = civilizationData;

		Refresh();
	}
	public void Refresh()
	{
		Load();
	}
	public void Setup(CivilizationData civilizationData, CivilizationData.CardNameAndBundle[] deckData)
	{
		Setup(civilizationData);

		this.cardList = deckData;

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

		RenderCards(civilizationData);
	}
	void RenderCards(CivilizationData civilizationData)
    {

		CivilizationData.CardNameAndBundle[] cards;

		if (cardList == null)
		{
			cards = civilizationData.GetAvailableCards();
        }
        else
        {
			cards = cardList;
		}

		if (cards == null || cards.Length == 0)
		{
			Debug.Log("No cards found, or none loaded");

			loadedCards = new SimpleListObject[1];

			loadedCards[0] = Instantiate(CardListObject, CardListContainer);
			loadedCards[0].Setup("No cards found, or none loaded", null);
			return;
		}


		Debug.Log(civilizationData.GetName() + " loaded. There are " + cards.Length + " cards.");

		loadedCards = new SimpleListObject[cards.Length];
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
		loadedCards[index].Setup(assetData.Name, () => { OnClickCard(assetData); });
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
