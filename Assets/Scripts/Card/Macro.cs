using UnityEngine;

[CreateAssetMenu(fileName = "Card Macro", menuName = "ScriptableObjects/Card/Macro", order = 2)]
public class Macro : ScriptableObject
{
	public string Name;
	public string Description;

	public TriggerType TriggerType;
	public TargetType TargetType;

	/// <summary>
	/// Formats the name of the Macro by its level (Science X)
	/// </summary>
	/// <returns>The name formated.</returns>	
	/// <param name="level">Level.</param>
	public string GetName(int level = 1)
	{
		level = Mathf.Clamp(level, 1, level);

		CheckParameters();

		return (level > 1) ? name + " " + level.ToString() : name;
	}

	/// <summary>
	/// Formats de description of the macro by its level (%s = level)
	/// </summary>
	/// <returns>The description.</returns>
	/// <param name="level">Level.</param>
	public string GetDescription(int level = 1)
	{
		level = Mathf.Clamp(level, 1, level);

		CheckParameters();

		return (level > 0) ? Description.Replace("%s", level.ToString()) : Description;
	}

	/// <summary>
	/// Prevents null variables
	/// </summary>
	void CheckParameters()
	{
		Name = Sanitize(Name);
		Description = Sanitize(Description);
	}

	string Sanitize(string text)
    {
		return (text == null) ? "" : text;
	}
}
