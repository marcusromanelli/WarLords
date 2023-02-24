using NaughtyAttributes;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "ScriptableObjects/Card/Data", order = 2)]
public class Card : ScriptableObject
{
	public string Name;
	[ReadOnly] public string Id = Guid.NewGuid().ToString();
	public Sprite FrontCover;
	public uint ManaCost;
	public uint Attack;
	public uint Defense;
	public uint WalkSpeed = 1;
	[ReorderableList] public SkillData[] Skills;
	[Expandable, ReadOnly] public CivilizationGraphicsData Graphics;

	[Button]
	public void ResetId()
    {
		if(Id.Trim() != "")
			return;

		Id = Guid.NewGuid().ToString();
	}
}
