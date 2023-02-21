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
	[SerializeField] GameObject BackCoverObject;
	[SerializeField] Texture BackCover;
	[SerializeField] GameObject Token;
	[SerializeField] RawBundleData[] AvailableCards;

	public string GetId() => Id;
	public string GetName() => Name;
	public GameObject GetBackCoverObject() => BackCoverObject;
	public Texture GetBackCoverTexture() => BackCover;
	public GameObject GetToken() => Token;
	public RawBundleData[] GetAvailableCards() => AvailableCards;
	public RawBundleData FindCardByid(string id)
    {
		foreach (var card in AvailableCards)
			if (card.Id == id)
				return card;

		return default;
    }
	public Dictionary<RawBundleData, int> LoadCardReferences(string[] deckData) {

		Dictionary<RawBundleData, int> newDeckData = new Dictionary<RawBundleData, int>();

		foreach (var card in deckData)
        {
			var cardObj = FindCardByid(card);

			if(cardObj.Id == "")
            {
				Debug.Log("Card not found: " + card);
				continue;
            }


            if (!newDeckData.ContainsKey(cardObj))
				newDeckData.Add(cardObj, 0);

			newDeckData[cardObj]++;
		}

		return newDeckData;
	}

	public void OnValidate()
	{
		if (Id == "")
			Id = Guid.NewGuid().ToString();
	}

#if UNITY_EDITOR
	[Button("Refresh Card References")]
	public void RefreshCardReferences()
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

			list.Add(new RawBundleData() { Id = card.Id, Name = card.Name, Bundle = referenceBundle });
		}

		AvailableCards = list.ToArray();
	}
#endif
}
