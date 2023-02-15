using NaughtyAttributes;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "ScriptableObjects/Card/Data", order = 2)]
public class CardData : ScriptableObject
{
	[ReadOnly] public string Id = Guid.NewGuid().ToString();
    public uint ManaCost;
	public uint Attack;
	public uint Defense;
	public uint WalkSpeed = 1;
	[ReorderableList] public SkillData[] Skills;

	[Button]
	public void ResetId()
    {
		Id = Guid.NewGuid().ToString();
	}
}
