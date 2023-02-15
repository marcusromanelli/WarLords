using UnityEngine;
using System.Collections;
using TMPro;
using NaughtyAttributes;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


public class DeckManagerScreen : MonoBehaviour {
	[SerializeField] CivilizationPanel civilizationPanel;
	[SerializeField] CardsPanel MenuCardList;

	[SerializeField] DeckPanel DeckList;
	[SerializeField] DeckEditPanel DeckCardList;

	private CivilizationData currentCivilization;
	private User userData;

	void Start () {
		var civData = CivilizationManager.GetData();
		userData = UserManager.GetData();

		civilizationPanel.gameObject.SetActive(true);
		civilizationPanel.Setup(OnCivilizationClick);
		civilizationPanel.Load(civData);

		DeckList.Setup(OnDeckClick);
	}
	

	void OnCivilizationClick(CivilizationData civilizationData)
	{
		currentCivilization = civilizationData;
		DeckList.gameObject.SetActive(true);
		DeckList.Load(userData.GetDecks(civilizationData.GetId()));

		civilizationPanel.gameObject.SetActive(false);
		MenuCardList.gameObject.SetActive(true);
		MenuCardList.Setup(civilizationData);
	}
	void OnDeckClick(UserDeck deck)
    {
		if(deck == null)
        {
			var civId = currentCivilization.GetId();

			userData.AddNewDeck(civId);

			DeckList.Load(userData.GetDecks(civId));
			return;
        }
        else
        {
			DeckList.gameObject.SetActive(false);
			DeckCardList.gameObject.SetActive(true);

			DeckCardList.Setup(currentCivilization, deck, OnReturnToDeckList);
		}
	}
	public void ReturnToMenu()
	{
		SceneController.LoadLevel(MenuScreens.Menu);
	}
	public void OnReturnToCivilizationList()
	{
		civilizationPanel.gameObject.SetActive(true);
		MenuCardList.gameObject.SetActive(false);
		DeckList.gameObject.SetActive(false);
	}
	public void OnReturnToDeckList()
	{
		DeckList.gameObject.SetActive(true);
		DeckCardList.gameObject.SetActive(false);
	}
	public void Save()
	{
		userData.Save();
	}
}
