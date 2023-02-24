using UnityEngine;



[CreateAssetMenu(fileName = "Civilization Graphics Data", menuName = "ScriptableObjects/Card/CivilizationGraphics", order = 2)]
public class CivilizationGraphicsData : ScriptableObject
{
	[SerializeField] GameObject BackCoverObject;
	[SerializeField] Texture BackCover;
	[SerializeField] GameObject Token;

	public GameObject GetBackCoverObject() => BackCoverObject;
	public Texture GetBackCoverTexture() => BackCover;
	public GameObject GetToken() => Token;
}
