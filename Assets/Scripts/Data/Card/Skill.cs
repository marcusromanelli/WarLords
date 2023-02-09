using System;
using System.Text;
using UnityEngine;

[Serializable]
public struct SkillData
{
	[SerializeField] uint ManaCost;
    [SerializeField] Skill Data;

	public override string ToString()
	{
		return Data.ToString();
	}
    public uint GetManaCost() => ManaCost;

    public static bool operator ==(SkillData c1, SkillData c2)
    {
        return c1.Equals(c2);
    }    
    public static bool operator !=(SkillData c1, SkillData c2)
    {
        return !c1.Equals(c2);
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj) && Data == ((SkillData)obj).Data;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[CreateAssetMenu(fileName = "Card Skill", menuName = "ScriptableObjects/Card/Skill", order = 2)]
public class Skill : ScriptableObject
{
	public uint Level = 1;

	public Macro Macro;

    public override string ToString()
    {
		return (new StringBuilder(Macro.GetName(Level)).Append(" - ").Append(Macro.GetDescription(Level)).ToString());
    }
}