using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

	private CivilizationGraphicsData graphics;
	public CivilizationGraphicsData Graphics => graphics;

	private Sprite frontCover;
	public Sprite FrontCover => frontCover;

	public List<SkillData> ActiveSkills => activeSkills;
	public SkillData[] OriginalCardSkills => originalSkills;

	private SkillData[] originalSkills;
	private List<SkillData> activeSkills;

	public RuntimeCardData(Card card)
    {
		name = card.Name;
		manaCost = card.ManaCost;
		attack = card.Attack;
		defense = card.Defense;
		walkSpeed = card.WalkSpeed;
		graphics = card.Graphics;
		frontCover = card.FrontCover;

		activeSkills = new List<SkillData>();

		originalSkills = card.Skills;
    }
	public void UnselectAllSkills()
    {
		activeSkills.Clear();
    }
	public void ToggleSkill(SkillData skillData, bool enabled)
    {
        if (!enabled)
		{
			activeSkills.Remove(skillData);
			return;
		}

		if (activeSkills.Contains(skillData))
			return;

		activeSkills.Add(skillData);
	}
	public uint CalculateSummonCost(bool isSkillOnly)
	{
		uint skillCost = 0;

		foreach (SkillData skill in activeSkills)
		{
			skillCost += skill.GetManaCost();
		}

		if (isSkillOnly)
			return skillCost;

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
	public void BuffSkills(RuntimeCardData newRuntimeCardData)
    {
		foreach(var skill in newRuntimeCardData.ActiveSkills)
        {
            if (activeSkills.Contains(skill)){
				Debug.Log("Token already contains an active skill " + skill.ToString() + ", ignoring.");

				continue;
            }

			activeSkills.Add(skill);
		}
		
    }
}