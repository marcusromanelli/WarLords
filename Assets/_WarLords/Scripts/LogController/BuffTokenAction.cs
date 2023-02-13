using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct BuffTokenAction : IGameAction
{
	public Player ownerPlayer;
	public CardObject targetToken;
	public CardObject summonedToken;
	public List<SkillData> usedSkills;
	public uint manaCost;

	public string GetDescription()
	{
		StringBuilder str = new StringBuilder();

		str.Append("[");
		str.Append(GetType().Name);
		str.Append("] - ");

		str.Append(ownerPlayer.GetName());
		str.Append(" skill-buffed ");
		str.Append(targetToken.GetName());
		str.Append(" using ");
		str.Append(summonedToken.GetName());
		str.Append(" 's skills: ");

		foreach(var skill in usedSkills)
		{
			str.Append(skill.ToString());
			str.Append(", ");
		}

		str.Append(" and it cost ");
		str.Append(manaCost);
		str.Append(" of mana.");

		return str.ToString();
	}
	public static BuffTokenAction Create(CardObject targetToken, CardObject summonedToken, List<SkillData> usedSkills, uint manaCost)
	{
		return new BuffTokenAction()
		{
			ownerPlayer = targetToken.Player,
			targetToken = targetToken,
			summonedToken = summonedToken,
			usedSkills = usedSkills,
			manaCost = manaCost,
		};
	}
}