using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public delegate void OnClickReturnToDeckList();
public delegate void OnChangedDeckCardList(UserDeck alteredDeck);
public delegate void OnChangedDeckName(UserDeck alteredDeck, string name);
public delegate void OnChangedDeck(UserDeck alteredDeck);
public class DeckEditPanel : MonoBehaviour
{
	[SerializeField] UICardViewer uiCardViewer;
	[SerializeField] CardsPanel CardsPanel;
	[SerializeField] TMP_InputField DeckName;
	[SerializeField, ReadOnly] Card selectedCard;

	private UserDeck currentUserDeck;
	private OnClickReturnToDeckList onClickReturnToDeckList;
	private OnChangedDeckName onChangedDeckName;
	private OnChangedDeckCardList onChangedDeckCardList;
	private CivilizationData currentCivilizationData;

	public void Setup(CivilizationData civilizationData, UserDeck userDeck, OnClickReturnToDeckList onClickReturnToDeckList, OnChangedDeckName onChangedDeckName, OnChangedDeckCardList onChangedDeckCardList)
	{
		this.onClickReturnToDeckList = onClickReturnToDeckList;
		this.onChangedDeckName = onChangedDeckName;
		this.onChangedDeckCardList = onChangedDeckCardList;
		this.currentUserDeck = userDeck;
		this.currentCivilizationData = civilizationData;

		RefreshCardList();
	}
	RawBundleData[] ConvertDeckDataToList(Dictionary<RawBundleData, int> deckData)
	{
		List<RawBundleData> cardList = new List<RawBundleData>();

		foreach (var card in deckData)
		{
			for (int i = 0; i < card.Value; i++)
			{
				cardList.Add(card.Key);
			}
		}

		return cardList.ToArray();
	}
	void RefreshCardList()
	{
		DeckName.text = currentUserDeck.GetName();

		RawBundleData[] cardList = ConvertDeckDataToList(currentCivilizationData.LoadCardReferences(currentUserDeck.GetCards()));
		CardsPanel.Setup(currentCivilizationData, OnClickCard, cardList);
	}
	public bool IsEditing()
    {
		return gameObject.activeSelf;
    }
	public void Unload()
	{
		CardsPanel.Unload();
	}
	public void OnDeckNameChanged()
	{
		onChangedDeckName?.Invoke(currentUserDeck, DeckName.text);
	}
	public void OnClickReturn()
	{
		onClickReturnToDeckList?.Invoke();
	}
	public void AddCardToCurrentDeck(Card card)
	{
		currentUserDeck.AddCard(card);

		RefreshCardList();

		onChangedDeckCardList?.Invoke(currentUserDeck);
	}
	public void RemoveCardToCurrentDeck(Card card)
	{
		currentUserDeck.RemoveCard(card);

		RefreshCardList();

		onChangedDeckCardList?.Invoke(currentUserDeck);

		if (!currentUserDeck.HasCard(card))
			uiCardViewer.Hide();
	}
	void OnClickRemoveCardButton()
    {
		RemoveCardToCurrentDeck(selectedCard);
	}
	void OnClickCard(Card card)
    {
		selectedCard = card;

		uiCardViewer.Show(card, null, OnClickRemoveCardButton);
	}
	/*public void OnRemoveCardonEditDeckToDeck()
	{
		onClickReturnToDeckList?.Invoke();
	}*/
	/*public void OnChangedDeck()
	{
		onChangedDeck?.Invoke(userDeck);
	}*/
}