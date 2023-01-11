using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MacroComponent : MonoBehaviour {

	Player currentPlayer;
	public CardObject originalCard;


	public Skill Skill;
	public bool isChecking;

	List<GameObject> auxiliarList;
	int auxiliarInt, auxiliarInt2, auxiliarInt3, auxiliarInt4;

	void Awake () {
		isChecking = false;
		currentPlayer = GameController.Singleton.getCurrentPlayer ();
	}
	

	void Update () {
		if (isChecking) {
			switch (Skill.macroType) {
			case MacroType.Invader:
				if (!currentPlayer.hasCondition (ConditionType.PickSpawnArea)) {
					RemoveMacro ();
				}
				break;
			case MacroType.BloodBrothers:
				//auxint = numero de personagens que o cara tem em campo
				//auxint2 = numero que a macro acha que tem em campo
				auxiliarInt = currentPlayer.getNumberOfHeroes ();
				if (auxiliarInt > auxiliarInt2) {
					auxiliarInt2 = auxiliarInt;
					originalCard.Character.AddAttack (Skill.skillLevel);
					originalCard.Character.AddLife (Skill.skillLevel);
				} else if (auxiliarInt2 > auxiliarInt) {
					auxiliarInt2 = auxiliarInt;
				}
				break;
			case MacroType.Scourge:
				auxiliarInt = currentPlayer.getNumberOfHeroes () + GameController.getOpponent(currentPlayer).getNumberOfHeroes();
				if (auxiliarInt > auxiliarInt2) {
					for (int i = 0; i < (auxiliarInt - auxiliarInt2); i++) {
						GameController.getOpponent (originalCard.player).doDamage (Skill.skillLevel);	
					}
					auxiliarInt2 = auxiliarInt;
				} else if (auxiliarInt2 > auxiliarInt) {
					auxiliarInt2 = auxiliarInt;
				}

				break;
			case MacroType.Science:
				auxiliarInt = originalCard.Character.card.Skills.Count;
				if (auxiliarInt > auxiliarInt2) {
					originalCard.player.DrawCard ();
					auxiliarInt2 = auxiliarInt;
				} else if (auxiliarInt2 > auxiliarInt) {
					auxiliarInt2 = auxiliarInt;
				}
				break;
			}
		}
		//if (originalCard == null || originalCard.Character == null) {
		//	RemoveMacro();
		//}
	}

	void RemoveMacro(){
		switch (Skill.macroType) {
		case MacroType.Invader:
			auxiliarList.ForEach (delegate(GameObject obj) {
				if(obj.GetComponent<SpawnArea>().LocalPlayer==false){
					obj.GetComponent<SpawnArea>().canBeUsedToSpawn = true;
				}
			});
			break;
		}
		Debug.LogWarning("Macro "+Skill.macroType.ToString()+" removed");
		GameController.RemoveMacro(this);
		Destroy (this);
	}
	public void setActive(){
		Initialize ();
	}
	public void SetValues(Skill type, CardObject hero){
		Skill = type;
		originalCard = hero;
		isChecking = false;
		if (type.triggerType == TriggerType.Passive) {
			Initialize ();
		}
	}
		
	public string getDescription(){
		return Skill.name+" - "+Skill.description;
	}
	public void Unenchant(){
		switch (Skill.macroType) {
		case MacroType.BloodBrothers:
			originalCard.Character.RemoveAttack (Skill.skillLevel);
			originalCard.Character.ResetLife ();
			break;
		case MacroType.Exalt:
			foreach (GameObject aux in auxiliarList) {
				if (aux != null && aux.GetComponent<Hero> ()) {
					aux.GetComponent<Hero> ().RemoveAttack (Skill.skillLevel);
				}
			}
			break;
		default:
			Debug.Log (Skill.macroType.ToString () + " does not have an unenchant case");	
			break;
		}
		RemoveMacro ();
	}
	void Initialize(){
		isChecking = true;
		auxiliarList = new List<GameObject> ();
		LogController.Log (Action.UseMacro, originalCard.player, Skill.macroType);

		switch (Skill.macroType) {
		case MacroType.Invader:
			List<SpawnArea> spawns = GameObject.FindObjectsOfType<SpawnArea> ().ToList ();
			spawns.ForEach (delegate(SpawnArea obj) {
				if(obj.LocalPlayer == false){
					//obj.LocalPlayer = true;
					obj.canBeUsedToSpawn = true;
					auxiliarList.Add(obj.gameObject);	
				}
			});
			break;
		case MacroType.Lifelink:
			originalCard.player.AddLife (originalCard.Character.lastGivenDamage);
			RemoveMacro ();
			break;
		case MacroType.Abundance:
			Debug.Log ("Rodando Abundance");
			foreach (Player aux in GameController.Singleton.Players) {
				aux.DrawCard (Skill.skillLevel);
				aux.AddMana (Skill.skillLevel);
			}
			RemoveMacro ();
			break;
		case MacroType.Quicken:
			currentPlayer.AddMana (Skill.skillLevel);
			RemoveMacro ();
			break;
		case MacroType.EnergyFlare:
			foreach (Player aux in GameController.Singleton.Players) {
				aux.AddMana (Skill.skillLevel);
			}
			RemoveMacro ();
			break;
		case MacroType.DirectDamage:
			GameController.getOpponent (currentPlayer).doDamage (Skill.skillLevel);
			RemoveMacro ();
			break;
		case MacroType.BloodBrothers:
			auxiliarInt = currentPlayer.getNumberOfHeroes ();
			auxiliarInt2 = auxiliarInt;
			break;
		case MacroType.Excavate:
			currentPlayer.GetRandomCardFromGraveyard ();
			RemoveMacro ();
			break;
		case MacroType.Renew:
			originalCard.player.DrawCard ();
			RemoveMacro ();
			break;
		case MacroType.Dispel:
			Player en = GameController.getOpponent(currentPlayer);
			foreach(CardObject enemy in en.Battlefield){
				enemy.Character.DisableSkills();
			}
			foreach(MacroComponent aux in GameController.GetMacrosFromPlayer(en)){
				aux.Unenchant();
			}
			RemoveMacro ();
			break;
		case MacroType.Scourge:
			auxiliarInt = currentPlayer.getNumberOfHeroes () + GameController.getOpponent (currentPlayer).getNumberOfHeroes ()+1;
			auxiliarInt2 = auxiliarInt;
			break;
		case MacroType.Speed:
			//if (!originalCard.Character.card.hasSkill (MacroType.Speed).isActive) {
				originalCard.Character.AddWalkSpeed (Skill.skillLevel);
			//}
			RemoveMacro ();
			break;
		case MacroType.Sift:
			originalCard.player.GetRandomCardFromDeck (Skill.skillLevel);
			originalCard.player.addCondition (ConditionType.DiscartCard, 5);
			RemoveMacro ();
			break;
		case MacroType.TripleStrike:
			originalCard.Character.AddNumberOfAttacks (2);
			RemoveMacro ();
			break;
		case MacroType.DoubleStrike:
			originalCard.Character.AddNumberOfAttacks (1);
			RemoveMacro ();
			break;
		case MacroType.Exalt:
			Vector2 pos = originalCard.Character.gridPosition;
			for(int i = 0; i<Grid.Singleton.numberOfSquares;i++){
				pos.x = i;
				List<Collider> aux2 = Physics.OverlapSphere (Grid.GridToUnity (pos), 0.3f, 1 << LayerMask.NameToLayer("Hero")).ToList ();
				if(aux2.Count>0){
					auxiliarList.Add (aux2 [0].gameObject);
					aux2[0].GetComponent<Hero>().AddAttack(Skill.skillLevel);
				}
			}
			break;
		case MacroType.Wrath:
			List<Hero> derp = GameObject.FindObjectsOfType<Hero> ().ToList ();
			derp.Remove (originalCard.Character);
			int damage = derp.Count;
			foreach (Hero lol in derp) {
				lol.Die ();
			}
			originalCard.player.doDamage (damage);
			RemoveMacro ();
			break;
		case MacroType.Science:
			auxiliarInt = originalCard.Character.card.Skills.Count;
			auxiliarInt2 = auxiliarInt;
			break;
		case MacroType.Landmines:
			List<Hero> Enemies;
			Enemies = GameObject.FindObjectsOfType<Hero> ().ToList ();
			Enemies.RemoveAll (a => a.player.isRemotePlayer == originalCard.player.isRemotePlayer);
			foreach (Hero hero in Enemies) {
				//Debug.Log ("Dono original da carta é "+((originalCard.player.isRemotePlayer)?"Remoto":"Local")+". Posição do heroi: "+Grid.UnityToGrid(hero.transform.position)+". Posição necessaria: "+((originalCard.player.isRemotePlayer)?"Y >= que "+(Grid.Singleton.numberOfSquares - Grid.Singleton.numberOfSpawnAreasPerLane):"Y < que "+Grid.Singleton.numberOfSpawnAreasPerLane));
				if ((Grid.UnityToGrid (hero.transform.position).y < Grid.Singleton.numberOfSpawnAreasPerLane && !originalCard.player.isRemotePlayer) 
					|| (Grid.UnityToGrid (hero.transform.position).y >= (Grid.Singleton.numberOfSquares - Grid.Singleton.numberOfSpawnAreasPerLane) && originalCard.player.isRemotePlayer)){
					hero.doDamage (Skill.skillLevel);
				}
			}
			RemoveMacro();
			break;
		case MacroType.ExplosiveAttack:
			Vector2 pos2 = originalCard.Character.gridPosition;

			//Top
			pos2.y = originalCard.Character.gridPosition.y + 1;
			Collider[] auxL = Physics.OverlapSphere (Grid.GridToUnity (pos2), 0.3f, 1 << LayerMask.NameToLayer("Hero"));
			if (auxL.Length > 0) {
				if(auxL[0].GetComponent<Hero>().player == originalCard.player){
					auxL [0].GetComponent<Hero> ().doDamage (originalCard.Character.lastGivenDamage);
				}
			}

			//Bot
			pos2.y = originalCard.Character.gridPosition.y - 1;
			auxL = Physics.OverlapSphere (Grid.GridToUnity (pos2), 0.3f, 1 << LayerMask.NameToLayer("Hero"));
			if (auxL.Length > 0) {
				if(auxL[0].GetComponent<Hero>().player == originalCard.player){
					auxL [0].GetComponent<Hero> ().doDamage (originalCard.Character.lastGivenDamage);
				}
			}

			//Right
			pos2.y = originalCard.Character.gridPosition.y;
			pos2.x = originalCard.Character.gridPosition.x + 1;
			auxL = Physics.OverlapSphere (Grid.GridToUnity (pos2), 0.3f, 1 << LayerMask.NameToLayer("Hero"));
			if (auxL.Length > 0) {
				if(auxL[0].GetComponent<Hero>().player == originalCard.player){
					auxL [0].GetComponent<Hero> ().doDamage (originalCard.Character.lastGivenDamage);
				}
			}

			//Left
			pos2.y = originalCard.Character.gridPosition.y;
			pos2.x = originalCard.Character.gridPosition.x + 1;
			auxL = Physics.OverlapSphere (Grid.GridToUnity (pos2), 0.3f, 1 << LayerMask.NameToLayer("Hero"));
			if (auxL.Length > 0) {
				if(auxL[0].GetComponent<Hero>().player == originalCard.player){
					auxL [0].GetComponent<Hero> ().doDamage (originalCard.Character.lastGivenDamage);
				}
			}

			RemoveMacro ();
			break;

		//Place holders
		case MacroType.Bombard:
			List<Hero> heroes = GameObject.FindObjectsOfType<Hero> ().ToList ();
			heroes.RemoveAll (a => a.player.civilization == originalCard.player.civilization);
			if (heroes.Count > 0) {
				int index = Random.Range (0, heroes.Count - 1);
				heroes [index].doDamage (Skill.skillLevel);
			}
			RemoveMacro();
			break;


		case MacroType.Masochism:
			heroes = GameObject.FindObjectsOfType<Hero> ().ToList ();
			heroes.RemoveAll (a => a.player.civilization == originalCard.player.civilization);
			if (heroes.Count > 0) {
				Hero hero = heroes [Random.Range (0, heroes.Count - 1)];
				hero.doDamage(hero.calculateAttackPower());
			}
			RemoveMacro();
			break;


		case MacroType.FriendlyAid:
			heroes = GameObject.FindObjectsOfType<Hero> ().ToList ();
			heroes.RemoveAll (a => a.player.civilization != originalCard.player.civilization);
			if (heroes.Count > 0) {
				Hero hero = heroes [Random.Range (0, heroes.Count - 1)];
				hero.AddLife (Skill.skillLevel);
			}
			RemoveMacro();
			break;


		case MacroType.Waste:
			heroes = GameObject.FindObjectsOfType<Hero> ().ToList ();
			heroes.RemoveAll (a => a.player.civilization == originalCard.player.civilization);
			if (heroes.Count > 0) {
				Hero hero = heroes [Random.Range (0, heroes.Count - 1)];
				hero.doDamage (Skill.skillLevel);
			}
			RemoveMacro();
			break;
		}
	}
}