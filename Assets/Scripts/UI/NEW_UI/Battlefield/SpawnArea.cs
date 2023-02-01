﻿using NaughtyAttributes;
using UnityEngine;

public class SpawnArea: MonoBehaviour, ICardPlaceable
{
	[SerializeField] new Renderer renderer;
	[SerializeField] Color defaultColor = Color.white;
	[SerializeField] Color spawnAreaColor = Color.gray;
	[SerializeField] Color selectedColor = Color.black;
	[SerializeField] Transform cardPositionDataReference;
	[SerializeField, ReadOnly] bool isTemporarySpawnArea;
	[SerializeField, ReadOnly] HeroObject hero = null;

	public HeroObject Hero => hero;

	bool IsSelectingSpawnArea;
	Color lastUsedColor;

	/*public bool IsSpawnArea => isSpawnArea;

	public void SetSpawnArea(bool value)
    {
		isSpawnArea = value;
	}*/
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
		if (!IsSelectingSpawnArea)
		{
			SetColor(defaultColor);
			return;
		}

        if (isSelected)
		{
			SetColor(selectedColor);
			return;
		}

		SetColor(spawnAreaColor);

	}
	void SetColor(Color color)
	{
		if (color == lastUsedColor)
			return;

		renderer.material.color = color;
		lastUsedColor = color;
	}
	public void SetHero(HeroObject hero)
	{
		this.hero = hero;
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