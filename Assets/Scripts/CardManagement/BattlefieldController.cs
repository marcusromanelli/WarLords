using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BattlefieldController : PlaceableCard {

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
		if (waitingForSpawnPoint) {
			layerMask = 1 << 13;
			results = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition), 1000, layerMask);
			List<RaycastHit> result = results.ToList ().FindAll (a => (a.collider.GetComponent<SpawnArea> () != null && (a.collider.GetComponent<SpawnArea>().LocalPlayer || (!a.collider.GetComponent<SpawnArea>().LocalPlayer && a.collider.GetComponent<SpawnArea>().canBeUsedToSpawn))));

			if (result.Count>0 && Input.GetMouseButtonDown(0)){
				if(Physics.CheckSphere(result[0].transform.position, 0.3f, 1 << LayerMask.NameToLayer("Hero"))){
					GameConfiguration.PlaySFX(GameConfiguration.denyAction);
				}else{
					cardWaitingForSpawn.setCharacterSpawnArea(result[0].transform.position);
					waitingForSpawnPoint = false;
					cardWaitingForSpawn = null;
				}
			}
		}
	}



	float calculateZ(int number){
		return (number % 4);
	}

	public void Summon(CardObject hero){
		hero.transform.SetParent(transform);
		player.AddCondition (ConditionType.PickSpawnArea);
		GameController.SetTriggerType (TriggerType.OnBeforeSpawn, hero);
		waitingForSpawnPoint = true;
		cardWaitingForSpawn = hero;
	}

	public void Summon(CardObject hero, Vector3 position){
		hero.transform.SetParent(transform);
		hero.setCharacterSpawnArea(position);
	}

	public void Kill(CardObject card){
		Hero hero = card.Character;
		Destroy (hero.gameObject);
		GameController.SetTriggerType(TriggerType.OnAfterDeath, card);
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
