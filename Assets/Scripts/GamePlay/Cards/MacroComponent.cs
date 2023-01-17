using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MacroComponent : MonoBehaviour
{
	Player Player;
	GameController GameController;
	Battlefield battlefield;
	public bool IsResolving { get; private set; }

	CardObject CardObject;


	Skill Skill;

	List<SpawnArea> spawnAreaAuxiliarList;
	List<Hero> heroAuxiliarList;

	int auxiliarInt, auxiliarInt2, auxiliarInt3, auxiliarInt4;

	void Awake()
	{
		IsResolving = false;
	}


	void Update()
	{
		if (!IsResolving)
			return;

		var currentPlayer = GameController.Singleton.currentPlayer;

		switch (Skill.macroType)
		{
			case MacroType.Invader:
				if (!currentPlayer.hasCondition(ConditionType.PickSpawnArea))
				{
					RemoveMacro();
				}
				break;
			case MacroType.BloodBrothers:
				//auxint = numero de personagens que o cara tem em campo
				//auxint2 = numero que a macro acha que tem em campo
				auxiliarInt = currentPlayer.getNumberOfHeroes();
				if (auxiliarInt > auxiliarInt2)
				{
					auxiliarInt2 = auxiliarInt;
					CardObject.Character.AddAttack(Skill.skillLevel);
					CardObject.Character.AddLife(Skill.skillLevel);
				}
				else if (auxiliarInt2 > auxiliarInt)
				{
					auxiliarInt2 = auxiliarInt;
				}
				break;
			case MacroType.Scourge:
				auxiliarInt = currentPlayer.getNumberOfHeroes() + GameController.GetOpponent(currentPlayer).getNumberOfHeroes();
				if (auxiliarInt > auxiliarInt2)
				{
					for (int i = 0; i < (auxiliarInt - auxiliarInt2); i++)
					{
						GameController.GetOpponent(CardObject.player).TakeDamage(Skill.skillLevel);
					}
					auxiliarInt2 = auxiliarInt;
				}
				else if (auxiliarInt2 > auxiliarInt)
				{
					auxiliarInt2 = auxiliarInt;
				}

				break;
			case MacroType.Science:
				auxiliarInt = CardObject.Character.card.Skills.Count;
				if (auxiliarInt > auxiliarInt2)
				{
					CardObject.player.DrawCard();
					auxiliarInt2 = auxiliarInt;
				}
				else if (auxiliarInt2 > auxiliarInt)
				{
					auxiliarInt2 = auxiliarInt;
				}
				break;
		}
	}

	void RemoveMacro()
	{
		switch (Skill.macroType)
		{
			case MacroType.Invader:
				spawnAreaAuxiliarList.ForEach(delegate (SpawnArea obj) {
					if (obj.player.GetPlayerType() == PlayerType.Remote)
					{
						obj.TemporarilySummonable = true;
					}
				});
				break;
		}
		Debug.LogWarning("Macro " + Skill.macroType.ToString() + " removed");
		GameController.RemoveMacro(this);
		Destroy(this);
	}
	public void setActive()
	{
		Initialize();
	}
	public void Setup(GameController gameController, Skill type, CardObject hero)
	{
		Skill = type;
		CardObject = hero;
		IsResolving = false;
		GameController = gameController;
		Player = hero.player;

		if (type.triggerType == TriggerType.Passive)
		{
			Initialize();
		}
	}

	public string getDescription()
	{
		return Skill.name + " - " + Skill.description;
	}
	public void Unenchant()
	{
		switch (Skill.macroType)
		{
			case MacroType.BloodBrothers:
				CardObject.Character.RemoveAttack(Skill.skillLevel);
				CardObject.Character.ResetLife();
				break;
			case MacroType.Exalt:
				foreach (SpawnArea aux in spawnAreaAuxiliarList)
				{
					if (aux != null && aux.GetComponent<Hero>())
					{
						aux.GetComponent<Hero>().RemoveAttack(Skill.skillLevel);
					}
				}
				break;
			default:
				Debug.Log(Skill.macroType.ToString() + " does not have an unenchant case");
				break;
		}
		RemoveMacro();
	}
	void Initialize()
	{
		IsResolving = true;
		spawnAreaAuxiliarList = new List<SpawnArea>();
		LogController.Log(Action.UseMacro, CardObject.player, Skill.macroType);

		var currentPlayer = GameController.Singleton.currentPlayer;

		switch (Skill.macroType)
		{
			case MacroType.Invader:
				List<SpawnArea> spawns = GameObject.FindObjectsOfType<SpawnArea>().ToList();
				spawns.ForEach(delegate (SpawnArea obj) {
					if (obj.player.GetPlayerType() == PlayerType.Remote)
					{
						obj.TemporarilySummonable = true;
						spawnAreaAuxiliarList.Add(obj);
					}
				});
				break;
			case MacroType.Lifelink:
				CardObject.player.AddLife(CardObject.Character.lastGivenDamage);
				RemoveMacro();
				break;
			case MacroType.Abundance:
				Debug.Log("Rodando Abundance");

				void AbundancePlayer(Player player)
				{
					player.DrawCard(Skill.skillLevel);
					player.AddMana(Skill.skillLevel);
				}

				AbundancePlayer(GameController.GetLocalPlayer());
				AbundancePlayer(GameController.GetRemotePlayer());

				RemoveMacro();
				break;
			case MacroType.Quicken:
				currentPlayer.AddMana(Skill.skillLevel);
				RemoveMacro();
				break;
			case MacroType.EnergyFlare:
				void EnergyFlarePlayer(Player player)
				{
					player.AddMana(Skill.skillLevel);
				}

				EnergyFlarePlayer(GameController.GetLocalPlayer());
				EnergyFlarePlayer(GameController.GetRemotePlayer());

				RemoveMacro();
				break;
			case MacroType.DirectDamage:
				GameController.GetOpponent(currentPlayer).TakeDamage(Skill.skillLevel);
				RemoveMacro();
				break;
			case MacroType.BloodBrothers:
				auxiliarInt = currentPlayer.getNumberOfHeroes();
				auxiliarInt2 = auxiliarInt;
				break;
			case MacroType.Excavate:
				currentPlayer.GetRandomCardFromGraveyard();
				RemoveMacro();
				break;
			case MacroType.Renew:
				CardObject.player.DrawCard();
				RemoveMacro();
				break;
			case MacroType.Dispel:
				Player en = GameController.GetOpponent(currentPlayer);
				var battleField = en.GetBattlefieldList();

				foreach (CardObject enemy in battleField)
				{
					enemy.Character.DisableSkills();
				}
				foreach (MacroComponent aux in GameController.GetMacrosFromPlayer(en))
				{
					aux.Unenchant();
				}
				RemoveMacro();
				break;
			case MacroType.Scourge:
				auxiliarInt = currentPlayer.getNumberOfHeroes() + GameController.GetOpponent(currentPlayer).getNumberOfHeroes() + 1;
				auxiliarInt2 = auxiliarInt;
				break;
			case MacroType.Speed:
				//if (!originalCard.Character.card.hasSkill (MacroType.Speed).isActive) {
				CardObject.Character.AddWalkSpeed(Skill.skillLevel);
				//}
				RemoveMacro();
				break;
			case MacroType.Sift:
				CardObject.player.GetRandomCardFromDeck(Skill.skillLevel);
				CardObject.player.AddCondition(ConditionType.DiscartCard, 5);
				RemoveMacro();
				break;
			case MacroType.TripleStrike:
				CardObject.Character.AddNumberOfAttacks(2);
				RemoveMacro();
				break;
			case MacroType.DoubleStrike:
				CardObject.Character.AddNumberOfAttacks(1);
				RemoveMacro();
				break;
			case MacroType.Exalt:
				Vector2 pos = CardObject.Character.gridPosition;
				for (int i = 0; i < battlefield.GetNumberOfSquares(); i++)
				{
					pos.x = i;
					List<Collider> colliders = Physics.OverlapSphere(battlefield.GridToUnity(pos), 0.3f, 1 << LayerMask.NameToLayer("Hero")).ToList();
					if (colliders.Count > 0)
					{
						var heroComponent = colliders[0].GetComponent<Hero>();

						heroAuxiliarList.Add(heroComponent);

						heroComponent.AddAttack(Skill.skillLevel);
					}
				}
				break;
			case MacroType.Wrath:
				List<Hero> derp = GameObject.FindObjectsOfType<Hero>().ToList();
				derp.Remove(CardObject.Character);
				int damage = derp.Count;
				foreach (Hero lol in derp)
				{
					lol.Die();
				}
				CardObject.player.TakeDamage(damage);
				RemoveMacro();
				break;
			case MacroType.Science:
				auxiliarInt = CardObject.Character.card.Skills.Count;
				auxiliarInt2 = auxiliarInt;
				break;
			case MacroType.Landmines:
				List<Hero> Enemies;
				Enemies = GameObject.FindObjectsOfType<Hero>().ToList();
				Enemies.RemoveAll(a => a.player.GetPlayerType() == CardObject.player.GetPlayerType());
				foreach (Hero hero in Enemies)
				{
					//Debug.Log ("Dono original da carta é "+((originalCard.player.isRemotePlayer)?"Remoto":"Local")+". Posição do heroi: "+Grid.UnityToGrid(hero.transform.position)+". Posição necessaria: "+((originalCard.player.isRemotePlayer)?"Y >= que "+(Grid.Singleton.numberOfSquares - Grid.Singleton.numberOfSpawnAreasPerLane):"Y < que "+Grid.Singleton.numberOfSpawnAreasPerLane));
					if ((battlefield.UnityToGrid(hero.transform.position).y < battlefield.GetNumberOfSpawnAreasPerLane() && CardObject.player.GetPlayerType() == PlayerType.Local)
						|| (battlefield.UnityToGrid(hero.transform.position).y >= (battlefield.GetNumberOfSquares() - battlefield.GetNumberOfSpawnAreasPerLane()) && CardObject.player.GetPlayerType() == PlayerType.Remote))
					{
						hero.doDamage(Skill.skillLevel);
					}
				}
				RemoveMacro();
				break;
			case MacroType.ExplosiveAttack:
				Vector2 pos2 = CardObject.Character.gridPosition;

				//Top
				pos2.y = CardObject.Character.gridPosition.y + 1;
				Collider[] auxL = Physics.OverlapSphere(battlefield.GridToUnity(pos2), 0.3f, 1 << LayerMask.NameToLayer("Hero"));
				if (auxL.Length > 0)
				{
					if (auxL[0].GetComponent<Hero>().player == CardObject.player)
					{
						auxL[0].GetComponent<Hero>().doDamage(CardObject.Character.lastGivenDamage);
					}
				}

				//Bot
				pos2.y = CardObject.Character.gridPosition.y - 1;
				auxL = Physics.OverlapSphere(battlefield.GridToUnity(pos2), 0.3f, 1 << LayerMask.NameToLayer("Hero"));
				if (auxL.Length > 0)
				{
					if (auxL[0].GetComponent<Hero>().player == CardObject.player)
					{
						auxL[0].GetComponent<Hero>().doDamage(CardObject.Character.lastGivenDamage);
					}
				}

				//Right
				pos2.y = CardObject.Character.gridPosition.y;
				pos2.x = CardObject.Character.gridPosition.x + 1;
				auxL = Physics.OverlapSphere(battlefield.GridToUnity(pos2), 0.3f, 1 << LayerMask.NameToLayer("Hero"));
				if (auxL.Length > 0)
				{
					if (auxL[0].GetComponent<Hero>().player == CardObject.player)
					{
						auxL[0].GetComponent<Hero>().doDamage(CardObject.Character.lastGivenDamage);
					}
				}

				//Left
				pos2.y = CardObject.Character.gridPosition.y;
				pos2.x = CardObject.Character.gridPosition.x + 1;
				auxL = Physics.OverlapSphere(battlefield.GridToUnity(pos2), 0.3f, 1 << LayerMask.NameToLayer("Hero"));
				if (auxL.Length > 0)
				{
					if (auxL[0].GetComponent<Hero>().player == CardObject.player)
					{
						auxL[0].GetComponent<Hero>().doDamage(CardObject.Character.lastGivenDamage);
					}
				}

				RemoveMacro();
				break;

			//Place holders
			case MacroType.Bombard:
				List<Hero> heroes = GameObject.FindObjectsOfType<Hero>().ToList();
				heroes.RemoveAll(a => a.player.GetCivilization() == CardObject.player.GetCivilization());
				if (heroes.Count > 0)
				{
					int index = Random.Range(0, heroes.Count - 1);
					heroes[index].doDamage(Skill.skillLevel);
				}
				RemoveMacro();
				break;


			case MacroType.Masochism:
				heroes = GameObject.FindObjectsOfType<Hero>().ToList();
				heroes.RemoveAll(a => a.player.GetCivilization() == CardObject.player.GetCivilization());
				if (heroes.Count > 0)
				{
					Hero hero = heroes[Random.Range(0, heroes.Count - 1)];
					hero.doDamage(hero.calculateAttackPower());
				}
				RemoveMacro();
				break;


			case MacroType.FriendlyAid:
				heroes = GameObject.FindObjectsOfType<Hero>().ToList();
				heroes.RemoveAll(a => a.player.GetCivilization() != CardObject.player.GetCivilization());
				if (heroes.Count > 0)
				{
					Hero hero = heroes[Random.Range(0, heroes.Count - 1)];
					hero.AddLife(Skill.skillLevel);
				}
				RemoveMacro();
				break;


			case MacroType.Waste:
				heroes = GameObject.FindObjectsOfType<Hero>().ToList();
				heroes.RemoveAll(a => a.player.GetCivilization() == CardObject.player.GetCivilization());
				if (heroes.Count > 0)
				{
					Hero hero = heroes[Random.Range(0, heroes.Count - 1)];
					hero.doDamage(Skill.skillLevel);
				}
				RemoveMacro();
				break;
		}
	}

	public Player GetPlayer()
    {
		return Player;
    }

	public CardObject GetCardObject()
	{
		return CardObject;
	}
	public Skill GetSkill()
	{
		return Skill;
	}
}