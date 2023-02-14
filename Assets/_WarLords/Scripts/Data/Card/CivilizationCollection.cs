using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public struct CivilizationRawData
{
	public string Name;
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
}
