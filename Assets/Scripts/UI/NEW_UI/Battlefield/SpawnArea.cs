using NaughtyAttributes;
using UnityEngine;

public class SpawnArea: MonoBehaviour, ICardPlaceable
{
	[SerializeField] new Renderer renderer;
	[SerializeField] Color defaultColor = Color.white;
	[SerializeField] Color spawnAreaColor = Color.gray;
	[SerializeField] Color selectedColor = Color.black;
	[SerializeField] Transform cardPositionDataReference;
	[SerializeField, ReadOnly] bool isSpawnArea;

	bool IsSelectingSpawnArea;
	Color lastUsedColor;

	public bool IsSpawnArea => isSpawnArea;

	public void SetSpawnArea(bool value)
    {
		isSpawnArea = value;
	}
	public void SetSelectingSpawnArea()
    {
		IsSelectingSpawnArea = true;

		UpdateColor();
	}
	public void StopSelectingSpawnArea()
	{
		IsSelectingSpawnArea = false;

		UpdateColor();
	}
	public void SetSelected(bool isSelected)
	{
		UpdateColor(isSelected);
	}
	void UpdateColor(bool isSelected = false)
	{
		if (!isSpawnArea || !IsSelectingSpawnArea)
		{
			SetColor(defaultColor);
			return;
		}

        if (isSelected)
		{
			SetColor(selectedColor);
			return;
		}

		SetColor(selectedColor);

	}
	void SetColor(Color color)
	{
		if (color == lastUsedColor)
			return;

		renderer.material.color = color;
		lastUsedColor = color;
	}
	/*


	private bool IsTemporarilySummonable;
	private Hero Hero = null;
	private Battlefield battlefield;

	private void Awake()
    {
		if (renderer == null)
			renderer = gameObject.GetComponent<Renderer>();
	}
	public void MarkAsSummonArea()
	{
		SetColor(selectedColor);
	}
	public void MarkAsNormalArea()
	{
		SetColor(defaultColor);
	}
	public void SetHero(Hero hero)
	{
		Hero = hero;
	}
	public bool HasHero()
    {
		return Hero != null;
    }*/
	public Vector3 GetTopCardPosition()
    {
		return cardPositionDataReference.transform.position;
	}
    public Quaternion GetRotationReference()
	{
		return cardPositionDataReference.transform.rotation;
	}
}