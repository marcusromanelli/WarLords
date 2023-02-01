using System;
using System.Text;
using UnityEngine;

[Serializable]
public struct SkillData
{
	public int ManaCost;
	public Skill Data;
	public override string ToString()
	{
		return Data.ToString();
	}
}

[CreateAssetMenu(fileName = "Card Skill", menuName = "ScriptableObjects/Card/Skill", order = 2)]
public class Skill : ScriptableObject
{

	public int Level = 1;

	public Macro Macro;

    public override string ToString()
    {
		return (new StringBuilder(Macro.GetName(Level)).Append(" - ").Append(Macro.GetDescription(Level)).ToString());
    }

    /*[XmlIgnoreAttribute]
	public bool isActive;

	public int skillLevel;
	[XmlIgnoreAttribute]
	public TriggerType triggerType;
	public MacroType macroType;*/

    /*public void setDescription(Macro macro){
		this.name = macro.getName(skillLevel);
		this.description = macro.getDescription(skillLevel);
		this.triggerType = macro.triggerType;
	}*/
}
