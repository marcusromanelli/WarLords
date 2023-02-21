using System.Collections.Generic;
using TMPro;
using UnityEngine;

public delegate void OnClickReturnToDeckList();
public delegate void OnChangedDeckName(UserDeck alteredDeck, string name);
public delegate void OnChangedDeck(UserDeck alteredDeck);
public class DeckEditPanel : MonoBehaviour
{
	[SerializeField] CardsPanel CardsPanel;
	[SerializeField] TMP_InputField DeckName;

	private UserDeck userDeck;
	private OnClickReturnToDeckList onClickReturnToDeckList;
	private OnChangedDeckName onChangedDeckName;

	public void Setup(CivilizationData civilizationData, UserDeck userDeck, OnClickReturnToDeckList onClickReturnToDeckList, OnChangedDeckName onChangedDeckName)
	{
		this.onClickReturnToDeckList = onClickReturnToDeckList;
		this.onChangedDeckName = onChangedDeckName;
		this.userDeck = userDeck;

		DeckName.text = userDeck.GetName();

		CivilizationData.CardNameAndBundle[] cardList = ConvertDeckDataToList(civilizationData.LoadCardReferences(userDeck.GetCards()));
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
		onChangedDeckName?.Invoke(userDeck, DeckName.text);
	}
	public void OnClickReturn()
	{
		onClickReturnToDeckList?.Invoke();
	}
	/*public void OnChangedDeck()
	{
		onChangedDeck?.Invoke(userDeck);
	}*/
}