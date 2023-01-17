using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BattlefieldController : PlaceableCard 
{
	[SerializeField] Battlefield battlefield;
	[SerializeField] GameController gameController;
	CardObject cardWaitingForSpawn;
	Player player;
	RaycastHit[] results;
	RaycastHit[] heroes;

	public bool waitingForSpawnPoint;
	int layerMask;


	void Start () {
		player = GetComponentInParent<Player> ();
	}
		
	void Update () {
		CheckEmptySpawnArea();
	}


	void CheckEmptySpawnArea()
    {
		var currentTile = battlefield.GetSelectedTile();

		if (!waitingForSpawnPoint || currentTile == null || !Input.GetMouseButton(0))
			return;

		if (currentTile.Hero != null)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			return;
		}

		cardWaitingForSpawn.setCharacterSpawnArea(currentTile);
		waitingForSpawnPoint = false;
		cardWaitingForSpawn = null;
	}


	float calculateZ(int number){
		return (number % 4);
	}

	public void Summon(CardObject hero){
		hero.transform.SetParent(transform);
		player.AddCondition (ConditionType.PickSpawnArea);
		gameController.SetTriggerType (TriggerType.OnBeforeSpawn, hero);
		waitingForSpawnPoint = true;
		cardWaitingForSpawn = hero;
	}

	public void Summon(CardObject hero, SpawnArea area){
		hero.transform.SetParent(transform);
		hero.setCharacterSpawnArea(area);
	}

	public void Kill(CardObject card){
		Hero hero = card.Character;
		Destroy (hero.gameObject);
		gameController.SetTriggerType(TriggerType.OnAfterDeath, card);
		Destroy (card.gameObject);
	}
		
	public Vector3 getTopPosition(){
		return transform.position;
	}

	public Quaternion getTopRotation(){
		return Quaternion.Euler (90, 0, 0);
	}

	public Vector3 getTopScale(){
		return Vector3.one;
	}
}
