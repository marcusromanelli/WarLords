using System.Collections.Generic;
using UnityEngine;

public delegate void OnDeckClick(UserDeck deck);
public delegate void OnNewDeckClick();
public class DeckPanel : MonoBehaviour
{
	[SerializeField] SimpleListObject NameElementPrefab;
	[SerializeField] Transform DeckNameGroup;
	[SerializeField] bool ReadOnly;

	private SimpleListObject[] elements;
	private OnDeckClick onDeckClick;
	private OnNewDeckClick onNewDeckClick;

	public void Setup(OnNewDeckClick onNewDeckClick, OnDeckClick onDeckClick)
	{
		this.onNewDeckClick = onNewDeckClick;
		this.onDeckClick = onDeckClick;
	}
	public void Load(UserDeckList userDecks)
	{
		_load(userDecks);
	}
	void _load(UserDeckList civDecks)
	{
		EraseAll();

		var startIndex = 0;
		var deckList = civDecks.GetDecks();
		var targetSize = deckList.Length;

		if (onNewDeckClick != null)
		{
			targetSize++;

			elements = new SimpleListObject[targetSize];

			elements[0] = Instantiate(NameElementPrefab, DeckNameGroup);
			elements[0].Setup("  +  ", CreateNewDeck);

			startIndex = 1;
		}
		else
			elements = new SimpleListObject[targetSize];

		for (int i = startIndex; i < targetSize; i++)
		{
			var deck = deckList[i - startIndex];

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
		onNewDeckClick?.Invoke();
	}
}