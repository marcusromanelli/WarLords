using System.Collections.Generic;
using UnityEngine;

public class ConditionManager : MonoBehaviour
{	
	public ConditionType CurrentType => Current != null ? Current.Type : ConditionType.None;
	public Condition Current => conditions.Count > 0 ? conditions[0] : null;
	public List<Condition> Conditions => conditions;

	[SerializeField] List<Condition> conditions = new List<Condition> ();




	public bool HasAny()
	{
		return conditions.Count > 0;
	}
	public bool Has(ConditionType condition)
	{
		return CurrentType == condition;
	}
	public void Remove(ConditionType conditionType)
	{
		if (!HasAny())
			return;

		var foundCondition = conditions.Find(condition => condition.Type == conditionType);

		if (!foundCondition)
			return;

		conditions.Remove(foundCondition);
	}
	public void Remove(Condition condition)
	{
		conditions.Remove(condition);
	}
	public void RemoveCurrent()
	{
		if (!HasAny())
			return;

		conditions.RemoveAt(0);
	}
}
