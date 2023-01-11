using System;
using UnityEngine;

[Serializable]
public struct Macro{
	public string name;
	public string description;
	public MacroType macroType;
	public TriggerType triggerType;
	public TargetType targetType;

	/// <summary>
	/// Formats the name of the Macro by its level (Science X)
	/// </summary>
	/// <returns>The name formated.</returns>	
	/// <param name="level">Level.</param>
	public string getName(int level = 1){
		level = (level<=0)?1:level;		
		checkParameters (level);
		return (level > 1) ? name + " " + level.ToString () : name;
	}

	/// <summary>
	/// Formats de description of the macro by its level (%s = level)
	/// </summary>
	/// <returns>The description.</returns>
	/// <param name="level">Level.</param>
	public string getDescription(int level = 1){
		level = (level<=0)?1:level;		
		checkParameters (level);
		return (level > 0) ? description.Replace ("%s", level.ToString ()) : description;
	}

	/// <summary>
	/// Prevents null variables
	/// </summary>
	/// <param name="level">Level.</param>
	void checkParameters(int level){
		name = (name == null) ? "" : name;
		description = (description == null) ? "" : description;
	}
}
