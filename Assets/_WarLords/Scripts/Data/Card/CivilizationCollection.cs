using UnityEngine;

public enum CivilizationType
{
	Aeterna,
	Arkamore
}

[CreateAssetMenu(fileName = "Civilization Collection", menuName = "ScriptableObjects/Civilization-Collection", order = 2)]
public class CivilizationCollection : ScriptableObject
{
	public CivilizationType[] Civilizations;
}
