﻿using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class UILife : MonoBehaviour {

	[SerializeField] Transform chipObject;
	[SerializeField] TextMesh sideA;
	[SerializeField] TextMesh sideB;
	[SerializeField] float rotationSpeed = 350;
	[SerializeField] bool ROTATE;
	[SerializeField] Side currentSide;

	private uint currentLife;
	private bool hasInitialized = false;
	private Quaternion sideARotation = Quaternion.Euler(new Vector3(0, 180, 0));
	private Quaternion sideBRotation = Quaternion.Euler(new Vector3(0, 180, 180));

	private enum Side{ A, B }
	

	void Start(){
		currentSide = Side.A;
	}

	public void Setup(uint startLife)
    {
		SetLife(startLife);

		hasInitialized = true;
	}

	void UpdateLife()
	{
		if (!hasInitialized)
		{
			SetValue(sideA, currentLife);
			return;
		}

		switch (currentSide) {
			case Side.A:
				SetValue(sideB, currentLife);
				currentSide = Side.B;
				break;
			case Side.B:
				SetValue(sideA, currentLife);
				currentSide = Side.A;
				break;
		}

		transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles + Vector3.forward);
	}
	public void SetLife(uint newLife)
    {
		if(newLife > currentLife)
			GameConfiguration.PlaySFX(GameConfiguration.heal);
		else if (newLife < currentLife)
			GameConfiguration.PlaySFX(GameConfiguration.Hit);

		currentLife = newLife;

		UpdateLife();
	}
	void SetValue(TextMesh text, uint value)
	{
		text.text = value.ToString();

	}
	void RotateChip()
    {
		if (!Application.isPlaying)
			return;

		Quaternion targetRotation;

		switch (currentSide)
		{
			default:
			case Side.A:
				targetRotation = sideARotation;
				break;
			case Side.B:
				targetRotation = sideBRotation;
				break;
		}

		if(chipObject.rotation != targetRotation)
			chipObject.rotation = Quaternion.RotateTowards(chipObject.rotation, targetRotation, rotationSpeed * Time.deltaTime);
	}
	void Update () {
		RotateChip();
	}

	[Button("Force Rotate Chip", EButtonEnableMode.Playmode)]
	void ForceRotateChip()
	{
		SetLife(currentLife - 1);
	}
}