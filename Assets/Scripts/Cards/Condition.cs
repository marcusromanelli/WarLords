using UnityEngine;
using System.Collections;

public class Condition : MonoBehaviour {

	Player player;
	public ConditionType Type;
	public bool isChecking;
	public float targetQuantity;
	public float originalQuantity;

	void Awake () {
		player = GetComponent<Player> ();
		isChecking = false;
	}
	

	void Update () {
		if (isChecking) {
			switch (Type) {
			case ConditionType.DiscartCard:
				if (player.Hand.Count == originalQuantity - targetQuantity) {
					player.removeCondition (this);
					Destroy (this);
				}
				break;
			case ConditionType.DrawCard:
				if (player.Hand.Count == originalQuantity + targetQuantity) {
					player.removeCondition (this);
					Destroy (this);
				}
				break;
			case ConditionType.SendCardToManaPool:
				if (player.Hand.Count<=0 || player.ManaPool.Count == originalQuantity + targetQuantity || player.ManaPool.Count >= GameConfiguration.maxNumberOfCardsInManaPool) {
					player.removeCondition (this);
					Destroy (this);
				}
				break;
			case ConditionType.PickSpawnArea:
				if (player.Battlefield.FindAll(a => a.Character == null).Count<=0) {
					player.removeCondition (this);
					Destroy (this);
				}
				break;
			}
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
		switch (Type) {
		default:
			return "Error.";
		case ConditionType.DiscartCard:
			return "You have to discart " + Mathf.Abs(((player.Hand.Count - originalQuantity)))  + " cards.";
		case ConditionType.DrawCard:
			return "You have to draw " + Mathf.Abs(((player.Hand.Count - originalQuantity) - targetQuantity))  + " cards.";
		case ConditionType.SendCardToManaPool:
			return "You have to send " +  Mathf.Abs(((player.ManaPool.Count - originalQuantity) - targetQuantity))  + " cards to the mana pool.";
		case ConditionType.PickSpawnArea:
			return "You have to pick an spawn area for a recently summoned hero.";
		}
	}

	void Initialize(){
		isChecking = true;
		switch (Type) {
		case ConditionType.DiscartCard:
			originalQuantity = player.Hand.Count;
			break;
		case ConditionType.DrawCard:
			originalQuantity = player.Hand.Count;
			break;
		case ConditionType.SendCardToManaPool:
			originalQuantity = player.ManaPool.Count;
			break;
		}
	}
}