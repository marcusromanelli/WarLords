using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public struct RawBundleData
{
	public string Id;
	public string Name;
	public AssetReference Bundle;
}

[CreateAssetMenu(fileName = "Civilization Collection", menuName = "ScriptableObjects/Civilization-Collection", order = 2)]
public class CivilizationCollection : ScriptableObject
{
	[SerializeField] RawBundleData[] Civilizations;

	public RawBundleData[] GetAvailableCivilizationRawData()
    {
		return Civilizations;
    }
	public RawBundleData? GetCivilizationRawData(string Id)
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

		List<RawBundleData> list = new List<RawBundleData>();

		foreach (var file in files)
		{
			var civ = AssetDatabase.LoadAssetAtPath<CivilizationData>(file);

			if (civ == null)
				continue;

			var address = AssetDatabase.AssetPathToGUID(file);

			AssetReference referenceBundle = new AssetReference(address);

			list.Add(new RawBundleData() { Id = civ.GetId(), Name = civ.GetName(), Bundle = referenceBundle });

			civ.RefreshCardReferences();
		}

		Civilizations = list.ToArray();
	}
#endif
}
