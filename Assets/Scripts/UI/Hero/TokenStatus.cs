using System;
using UnityEngine;

[Serializable]
public class TokenStatus 
{
	[SerializeField] UITokenStatus uiTokenStatus;


	private uint startLife;
	private uint currentLife;

	private uint startAttack;
	private uint currentAttack;
	public uint Life => currentLife;
	public uint Attack => currentAttack;

	public void Setup(uint startLife, uint startAttack)
    {
		this.startLife = currentLife = startLife;
		this.startAttack = currentAttack = startAttack;

		uiTokenStatus.Setup(startLife, startAttack);
	}
	void SetLife(uint newLife)
	{
		uiTokenStatus.SetLife(newLife);
	}
	void SetAttack(uint newAttack)
	{
		uiTokenStatus.SetAttack(newAttack);
	}
	public void TakeDamage(uint damage)
	{
		int life = (int)currentLife - (int)damage;

		currentLife = (uint)Mathf.Clamp(life, 0, startLife);

		SetLife(currentLife);
	}
	public void Heal(uint health)
	{
		currentLife += health;

		SetLife(currentLife);
	}
	public void AddAttack(uint attackValue)
	{
		currentAttack += attackValue;

		SetAttack(currentAttack);
	}
	public void RemoveAttack(uint attackValue)
	{
		int attack = (int)currentAttack - (int)attackValue;

		currentAttack = (uint)Mathf.Clamp(attack, 0, startAttack);

		SetAttack(currentAttack);
	}
}
