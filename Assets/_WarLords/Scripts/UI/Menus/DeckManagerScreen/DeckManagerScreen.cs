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

	void Start () {
		var civData = CivilizationManager.GetData();
		var userData = UserManager.GetData();

		civilizationPanel.gameObject.SetActive(true);
		civilizationPanel.Setup(OnCivilizationClick);
		civilizationPanel.Load(civData);

		DeckList.Setup(OnDeckClick);
		DeckList.Load(userData.GetDecks());
	}
	
	void ReturnToMenu()
    {
		SceneController.LoadLevel(MenuScreens.Menu);
    }

	void OnCivilizationClick(CivilizationData civilizationData)
	{
		civilizationPanel.gameObject.SetActive(false);
		MenuCardList.gameObject.SetActive(true);
		MenuCardList.Setup(civilizationData);
	}
	void OnReturnToCivilizationClick()
	{
		civilizationPanel.gameObject.SetActive(true);
		MenuCardList.gameObject.SetActive(false);
	}
	void OnDeckClick(UserDeck deck)
    {
		if(deck == null)
        {
			var data = UserManager.GetData();
			data.AddNewDeck();

			DeckList.Load(data.GetDecks());
			return;
        }
    }
}
