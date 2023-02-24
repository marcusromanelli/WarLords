using NaughtyAttributes;
using UnityEngine;

public class DeckManagerScreen : MonoBehaviour {
	[SerializeField] DataReferenceLibrary dataReferenceLibrary;
	[SerializeField] UICardViewer uiCardViewer;
	[SerializeField] CivilizationPanel CivilizationPanel;
	[SerializeField] CardsPanel MenuCardList;
	[SerializeField] DeckPanel DeckList;
	[SerializeField] DeckEditPanel DeckCardList;
	[SerializeField] GameObject SaveButton;
	[SerializeField, ReadOnly] Card selectedCard;

	private CivilizationData currentCivilization;
	private DeckCollection deckCollection;
	private bool isEditingDeck;

	void Start () {
		var civData = CivilizationManager.GetAll();
		deckCollection = new DeckCollection(UserManager.GetData().GetDecks());

		CivilizationPanel.gameObject.SetActive(true);
		CivilizationPanel.Setup(OnCivilizationClick);
		CivilizationPanel.Load(civData);

		DeckList.Setup(OnNewDeckClick, OnDeckClick);
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

		CivilizationPanel.gameObject.SetActive(false);
		MenuCardList.gameObject.SetActive(true);
		MenuCardList.Setup(dataReferenceLibrary, civilizationData.GetId(), OnCardListClicked);
	}
	void OnNewDeckClick()
    {
		var civId = currentCivilization.GetId();

		deckCollection.AddNewDeck(civId);

		DeckList.Load(GetCurrentCivDeckList());

		SetDirty();
	}
	void OnDeckClick(UserDeck userDeck)
    {
		SetEditing(true);
		DeckList.gameObject.SetActive(false);
		DeckCardList.gameObject.SetActive(true);

		DeckCardList.Setup(dataReferenceLibrary, currentCivilization, (UserDeck)userDeck, OnReturnToDeckList, OnChangedDeckName, OnChangedDeckCardList);
	}
	void SetEditing(bool isEditing)
    {
		this.isEditingDeck = isEditing;

		OnCardListClicked(selectedCard);
	}
	void SetDirty()
    {
		SaveButton.SetActive(true);
	}
	void OnAddCardClicked()
    {
		DeckCardList.AddCardToCurrentDeck(selectedCard);
    }
	public void OnCardListClicked(Card card)
    {
		selectedCard = card;

		if (selectedCard == null)
			return;

		if(isEditingDeck)
			uiCardViewer.Show(card, OnAddCardClicked);
		else
			uiCardViewer.Show(card, null);
	}
	public void ReturnToMenu()
	{
		DeckCardList.Unload();
		CivilizationPanel.Unload();
		MenuCardList.Unload();

		SceneController.LoadLevel(GameScreens.Menu);
	}
	public void OnReturnToCivilizationList()
	{
		uiCardViewer.Hide();
		currentCivilization = null;
		CivilizationPanel.gameObject.SetActive(true);
		MenuCardList.gameObject.SetActive(false);
		DeckList.gameObject.SetActive(false);
		DeckCardList.gameObject.SetActive(false);
	}
	public void OnReturnToDeckList()
	{
		SetEditing(false);
		DeckList.gameObject.SetActive(true);
		DeckCardList.gameObject.SetActive(false);

		DeckList.Load(GetCurrentCivDeckList());
	}
	public void OnChangedDeckName(UserDeck deck, string name)
	{
		deck.SetName(name);

		OnChangedDeckCardList(deck);
	}
	public void OnChangedDeckCardList(UserDeck deck)
	{
		var civ = currentCivilization.GetId();

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
