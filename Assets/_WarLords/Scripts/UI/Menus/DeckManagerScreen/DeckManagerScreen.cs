using UnityEngine;

public class DeckManagerScreen : MonoBehaviour {
	[SerializeField] CivilizationPanel civilizationPanel;
	[SerializeField] CardsPanel MenuCardList;

	[SerializeField] DeckPanel DeckList;
	[SerializeField] DeckEditPanel DeckCardList;

	[SerializeField] GameObject SaveButton;

	private CivilizationData currentCivilization;
	private DeckCollection deckCollection;

	void Start () {
		var civData = CivilizationManager.GetData();
		deckCollection = new DeckCollection(UserManager.GetData().GetDecks());

		civilizationPanel.gameObject.SetActive(true);
		civilizationPanel.Setup(OnCivilizationClick);
		civilizationPanel.Load(civData);

		DeckList.Setup(OnDeckClick);
	}
	
	UserDeckList GetCurrentCivDeckList()
    {
		return deckCollection.GetCivilizationDeck(currentCivilization.GetId());
	}
	void OnCivilizationClick(CivilizationData civilizationData)
	{
		currentCivilization = civilizationData;
		DeckList.gameObject.SetActive(true);
		DeckList.Load(GetCurrentCivDeckList());

		civilizationPanel.gameObject.SetActive(false);
		MenuCardList.gameObject.SetActive(true);
		MenuCardList.Setup(civilizationData);
	}
	void OnDeckClick(UserDeck? userDeck)
    {
		if(userDeck == null)
        {
			var civId = currentCivilization.GetId();

			deckCollection.AddNewDeck(civId);

			DeckList.Load(GetCurrentCivDeckList());

			SetDirty();
			return;
        }
        else
        {
			DeckList.gameObject.SetActive(false);
			DeckCardList.gameObject.SetActive(true);

			DeckCardList.Setup(currentCivilization, (UserDeck)userDeck, OnReturnToDeckList, OnChangedDeckName);
		}
	}
	void SetDirty()
    {
		SaveButton.SetActive(true);
	}
	public void ReturnToMenu()
	{
		SceneController.LoadLevel(MenuScreens.Menu);
	}
	public void OnReturnToCivilizationList()
	{
		currentCivilization = null;
		civilizationPanel.gameObject.SetActive(true);
		MenuCardList.gameObject.SetActive(false);
		DeckList.gameObject.SetActive(false);
	}
	public void OnReturnToDeckList()
	{
		DeckList.gameObject.SetActive(true);
		DeckCardList.gameObject.SetActive(false);

		DeckList.Load(GetCurrentCivDeckList());
	}
	public void OnChangedDeckName(UserDeck deck, string name)
	{
		var civ = currentCivilization.GetId();

		deck.SetName(name);
		deckCollection.UpdateDeck(civ, deck);

		SetDirty();
	}
	public void Save()
	{
		var user = UserManager.GetData();
		user.SetDeckData(deckCollection);

		UserManager.UpdateData(user);
		SaveButton.SetActive(false);
	}
}
