using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public struct CivilizationRawData
{
	public string Id;
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
	public CivilizationRawData? GetCivilizationRawData(string Id)
    {
		foreach (var civ in Civilizations)
			if (civ.Id == Id)
				return civ;

		return null;
	}

#if UNITY_EDITOR
	[Button("Refresh Civilization References")]
	public void RefreshCardReferences()
	{
		var path = Directory.GetParent(AssetDatabase.GetAssetPath(this));

		var relativePath = Path.GetRelativePath(Directory.GetParent(Application.dataPath).FullName, path.FullName);

		string[] files = Directory.GetFiles(relativePath, "*.asset", SearchOption.AllDirectories);

		List<CivilizationRawData> list = new List<CivilizationRawData>();

		foreach (var file in files)
		{
			var civ = AssetDatabase.LoadAssetAtPath<CivilizationData>(file);

			if (civ == null)
				continue;

			var address = AssetDatabase.AssetPathToGUID(file);

			AssetReference referenceBundle = new AssetReference(address);

			list.Add(new CivilizationRawData() { Id = civ.GetId(), Name = civ.GetName(), Bundle = referenceBundle });
		}

		Civilizations = list.ToArray();
	}
#endif
}
