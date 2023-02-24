using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;



[CreateAssetMenu(fileName = "Civilization Data", menuName = "ScriptableObjects/Card/Civilization", order = 2)]
public class CivilizationData : ScriptableObject
{
	[SerializeField, ReadOnly] string Id;
	[SerializeField] string Name;
	[SerializeField] CivilizationGraphicsData Graphics;

	public string GetId() => Id;
	public string GetName() => Name;

	public void OnValidate()
	{
		if (Id == "")
			Id = Guid.NewGuid().ToString();
	}

#if UNITY_EDITOR
	[Button("Refresh Card References")]
	public List<RawBundleData> RefreshCardReferences()
    {
		var path = Directory.GetParent(AssetDatabase.GetAssetPath(this));

		var relativePath = Path.GetRelativePath(Directory.GetParent(Application.dataPath).FullName, path.FullName);

		string[] files = Directory.GetFiles(relativePath, "*.asset", SearchOption.AllDirectories);

		List<RawBundleData> list = new List<RawBundleData>();

		foreach (var file in files)
		{
			var card = AssetDatabase.LoadAssetAtPath<Card>(file);

			if (card == null)
				continue;

			var address = AssetDatabase.AssetPathToGUID(file);

			AssetReference referenceBundle = new AssetReference(address);

			var parentFolder = Directory.GetParent(file);

			card.Name = parentFolder.Name.Replace("_", " ");
			card.Graphics = Graphics;

			EditorUtility.SetDirty(card);

			list.Add(new RawBundleData() { Id = card.Id, Name = card.Name, Bundle = referenceBundle });
		}

		return list;
	}
#endif
}
