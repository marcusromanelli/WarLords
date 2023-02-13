using UnityEngine;

[CreateAssetMenu(fileName = "Civilization Data", menuName = "ScriptableObjects/Card/Civilization", order = 2)]
public class CivilizationData : ScriptableObject
{
	public string Name;
	public GameObject BackCover;
	public GameObject Token;
}
