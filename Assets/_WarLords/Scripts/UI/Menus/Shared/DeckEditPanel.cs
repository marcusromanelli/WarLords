using System.Collections.Generic;
using TMPro;
using UnityEngine;

public delegate void OnClickReturnToDeckList();
public class DeckEditPanel : MonoBehaviour
{
	[SerializeField] CardsPanel CardsPanel;
	[SerializeField] TMP_InputField DeckName;

	private UserDeck userDeck;
	private CivilizationData civilizationData;
	private OnClickReturnToDeckList onClickReturnToDeckList;

	public void Setup(CivilizationData civilizationData, UserDeck userDeck, OnClickReturnToDeckList onClickReturnToDeckList)
	{
		this.onClickReturnToDeckList = onClickReturnToDeckList;
		this.civilizationData = civilizationData;
		this.userDeck = userDeck;

		DeckName.text = userDeck.GetName();

		CivilizationData.CardNameAndBundle[] cardList = ConvertDeckDataToList(civilizationData.GetAvailableCards(userDeck.GetCards()));
		CardsPanel.Setup(civilizationData, cardList);
	}
	CivilizationData.CardNameAndBundle[] ConvertDeckDataToList(Dictionary<CivilizationData.CardNameAndBundle, int> deckData)
	{
		List<CivilizationData.CardNameAndBundle> cardList = new List<CivilizationData.CardNameAndBundle>();

		foreach (var card in deckData)
		{
			for (int i = 0; i < card.Value; i++)
			{
				cardList.Add(card.Key);
			}
		}

		return cardList.ToArray();
	}	
	public void OnDeckNameChanged()
	{
		userDeck.SetName(DeckName.text);
	}
	public void OnClickReturn()
	{
		onClickReturnToDeckList?.Invoke();
	}
}