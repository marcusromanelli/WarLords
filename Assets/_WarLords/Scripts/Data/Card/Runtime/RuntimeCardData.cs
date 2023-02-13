using System;
using System.Collections.Generic;

[Serializable]
public class RuntimeCardData
{
	private string name;
	public string Name => name;

	private uint manaCost;
	public uint ManaCost => manaCost;

	private uint attack;
	public uint Attack => attack;

	private uint defense;
	public uint Defense => defense;

	private uint walkSpeed;
	public uint WalkSpeed => walkSpeed;

	private Dictionary<SkillData, bool> activeSkills;

	private SkillData[] skills;
	public SkillData[] Skills => skills;

	public RuntimeCardData(Card card)
    {
		name = card.Name;
		manaCost = card.Data.ManaCost;
		attack = card.Data.Attack;
		defense = card.Data.Defense;
		walkSpeed = card.Data.WalkSpeed;

		activeSkills = new Dictionary<SkillData, bool>();

		skills = card.Data.Skills;

		foreach (var skill in skills)
			activeSkills[skill] = false;
    }
	public void UnselectAllSkills()
    {
		foreach(var skill in Skills)
        {
			activeSkills[skill] = false;
        }
    }
	public void ToggleSkill(SkillData skillData, bool enabled)
    {
		if (!activeSkills.ContainsKey(skillData))
			return;

		activeSkills[skillData] = enabled;
	}
	public uint CalculateSummonCost()
	{
		uint skillCost = 0;

		foreach (SkillData skill in Skills)
		{
			if (activeSkills[skill])
				skillCost += skill.GetManaCost();
		}

		return ManaCost + skillCost;
	}
	public uint CalculateAttack()
	{
		return Attack;
	}
	public uint CalculateDefense()
	{
		return Defense;
	}
	public uint CalculateWalkSpeed()
	{
		return WalkSpeed;
	}
}
