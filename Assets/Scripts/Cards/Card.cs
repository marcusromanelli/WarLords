using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public struct Card{

	//[NonSerialized]
	[XmlIgnoreAttribute]
	public int CardID;
	[XmlIgnoreAttribute]
	public int PlayID;
	public string name;
	public int manaCost;
	public int attack;
	public int life;

	[XmlIgnoreAttribute]
	public ManaStatus manaStatus;

	public Civilization civilization;
	public List<Skill> Skills;


	public Card(int CardID){
		Card aux = CardCollection.FindCardByID (CardID);
		this.CardID = aux.CardID;
		this.PlayID = -1;
		this.name = aux.name;
		this.manaCost = aux.manaCost;
		this.attack = aux.attack;
		this.life = aux.life;
		this.Skills = aux.Skills;
		this.manaStatus = ManaStatus.Used;
		this.civilization = aux.civilization;

		this.updateData ();
	}

	public void updateData(){
		for(int i=0;i<Skills.Count;i++){
			Macro aux = CardsLibrary.Singleton.Macros.findMacro(Skills[i].macroType);
			Skill aux2 = Skills[i];
			aux2.name = CardsLibrary.Singleton.Macros.findMacro(Skills[i].macroType).name;
			if(aux.description==null){
				aux2.description = "Macro: "+Skills[i].macroType.ToString()+" with no description."; 
				Debug.LogError("Macro: "+Skills[i].macroType.ToString()+" with no description.");
			}else{
				aux2.description = CardsLibrary.Singleton.Macros.findMacro(Skills[i].macroType).description.Replace("%s", aux2.skillLevel.ToString());
			}
			aux2.triggerType = CardsLibrary.Singleton.Macros.findMacro(Skills[i].macroType).triggerType;

			Skills[i] = aux2;
		}
	}
	public void setID(int id){
		this.CardID = id;
	}

	public List<Skill> hasSkillType(TriggerType type){
		List<Skill> list = new List<Skill>(Skills);

		list.RemoveAll(aux => ((aux.isActive && aux.triggerType != type) || !aux.isActive));

		return list;
	}

	public Skill hasSkill(MacroType type){
		foreach (Skill aux in Skills) {
			if (aux.macroType == type && aux.isActive) {
				return aux;
			}
		}

		return new Skill ();
	}

	public void AddSkill(Skill skill){
		skill.isActive = true;
		Skills.Add (skill);
		updateData();
	}

	public int calculateCost(){
		int cost = manaCost;
		foreach(Skill skill in Skills){ 
			if (skill.isActive) {
				cost += skill.manaCost;
			}
		}
		return cost;
	}

	public static bool operator ==(Card a, Card b){
		return (((Card)a).PlayID == ((Card)b).PlayID);
	}

	public static bool operator !=(Card a, Card b){
		return (((Card)a).PlayID != ((Card)b).PlayID);
	}

	public override bool Equals (object obj)
	{
		return ((obj.GetType() == typeof(Card)) && (((Card)obj).PlayID == this.PlayID));
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public bool Initialized()
    {
		return CardID > 0;

	}
}
