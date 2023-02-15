using System.Collections.Generic;
using UnityEngine;

public delegate void OnDeckClick(UserDeck deck);
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
	public void Load(List<UserDeck> userDecks)
	{
		_load(userDecks);
	}
	void _load(List<UserDeck> userDecks)
	{
		EraseAll();

		elements = new SimpleListObject[userDecks.Count + 1];

		elements[0] = Instantiate(NameElementPrefab, DeckNameGroup);
		elements[0].Setup("  +  ", CreateNewDeck);
		int i = 1;
		foreach (var deck in userDecks)
		{
			elements[i] = Instantiate(NameElementPrefab, DeckNameGroup);
			elements[i].Setup(deck.GetName(), () => { OnClickDeck(deck); });

			i++;
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