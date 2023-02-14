using UnityEngine;
using System.Collections;
using TMPro;
using NaughtyAttributes;

public class Menu : MonoBehaviour {

	[SerializeField] GameObject NameElementPrefab;
	[SerializeField] Transform CivilizationNameGroup;


	GameObject[] elements;

	void Start () {
		Load();
	}

	void EraseAll()
	{
		if (elements == null)
			return;

		for (int i = 0; i < elements.Length; i++)
		{
			Destroy(elements[i]);
		}
	}
	[Button("Force Reload")]
	public void Load(){
		EraseAll();

		var civs = CivilizationManager.GetData();

		if (civs == null || civs.Length == 0)
		{
			elements = new GameObject[1];

			elements[0] = Instantiate(NameElementPrefab, CivilizationNameGroup);
			elements[0].GetComponentInChildren<TMP_Text>().text = "No civilizations found, or none loaded";

#if UNITY_EDITOR
			SceneController.Singleton.LoadLevel(MenuScreens.Loader);
#endif
			return;
        }


		elements = new GameObject[civs.Length];
		int i = 0;
		foreach(var civ in civs)
        {
			elements[i++] = Instantiate(NameElementPrefab, CivilizationNameGroup);

			elements[i - 1].GetComponentInChildren<TMP_Text>().text = civ.Name;
        }
	}
}
