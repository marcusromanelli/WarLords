using UnityEngine;
using System.Collections;
using TMPro;
using NaughtyAttributes;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


public class CardVisualizerScreen : MonoBehaviour {

	[SerializeField] CivilizationPanel civilizationPanel;
	[SerializeField] CardsPanel MenuCardList;

	void Start () {
		var data = CivilizationManager.GetData();
		civilizationPanel.Setup(OnCivilizationClick);
		civilizationPanel.Load(data);
	}

	
	public void ReturnToMenu()
    {
		SceneController.LoadLevel(MenuScreens.Menu);
    }

	public void OnCivilizationClick(CivilizationData civilizationData)
    {
		MenuCardList.Setup(civilizationData);
    }
}
