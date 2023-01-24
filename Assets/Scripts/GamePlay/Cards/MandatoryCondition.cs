using System;
using UnityEngine;

[Serializable]
public class MandatoryCondition {
	public bool Completed => usedStoredValue >= targetQuantity;

	public MandatoryConditionType Type;
	public float targetQuantity;
	public float usedStoredValue;

	public void AddStoredValue(int value = 1)
    {
		usedStoredValue += value;
    }
	public void Setup(MandatoryConditionType type, float target){
		Type = type;
		targetQuantity = target;
	}
		
	public string GetDescription(){
		var remainingQuantity = Mathf.Abs(targetQuantity - usedStoredValue);

		switch (Type) {
		default:
			return "Error.";
		case MandatoryConditionType.DiscartCard:
			return "You have to discart " + remainingQuantity + " cards.";
		case MandatoryConditionType.DrawCard:
			return "You have to draw " + remainingQuantity + " cards.";
		case MandatoryConditionType.SendCardToManaPool:
			return "You have to send " + remainingQuantity + " cards to the mana pool.";
		case MandatoryConditionType.PickSpawnArea:
			return "You have to pick an spawn area for a recently summoned hero.";
		}
	}
}