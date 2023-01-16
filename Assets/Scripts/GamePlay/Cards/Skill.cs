using UnityEngine;
using System;
using System.Collections;
using System.Xml.Serialization;


[Serializable]
public struct Skill{

	[XmlIgnoreAttribute]
	public string name;

	[XmlIgnoreAttribute]
	public string description;

	public int manaCost;

	[XmlIgnoreAttribute]
	public bool isActive;

	public int skillLevel;
	[XmlIgnoreAttribute]
	public TriggerType triggerType;
	public MacroType macroType;

	/*public void setDescription(Macro macro){
		this.name = macro.getName(skillLevel);
		this.description = macro.getDescription(skillLevel);
		this.triggerType = macro.triggerType;
	}*/
}
