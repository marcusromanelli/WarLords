using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public delegate void OnCivilizationClick(CivilizationData civilizationId);
public class CivilizationPanel : MonoBehaviour
{
	[SerializeField] SimpleListObject NameElementPrefab;
	[SerializeField] Transform CivilizationNameGroup;

	private string currentCivilizationId = null;
	private SimpleListObject[] elements;
	private OnCivilizationClick onCivilizationClick;
	private AssetReference currentCivilizationReference;

	public void Setup(OnCivilizationClick onCivilizationClick)
    {
		this.onCivilizationClick = onCivilizationClick;
	}
	public void Load(RawBundleData[] civilizationRawData)
    {
		_load(civilizationRawData);
	}
	public void Unload()
	{
		DeallocateLastCivilization();
	}
	void _load(RawBundleData[] civilizationRawData)
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
			elements[i].Setup(civ.Name, () => { OnClickCivilization(civ.Id); });

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
	void OnLoadCivilizationData(CivilizationData civilizationRawData)
	{
		onCivilizationClick?.Invoke(civilizationRawData);
	}
	void DeallocateLastCivilization()
    {
		if (currentCivilizationReference == null)
			return;

		ResourceManager.DeallocateCivilization(currentCivilizationReference);
	}
	public void OnClickCivilization(string civilizationId)
	{
		DeallocateLastCivilization();

		if (currentCivilizationId == civilizationId)
		{
			currentCivilizationId = null;
			currentCivilizationReference = null;
			OnLoadCivilizationData(null);
			return;
		}

		var civAssetReference = CivilizationManager.GetData(civilizationId);

		currentCivilizationId = civilizationId;

		currentCivilizationReference = civAssetReference.Bundle;

		ResourceManager.AllocateCivilization(civAssetReference.Bundle, OnLoadCivilizationData);
	}
}
