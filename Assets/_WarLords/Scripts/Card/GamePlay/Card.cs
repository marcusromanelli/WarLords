using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardComparer : IEqualityComparer<Card>
{
	public bool Equals(Card x, Card y)
	{
		return x.GetHashCode() == y.GetHashCode();
	}

	public int GetHashCode(Card obj)
	{
		return obj.GetHashCode();
	}
}

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

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
