using NaughtyAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public struct RawBundleData
{
	[ReadOnly] public string Name;
	[ReadOnly] public string Id;
	[ReadOnly] public string ParentId;
	[ReadOnly] public AssetReference Bundle;

    public override int GetHashCode()
    {
		return Id.GetHashCode();
    }
}

[CreateAssetMenu(fileName = "Data Reference Library", menuName = "ScriptableObjects/Data Reference Library", order = 2)]
public class DataReferenceLibrary : ScriptableObject
{
	public Dictionary<string, List<RawBundleData>> CardsByCivilization
    {
        get
        {
            if (cardsByCivilization == null)
				CreateDictionary();

			return cardsByCivilization;
        }
    }

	[SerializeField] Dictionary<string, List<RawBundleData>> cardsByCivilization;


	public SerializableHashSet<RawBundleData> Cards => cards;
	[SerializeField, JsonProperty] SerializableHashSet<RawBundleData> cards;

	public SerializableHashSet<RawBundleData> Civilizations => civilizations;
	[SerializeField, JsonProperty] SerializableHashSet<RawBundleData> civilizations;



	public RawBundleData GetCivilization(string id)
	{
		RawBundleData value;

		civilizations.TryGetValue(new RawBundleData() { Id = id }, out value);

		return value;
	}
	public RawBundleData[] GetCivilizations()
	{
		return civilizations.GetList();
	}
	public RawBundleData GetCard(string id)
	{
		RawBundleData value;

		cards.TryGetValue(new RawBundleData() { Id = id }, out value);

		return value;
	}
	public RawBundleData[] GetCards(string[] ids)
	{
		RawBundleData[] finalCards = new RawBundleData[ids.Length];

		int c = 0;

		foreach(var cardId in ids)
		{
			RawBundleData value;

			cards.TryGetValue(new RawBundleData() { Id = cardId }, out value);

			finalCards[c++] = value;		
		}


		return finalCards;
	}

	void CreateDictionary()
    {
		cardsByCivilization = new Dictionary<string, List<RawBundleData>>();

		foreach (var civ in Civilizations)
        {
			if (!cardsByCivilization.ContainsKey(civ.Id))
				cardsByCivilization.Add(civ.Id, new List<RawBundleData>());


			foreach (var card in Cards)
			{
				if (card.ParentId == civ.Id)
					cardsByCivilization[civ.Id].Add(card);
			}
        }
    }

#if UNITY_EDITOR
	[Button("Refresh Civilization References")]
	public void RefreshCardReferences()
	{
		var path = Directory.GetParent(AssetDatabase.GetAssetPath(this));

		var relativePath = Path.GetRelativePath(Directory.GetParent(Application.dataPath).FullName, path.FullName);

		string[] files = Directory.GetFiles(relativePath, "*.asset", SearchOption.AllDirectories);

		cards = new SerializableHashSet<RawBundleData>();
		civilizations = new SerializableHashSet<RawBundleData>();

		foreach (var file in files)
		{
			var civ = AssetDatabase.LoadAssetAtPath<CivilizationData>(file);

			if (civ == null)
				continue;

			var address = AssetDatabase.AssetPathToGUID(file);

			AssetReference referenceBundle = new AssetReference(address);

			var civData = new RawBundleData() { Id = civ.GetId(), Name = civ.GetName(), Bundle = referenceBundle };

			civilizations.Add(civData);

			var references = civ.RefreshCardReferences();
			
			for(int index = 0; index < references.Count; index++)
			{
				var data = references[index];
				data.ParentId = civData.Id;
				references[index] = data;

				Cards.Add(references[index]);
			}
		}
	}
#endif
}
