using UnityEngine;
using System;
using NaughtyAttributes;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card/Main Card", order = 1)]
public class Card : ScriptableObject
{
	public string Id => Data.Id;

	public string Name;
	public Sprite FrontCover;
	[Expandable] public CivilizationData Civilization;
	[Expandable] public CardData Data;

	/*public void UpdateData()
	{
		for (int i = 0; i < Skills.Count; i++)
		{
			Macro aux = CardsLibrary.Singleton.Macros.findMacro(Skills[i].macroType);
			Skill aux2 = Skills[i];
	calc
	aux2.name = CardsLibrary.Singleton.Macros.findMacro(Skills[i].macroType).name;
			if (aux.description == null)
			{
				aux2.description = "Macro: " + Skills[i].macroType.ToString() + " with no description.";
				Debug.LogError("Macro: " + Skills[i].macroType.ToString() + " with no description.");
			}
			else
			{
				aux2.description = CardsLibrary.Singleton.Macros.findMacro(Skills[i].macroType).description.Replace("%s", aux2.skillLevel.ToString());
			}
			aux2.triggerType = CardsLibrary.Singleton.Macros.findMacro(Skills[i].macroType).triggerType;

			Skills[i] = aux2;
		}
	}
	public void setID(int id)
	{
		this.CardID = id;
	}

	public List<Skill> hasSkillType(TriggerType type)
	{
		List<Skill> list = new List<Skill>(Skills);

		list.RemoveAll(aux => ((aux.isActive && aux.triggerType != type) || !aux.isActive));

		return list;
	}

	public Skill hasSkill(MacroType type)
	{
		foreach (Skill aux in Skills)
		{
			if (aux.macroType == type && aux.isActive)
			{
				return aux;
			}
		}

		return new Skill();
	}

	public void AddSkill(Skill skill)
	{
		skill.isActive = true;
		Skills.Add(skill);
		updateData();
	}

	public bool Initialized()
	{
		return CardID > 0;
	}
	*/

	/*public static bool operator ==(Card a, Card b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Card a, Card b)
	{
		return a.Equals(b);
	}

	public override bool Equals(object obj)
	{
		return ((obj.GetType() == typeof(Card)) && (((Card)obj) == this));
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}*/
}
