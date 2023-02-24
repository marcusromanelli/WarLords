using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public delegate void OnSelectedCard(Card card);
public class CardsPanel : MonoBehaviour
{
    [SerializeField] SimpleListObject CardListObject;
	[SerializeField] Transform CardListContainer;


    private SimpleListObject[] loadedCards;
	private RawBundleData[] cardList;
	private RuntimeCardData runtimeCardData;
	private OnSelectedCard onSelectedCard;
	private DataReferenceLibrary dataLibrary;
	private string civilizationId;
	private AssetReference currentCardReference;

	public void Setup(DataReferenceLibrary dataLibrary, string civilizationId, OnSelectedCard onSelectedCard)
	{
		this.dataLibrary = dataLibrary;
		this.civilizationId = civilizationId;
		this.onSelectedCard = onSelectedCard;

		Refresh();
	}
	public void Refresh()
	{
		Load();
	}
	public void Setup(DataReferenceLibrary dataLibrary, string civilizationId, OnSelectedCard onSelectedCard = null, RawBundleData[] deckData = null)
	{
		cardList = deckData;
		Setup(dataLibrary, civilizationId, onSelectedCard);
	}
	public void Unload()
	{
		EraseAll();

		DeallocateLastCard();
	}
	public void OnClickCard(RawBundleData cardData)
	{
		DeallocateLastCard();

		if (runtimeCardData != null && runtimeCardData.Name == cardData.Name)
		{
			runtimeCardData = null;

			OnSelectedCard(null);
			return;
		}

		currentCardReference = cardData.Bundle;

		ResourceManager.AllocateCard(currentCardReference, OnLoadCardData);
	}
	void DeallocateLastCard()
    {
		if (currentCardReference == null)
			return;

		ResourceManager.DeallocateCard(currentCardReference);
	}
	void OnLoadCardData(Card cardData)
	{
		var cardDataResult = cardData;

		runtimeCardData = new RuntimeCardData(cardDataResult);

		OnSelectedCard(cardDataResult);
	}

	void OnSelectedCard(Card card)
    {
		onSelectedCard?.Invoke(card);
	}

	void Load()
    {
		EraseAll();

		RenderCards();
	}
	void RenderCards()
    {

		RawBundleData[] cards;
		RawBundleData civ = dataLibrary.GetCivilization(civilizationId);

		if (cardList == null)
		{
			cards = dataLibrary.CardsByCivilization[civilizationId].ToArray();
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


		Debug.Log(civ.Name + " loaded. There are " + cards.Length + " cards.");

		loadedCards = new SimpleListObject[cards.Length];
		int i = 0;
		foreach (var card in cards)
		{
			CreateCard(card, i);

			i++;
		}
	}
	void CreateCard(RawBundleData assetData, int index)
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
