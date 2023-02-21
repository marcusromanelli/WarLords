using System.Collections.Generic;
using UnityEngine;

public delegate void OnDeckClick(UserDeck? deck);
public class DeckPanel : MonoBehaviour
{
	[SerializeField] SimpleListObject NameElementPrefab;
	[SerializeField] Transform DeckNameGroup;

	private SimpleListObject[] elements;
	private OnDeckClick onDeckClick;

	public void Setup(OnDeckClick onDeckClick)
	{
		this.onDeckClick = onDeckClick;
	}
	public void Load(UserDeckList userDecks)
	{
		_load(userDecks);
	}
	void _load(UserDeckList civDecks)
	{
		EraseAll();

		var targetSize = 1;
		var deckList = civDecks.GetDecks();

		if (deckList != null && deckList.Length > 0)
			targetSize = deckList.Length + 1;

		elements = new SimpleListObject[targetSize];

		elements[0] = Instantiate(NameElementPrefab, DeckNameGroup);
		elements[0].Setup("  +  ", CreateNewDeck);

		for(int i = 1; i < targetSize; i++)
		{
			var deck = deckList[i - 1];

			elements[i] = Instantiate(NameElementPrefab, DeckNameGroup);
			elements[i].Setup(deck.GetName(), () => { OnClickDeck(deck); });
		}
	}
	void EraseAll()
	{
		if (elements == null)
			return;

		for (int i = 0; i < elements.Length; i++)
		{
			Destroy(elements[i].gameObject);
		}
	}
	public void OnClickDeck(UserDeck deck)
	{
		Debug.Log("Clicked deck " + deck.GetId());

		onDeckClick?.Invoke(deck);
	}
	public void CreateNewDeck()
	{
		Debug.Log("Clicked new deck");
		onDeckClick?.Invoke(null);
	}
}