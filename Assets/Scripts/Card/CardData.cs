using NaughtyAttributes;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "ScriptableObjects/Card/Data", order = 2)]
public class CardData : ScriptableObject
{
	[ReadOnly] public string Id = Guid.NewGuid().ToString();
    public int ManaCost;
	public int Attack;
	public int Defense;
	[ReorderableList] public SkillData[] Skills;

	[Button]
	public void ResetId()
    {
		Id = Guid.NewGuid().ToString();
	}
	public int SummonCost()
    {
		int cost = 0;
		/*foreach (Skill skill in Skills)
		{
			if (skill.isActive)
				cost += skill.manaCost;
		}*/

		return cost;
	}
}
