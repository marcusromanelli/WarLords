using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Battlefield : MonoBehaviour //this should be an class with no inheritance, but Unity refuses to serialize it, so fuck it
{
	[SerializeField] UIBattlefield uiBattlefield;

	private Dictionary<Player, List<HeroObject>> heroList = new Dictionary<Player, List<HeroObject>>();
	private GameController gameController;
	private Player localPlayer, remotePlayer;

	public void PreSetup(Player LocalPlayer, Player RemotePlayer, InputController InputController, GameController GameController, HandleCanSummonHero CanSummonHero)
	{
		gameController = GameController;

		localPlayer = LocalPlayer;
		remotePlayer = RemotePlayer;

		localPlayer.OnHoldCard += OnLocalPlayerHoldingCard;

		uiBattlefield.Setup(LocalPlayer, InputController, CanSummonHero);
	}

	#region UI_BATTLEFIELD_INTERFACE
	void OnLocalPlayerHoldingCard(Player player, CardObject cardObject)
	{
		uiBattlefield.OnLocalPlayerHoldCard(player, cardObject);
	}
	#endregion UI_BATTLEFIELD_INTERFACE

	#region MOVEMENT_PHASE
	public IEnumerator MovementPhase(Player currentPlayer)
	{
		yield return DoMovement(currentPlayer);
	}
	IEnumerator DoMovement(Player currentPlayer)
	{
		if (!heroList.ContainsKey(currentPlayer))
			yield break;

		List<HeroObject> heroes = heroList[currentPlayer];

		foreach (HeroObject hero in heroes)
		{
			yield return MoveHero(currentPlayer, hero);
		}
	}
	IEnumerator MoveHero(Player currentPlayer, HeroObject hero)
	{
		var tile = GetHeroTile(currentPlayer, hero);
		tile.SetHero(null);
		var newGridPosition = uiBattlefield.Normalize(CalculateHeroEndPosition(currentPlayer, hero));
		var newPosition = uiBattlefield.GridToUnity(newGridPosition);

		hero.Move(newPosition);

		yield return hero.IsWalking();

		var newTile = GetTileByPosition(newPosition);
		hero.SetPosition(newGridPosition);

		SetHeroTile(hero, newTile);
		EvaulateHeroTarget(currentPlayer, hero);
	}
	SpawnArea GetTileByPosition(Vector3 position)
	{
		return uiBattlefield.GetTileByPosition(position);
	}
	Vector2 CalculateHeroEndPosition(Player currentPlayer, HeroObject hero)
	{
		Vector2 gridPos = hero.GridPosition;
		var currentField = GetFields()[(int)gridPos.x, (int)gridPos.y];

		gridPos.y += GetHeroMovementDirection(currentPlayer) * hero.GetWalkSpeed();

		if (uiBattlefield.IsOnEnemyEdge(currentPlayer, currentField))
			return hero.GridPosition;

		var targetField = GetFields()[(int)gridPos.x, (int)gridPos.y];

		if (targetField.Hero != null)
			return hero.GridPosition;

		return gridPos;
	}
	void EvaulateHeroTarget(Player currentPlayer, HeroObject hero)
	{
		Vector2 gridPos = hero.GridPosition;
		var currentField = GetFields()[(int)gridPos.x, (int)gridPos.y];

		var targetPlayer = currentPlayer == localPlayer ? remotePlayer : localPlayer;

		if (uiBattlefield.IsOnEnemyEdge(currentPlayer, currentField))
		{
			hero.SetTarget(targetPlayer);
			return;
		}

		gridPos.y += GetHeroMovementDirection(currentPlayer) * hero.GetWalkSpeed();
		var nextField = GetFields()[(int)gridPos.x, (int)gridPos.y];

		if (nextField.Hero != null)
		{
			hero.SetTarget(nextField.Hero);
		}
		else
			hero.ResetTargets();
	}
	int GetHeroMovementDirection(Player player)
	{
		return player == localPlayer ? 1 : -1;
	}
	public SpawnArea GetHeroTile(Player player, HeroObject hero)
	{
		return uiBattlefield.GetHeroTile(player, hero);
	}
	#endregion MOVEMENT_PHASE

	#region ATTACK_PHASE
	public IEnumerator AttackPhase(Player currentPlayer, Player enemyPlayer)
	{
		yield return DoAttack(currentPlayer, enemyPlayer);
	}
	IEnumerator DoAttack(Player currentPlayer, Player enemyPlayer)
	{
		if (!heroList.ContainsKey(currentPlayer))
			yield break;

		List<HeroObject> heroes = heroList[currentPlayer];

		foreach (HeroObject hero in heroes)
			HeroAttack(hero, enemyPlayer);
	}
	void HeroAttack(HeroObject hero, Player enemyPlayer)
	{
		if (!hero.HasTarget())
			return;

		var target = hero.GetTarget();
		hero.Attack();

		var isAlive = TargetIsAlive(target, enemyPlayer);

		if (!isAlive)
			hero.ResetTargets();
	}
	bool TargetIsAlive(IAttackable target, Player ownerPlayer)
	{
		if (target.GetLife() > 0)
			return true;

		if (!(target is HeroObject))
			return true;

		var heroObject = target as HeroObject;

		DestroyHero(heroObject, ownerPlayer);

		return false;
	}
	#endregion ATTACK_PHASE

	#region HERO_LOGIC
	public void Summon(Player player, Card card, SpawnArea spawnArea = null)
	{
		var areaPosition = spawnArea ?? uiBattlefield.SelectedTile;

		var hero = HeroFactory.Create(card, transform, areaPosition.transform.position, Quaternion.identity);

		hero.SetPosition(uiBattlefield.UnityToGrid(areaPosition.transform.position));

		GameConfiguration.PlaySFX(GameConfiguration.Summon);

		//gameController.SetTriggerType(TriggerType.OnAfterSpawn, heroCard);

		AddHero(player, hero);

		ReorderHeroList(player);

		SetHeroTile(hero, areaPosition);
	}
	public bool PlayerHasHeroSummoned(Player player, Card card)
	{
		if (!heroList.ContainsKey(player))
			return false;

		return heroList[player].Any(c => c.GetId() == card.Id);
	}
	public bool CanSummonOnSelectedTile(Player player)
	{
		return CanSummonOnTile(player, uiBattlefield.SelectedTile);
	}
	public bool CanSummonOnTile(Player player, SpawnArea spawnArea)
	{
		if (spawnArea == null)
			return false;

		return uiBattlefield.CanPlayerSummonOnTile(player, spawnArea);
	}
	void AddHero(Player player, HeroObject hero)
	{
		if (!heroList.ContainsKey(player))
			heroList.Add(player, new List<HeroObject>());

		heroList[player].Add(hero);
	}
	void RemoveHero(Player player, HeroObject hero)
	{
		if (!heroList.ContainsKey(player))
			return;

		heroList[player].Remove(hero);
	}
	void ReorderHeroList(Player player)
	{
		var targetEdge = player == localPlayer ? uiBattlefield.GetRemotePlayerEdge() : uiBattlefield.GetLocalPlayerEdge();

		heroList[player].OrderByDescending(hero => Mathf.Abs(targetEdge - uiBattlefield.UnityToGrid(hero.transform.position).y));
	}
	void SetHeroTile(HeroObject hero, SpawnArea spawnArea)
	{
		if (spawnArea.Hero != null)
			Debug.LogError("[ERROR] TILE NOT EMPTY");

		spawnArea.SetHero(hero);
	}
	void DestroyHero(HeroObject heroObject, Player ownerPlayer)
    {
		var tile = uiBattlefield.GetHeroTile(ownerPlayer, heroObject);

		RemoveHero(ownerPlayer, heroObject);

		ownerPlayer.OnHeroDied(heroObject.OriginalCardData, tile);

		HeroFactory.AddToPool(heroObject);
	}
	#endregion HERO_LOGIC

	public SpawnArea[,] GetFields()
	{
		return uiBattlefield.GetFields();
	}
}
