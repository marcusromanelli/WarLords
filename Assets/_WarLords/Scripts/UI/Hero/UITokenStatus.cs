using UnityEngine;

[ExecuteInEditMode]
public class UITokenStatus : MonoBehaviour 
{
	[SerializeField] TextMesh attack;
	[SerializeField] TextMesh life;

	private uint currentLife;
	private uint currentAttack;

	public void Setup(uint startLife, uint startAttack)
    {
		SetLife(startLife);
		SetAttack(startAttack);
	}
	public void SetLife(uint newLife)
	{
		if (newLife > currentLife)
			GameRules.PlaySFX(GameRules.heal);
		else if (newLife < currentLife)
			GameRules.PlaySFX(GameRules.Hit);

		currentLife = newLife;

		SetValue(life, currentLife);
	}
	public void SetAttack(uint newLife)
	{
		currentAttack = newLife;

		SetValue(attack, currentAttack);
	}

	void SetValue(TextMesh text, uint value)
	{
		text.text = value.ToString();

	}	
}
