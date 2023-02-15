using UnityEngine;
using System.Collections;
using TMPro;
using NaughtyAttributes;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class CardVisualizerScreen : MonoBehaviour {

	[SerializeField] CivilizationNameObject NameElementPrefab;
	[SerializeField] Transform CivilizationNameGroup;
	[SerializeField] MenuCardList MenuCardList;



	private string currentCivilizationId = null;
	private AsyncOperationHandle<CivilizationData> civilizationDataHandler;
	private CivilizationNameObject[] elements;

	void Start () {
		Load();
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
	public void OnClickCivilization(string civilizationId)
    {
		if (civilizationDataHandler.IsValid())
			Addressables.Release(civilizationDataHandler);

		MenuCardList.Unload();

		if (currentCivilizationId == civilizationId)
		{
			currentCivilizationId = null;
			return;
		}

		var civAssetReference = CivilizationManager.GetData(civilizationId);

		currentCivilizationId = civilizationId;

		StartCoroutine(LoadCivilizationData(civAssetReference));
    }
	public void ReturnToMenu()
    {
		SceneController.LoadLevel(MenuScreens.Menu);
    }

	IEnumerator LoadCivilizationData(CivilizationRawData? civilizationRawData)
	{
		civilizationDataHandler = Addressables.LoadAssetAsync<CivilizationData>(civilizationRawData.Value.Bundle);

		yield return civilizationDataHandler;

		if (civilizationDataHandler.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError("Error downloading civilization data: " + civilizationDataHandler.OperationException.ToString());
			yield break;
		}

		MenuCardList.Setup(civilizationDataHandler.Result);
	}

	[Button("Force Render")]
	void Load(){
		EraseAll();

		var civs = CivilizationManager.GetData();

		if (civs == null || civs.Length == 0)
		{
			elements = new CivilizationNameObject[1];

			elements[0] = Instantiate(NameElementPrefab, CivilizationNameGroup);
			elements[0].Setup(this, "No civilizations found, or none loaded");

			return;
        }


		elements = new CivilizationNameObject[civs.Length];
		int i = 0;
		foreach(var civ in civs)
        {
			elements[i] = Instantiate(NameElementPrefab, CivilizationNameGroup);
			elements[i].Setup(this, civ.Id);

			i++;
        }
	}
}
