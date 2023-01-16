using UnityEngine;
using System.Collections;

public class Condition : MonoBehaviour {

	Player player;
	public ConditionType Type;
	public bool isChecking;
	public float targetQuantity;
	public float originalStoredValue;

	void Awake () {
		player = GetComponent<Player> ();
		isChecking = false;
	}
	

	void Update () {
		CheckConditions();
	}

	void CheckConditions()
	{
		if (!isChecking)
			return;

		var playerHandNumber = player.GetCurrentHandNumber();
		var playerManaPoolNumber = player.GetCurrentManaPoolCount();

		switch (Type)
		{
			case ConditionType.DiscartCard:
				if (playerHandNumber == originalStoredValue - targetQuantity)
				{
					player.removeCondition(this);
					Destroy(this);
				}
				break;
			case ConditionType.DrawCard:
				if (playerHandNumber == originalStoredValue + targetQuantity)
				{
					player.removeCondition(this);
					Destroy(this);
				}
				break;
			case ConditionType.SendCardToManaPool:
				if (/*playerHandNumber <= 0 || */playerManaPoolNumber == originalStoredValue + targetQuantity || playerManaPoolNumber >= GameConfiguration.maxNumberOfCardsInManaPool)
				{
					player.removeCondition(this);
					Destroy(this);
				}
				break;
			case ConditionType.PickSpawnArea:
				var playerEmptyBattleFieldNumber = player.GetEmptyBattleFieldNumber();

				if (playerEmptyBattleFieldNumber <= 0)
				{
					player.removeCondition(this);
					Destroy(this);
				}
				break;
		}
	}

	public void setActive(){
		Initialize ();
	}
	public void SetValues(ConditionType type, float target){
		Type = type;
		targetQuantity = target;
		isChecking = false;
	}
		
	public string getDescription(){
		var originalQuantityNumber = player.GetCurrentHandNumber() - originalStoredValue;

		switch (Type) {
		default:
			return "Error.";
		case ConditionType.DiscartCard:
			return "You have to discart " + Mathf.Abs(originalQuantityNumber)  + " cards.";
		case ConditionType.DrawCard:
			return "You have to draw " + targetQuantity + " cards.";
		case ConditionType.SendCardToManaPool:
			return "You have to send " + targetQuantity + " cards to the mana pool.";
		case ConditionType.PickSpawnArea:
			return "You have to pick an spawn area for a recently summoned hero.";
		}
	}

	void Initialize(){
		isChecking = true;
		switch (Type) {
		case ConditionType.DiscartCard:
		case ConditionType.DrawCard:
			originalStoredValue = player.GetCurrentHandNumber();
			break;
		case ConditionType.SendCardToManaPool:
			originalStoredValue = player.GetCurrentManaPoolCount();
			break;
		}
	}
}