using NaughtyAttributes;
using UnityEngine;

public class SpawnArea: MonoBehaviour, ICardPlaceable
{
	/*[SerializeField] new Renderer renderer;
	[SerializeField] Color defaultColor = Color.white;
	[SerializeField] Color spawnAreaColor = Color.gray;
	[SerializeField] Color selectedColor = Color.black;*/
	[SerializeField] Transform cardPositionDataReference;
	[SerializeField, ReadOnly] bool isTemporarySpawnArea;
	[SerializeField, ReadOnly] CardObject token = null;

	public CardObject Token => token;
	public Vector2 GridPosition => gridPosition;

	private Vector2 gridPosition;
	private bool isSelectingSpawnArea;
	Color lastUsedColor;

	/*public bool IsSpawnArea => isSpawnArea;

	public void SetSpawnArea(bool value)
    {
		isSpawnArea = value;
	}*/

	public void SetPosition(int x, int y)
    {
		gridPosition = new Vector2(x, y);
    }
	public void SetSelectingSpawnArea()
    {
		isSelectingSpawnArea = true;

		UpdateColor();
	}
	public void StopSelectingSpawnArea()
	{
		isSelectingSpawnArea = false;

		UpdateColor();
	}
	public void SetSelected(bool isSelected)
	{
		UpdateColor(isSelected);
	}
	void UpdateColor(bool isSelected = false)
	{
		/*if (!isSelectingSpawnArea)
		{
			SetColor(defaultColor);
			return;
		}

        if (isSelected)
		{
			SetColor(selectedColor);
			return;
		}
		
		SetColor(spawnAreaColor);*/

	}
	void SetColor(Color color)
	{
		/*if (color == lastUsedColor)
			return;

		renderer.material.color = color;
		lastUsedColor = color;*/
	}
	public void SetToken(CardObject hero)
	{
		this.token = hero;
	}
	public void RemoveToken()
	{
		token = null;
	}
	/*
	private bool IsTemporarilySummonable;
	private Battlefield battlefield;*/

	public Vector3 GetTopCardPosition()
    {
		return cardPositionDataReference.transform.position;
	}
    public Quaternion GetRotationReference()
	{
		return cardPositionDataReference.transform.rotation;
	}
}