using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public struct CivilizationRawData
{
	public string Id;
	public AssetReference Bundle;
}

[CreateAssetMenu(fileName = "Civilization Collection", menuName = "ScriptableObjects/Civilization-Collection", order = 2)]
public class CivilizationCollection : ScriptableObject
{
	[SerializeField] CivilizationRawData[] Civilizations;

	public CivilizationRawData[] GetAvailableCivilizationRawData()
    {
		return Civilizations;
    }
	public CivilizationRawData? GetCivilizationRawData(string Id)
    {
		foreach (var civ in Civilizations)
			if (civ.Id == Id)
				return civ;

		return null;
    }
}
