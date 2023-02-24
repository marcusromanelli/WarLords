using UnityEngine;
using System.Collections;
using TMPro;
using NaughtyAttributes;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


public class CardVisualizerScreen : MonoBehaviour {

	[SerializeField] DataReferenceLibrary dataReferenceLibrary;
	[SerializeField] CivilizationPanel civilizationPanel;
	[SerializeField] CardsPanel MenuCardList;
	[SerializeField] UICardViewer uiCardViewer;

	void Start () {
		var data = CivilizationManager.GetAll();
		civilizationPanel.Setup(OnCivilizationClick);
		civilizationPanel.Load(data);
	}

	
	public void ReturnToMenu()
	{
		civilizationPanel.Unload();
		MenuCardList.Unload();

		SceneController.LoadLevel(GameScreens.Menu);
    }

	void OnCivilizationClick(CivilizationData civilizationData)
    {
		if (civilizationData == null)
		{
			MenuCardList.Unload();
			return;
		}

		MenuCardList.Setup(dataReferenceLibrary, civilizationData.GetId(), OnCardClick);
    }

	void OnCardClick(Card card)
    {
		if (card == null)
			uiCardViewer.Hide();
		else
			uiCardViewer.Show(card);
	}
}
