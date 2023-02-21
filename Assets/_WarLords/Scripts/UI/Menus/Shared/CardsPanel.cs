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
	private CivilizationData civilizationData;
	private RawBundleData[] cardList;
	private AsyncOperationHandle<Card> cardDataHandler;
	private RuntimeCardData runtimeCardData;
	private OnSelectedCard onSelectedCard;

	public void Setup(CivilizationData civilizationData, OnSelectedCard onSelectedCard)
	{
		this.civilizationData = civilizationData;
		this.onSelectedCard = onSelectedCard;

		Refresh();
	}
	public void Refresh()
	{
		Load();
	}
	public void Setup(CivilizationData civilizationData, OnSelectedCard onSelectedCard = null, RawBundleData[] deckData = null)
	{
		cardList = deckData;
		Setup(civilizationData, onSelectedCard);
	}
	public void Unload()
	{
		EraseAll();

		if (cardDataHandler.IsValid())
			Addressables.Release(cardDataHandler);
	}
	public void OnClickCard(RawBundleData cardData)
	{
		if (cardDataHandler.IsValid())
			Addressables.Release(cardDataHandler);

		if (runtimeCardData != null && runtimeCardData.Name == cardData.Name)
		{
			runtimeCardData = null;
			return;
		}

		StartCoroutine(LoadCardData(cardData));
	}

	IEnumerator LoadCardData(RawBundleData cardData)
	{
		cardDataHandler = Addressables.LoadAssetAsync<Card>(cardData.Bundle);

		yield return cardDataHandler;

		if (cardDataHandler.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError("Error loading card data: " + cardDataHandler.OperationException.ToString());
			yield break;
		}

		var cardDataResult = cardDataHandler.Result;

		runtimeCardData = new RuntimeCardData(cardDataResult);

		onSelectedCard?.Invoke(cardDataResult);
	}

	void Load()
    {
		EraseAll();

		RenderCards(civilizationData);
	}
	void RenderCards(CivilizationData civilizationData)
    {

		RawBundleData[] cards;

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
