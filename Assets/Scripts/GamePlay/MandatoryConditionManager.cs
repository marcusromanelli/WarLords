using System.Collections.Generic;
using UnityEngine;

public class MandatoryConditionManager : MonoBehaviour
{	
	public MandatoryConditionType CurrentType => Current != null ? Current.Type : MandatoryConditionType.None;
	public MandatoryCondition Current => conditions.Count > 0 ? conditions[0] : null;
	public List<MandatoryCondition> Conditions => conditions;

	[SerializeField] List<MandatoryCondition> conditions = new List<MandatoryCondition> ();

	public void Setup(Player player)
    {
		player.OnDrawCard += OnDrawCard;
		player.OnDiscardCard += OnDiscardCard;
		player.OnSendManaCreation += OnSendCardToManaPool;
		player.OnPickSpawnArea += OnPickSpawnArea;
    }
	public bool HasAny()
	{
		return conditions.Count > 0;
	}
	public bool Has(MandatoryConditionType condition)
	{
		return CurrentType == condition;
	}
	public void Remove(MandatoryConditionType conditionType)
	{
		if (!HasAny())
			return;

		var foundCondition = conditions.Find(condition => condition.Type == conditionType);

		if (foundCondition == null)
			return;

		conditions.Remove(foundCondition);
	}
	public void Remove(MandatoryCondition condition)
	{
		conditions.Remove(condition);
	}
	public void RemoveCurrent()
	{
		if (!HasAny())
			return;

		conditions.RemoveAt(0);
	}
	public void AddCondition(MandatoryConditionType conditionType, int conditionTarget)
    {
		var newCondition = new MandatoryCondition();
		newCondition.Setup(conditionType, conditionTarget);
		Conditions.Add(newCondition);
	}
	void CheckCompletion()
    {
		if (!Current.Completed)
			return;

		RemoveCurrent();
    }
	void OnSendCardToManaPool(int number)
    {
		if (CurrentType != MandatoryConditionType.SendCardToManaPool)
			return;

		Current.AddStoredValue(number);
		CheckCompletion();
	}
	void OnPickSpawnArea()
	{
		if (CurrentType != MandatoryConditionType.PickSpawnArea)
			return;

		Current.AddStoredValue();
		CheckCompletion();
	}
	void OnDrawCard(int number)
	{
		if (CurrentType != MandatoryConditionType.DrawCard)
			return;

		Current.AddStoredValue(number);
		CheckCompletion();
	}
	void OnDiscardCard(int number)
	{
		if (CurrentType != MandatoryConditionType.DiscartCard)
			return;

		Current.AddStoredValue(number);
		CheckCompletion();
	}
}
