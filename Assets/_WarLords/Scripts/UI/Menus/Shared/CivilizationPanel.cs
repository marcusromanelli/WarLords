using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public delegate void OnCivilizationClick(CivilizationData civilizationData);
public class CivilizationPanel : MonoBehaviour
{
	[SerializeField] SimpleListObject NameElementPrefab;
	[SerializeField] Transform CivilizationNameGroup;

	private string currentCivilizationId = null;
	private AsyncOperationHandle<CivilizationData> civilizationDataHandler;
	private SimpleListObject[] elements;
	private OnCivilizationClick onCivilizationClick;

	public void Setup(OnCivilizationClick onCivilizationClick)
    {
		this.onCivilizationClick = onCivilizationClick;
	}
	public void Load(CivilizationRawData[] civilizationRawData)
    {
		_load(civilizationRawData);
	}
	void _load(CivilizationRawData[] civilizationRawData)
	{
		EraseAll();


		if (civilizationRawData == null || civilizationRawData.Length == 0)
		{
			elements = new SimpleListObject[1];

			elements[0] = Instantiate(NameElementPrefab, CivilizationNameGroup);
			elements[0].Setup("No civilizations found, or none loaded", null);

			return;
		}


		elements = new SimpleListObject[civilizationRawData.Length];
		int i = 0;
		foreach (var civ in civilizationRawData)
		{
			elements[i] = Instantiate(NameElementPrefab, CivilizationNameGroup);
			elements[i].Setup(civ.Id, () => { OnClickCivilization(civ.Id); });

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
	IEnumerator LoadCivilizationData(CivilizationRawData? civilizationRawData)
	{
		if (civilizationRawData == null)
			yield break;

		civilizationDataHandler = Addressables.LoadAssetAsync<CivilizationData>(civilizationRawData.Value.Bundle);

		yield return civilizationDataHandler;

		if (civilizationDataHandler.Status != AsyncOperationStatus.Succeeded)
		{
			Debug.LogError("Error downloading civilization data: " + civilizationDataHandler.OperationException.ToString());
			yield break;
		}


		onCivilizationClick?.Invoke(civilizationDataHandler.Result);
	}
	public void OnClickCivilization(string civilizationId)
	{
		if (civilizationDataHandler.IsValid())
			Addressables.Release(civilizationDataHandler);

		if (currentCivilizationId == civilizationId)
		{
			currentCivilizationId = null;
			return;
		}

		var civAssetReference = CivilizationManager.GetData(civilizationId);

		currentCivilizationId = civilizationId;

		StartCoroutine(LoadCivilizationData(civAssetReference));
	}
}
