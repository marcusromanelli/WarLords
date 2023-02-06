using System;
using UnityEngine;

[Serializable]
public class Life 
{
	[SerializeField] UILife uiLife;

	private uint startLife;
	private uint currentLife;

	public uint Current => currentLife;

	public void Setup(uint startLife)
	{
		this.startLife = startLife;

		uiLife.Setup(startLife);
	}

	public void TakeDamage(uint value)
	{
		currentLife = (uint)Mathf.Clamp(currentLife - value, 0, currentLife);

		uiLife.SetLife(currentLife);
	}

	public void AddLife(uint value)
	{
		currentLife = (uint)Mathf.Clamp(currentLife + value, 0, currentLife);

		uiLife.SetLife(currentLife);
	}
}
