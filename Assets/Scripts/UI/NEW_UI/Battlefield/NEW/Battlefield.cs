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

	Dictionary<Player, List<Hero>> heroList = new Dictionary<Player, List<Hero>>();
	GameController gameController;

	public void Setup(InputController InputController, GameController GameController, HandleCanSummonHero CanSummonHero)
	{
		gameController = GameController;

		GameController.LocalPlayer.OnHoldCard += OnLocalPlayerHoldingCard;

		uiBattlefield.Setup(InputController, CanSummonHero);
	}

	#region UI_BATTLEFIELD_INTERFACE
	void OnLocalPlayerHoldingCard(CardObject cardObject)
    {
		uiBattlefield.OnLocalPlayerHoldCard(cardObject);
	}
	#endregion UI_BATTLEFIELD_INTERFACE

	#region MOVEMENT_PHASE
	public IEnumerator MovementPhase()
	{
		yield return DoMovement();
	}
	IEnumerator DoMovement()
	{
		//gameController.DisablePlayers();

		/*var currentPlayer = gameController.GetCurrentPlayer();

		List<Hero> heroes = heroList[currentPlayer];

		foreach (Hero hero in heroes)
		{
			var tile = GetHeroTile(currentPlayer, hero);
			tile.SetHero(null);

			hero.moveForward();
			while (hero.IsWalking())
			{
				yield return null;
			}


			var newTile = GetTileByPosition(hero.GridPosition);
			SetHeroTile(hero, newTile);

			yield return new WaitForSeconds(1f);
		}

		gameController.EnablePlayers();*/

		yield return new WaitForSeconds(1);
	}
	#endregion MOVEMENT_PHASE

	#region ATTACK_PHASE
	public async Task AttackPhase()
	{
		/*hasFinishedAttack = false;

		//StartCoroutine(DoMovement());

		while (!hasFinishedAttack)
		{
			await Task.Delay(25);
		};*/
	}
	#endregion ATTACK_PHASE

	#region HERO_LOGIC
	public List<Hero> GetHeroes(Player player)
	{
		return heroList[player];
	}
	public bool PlayerHasHeroSummoned(Player player, Card card)
	{
		return false;

		if (!heroList.ContainsKey(player))
			return false;

		return heroList[player].Any(c => c.CardObject.Data.CardID == card.CardID);
	}
	void AddHero(Player player, Hero hero)
	{
		if (!heroList.ContainsKey(player))
			heroList.Add(player, new List<Hero>());

		heroList[player].Add(hero);
	}

	void ReorderHeroList(Player player)
	{
		var targetEdge = GetPlayerType(player) == PlayerType.Local ? uiBattlefield.GetRemotePlayerEdge() : uiBattlefield.GetLocalPlayerEdge();

		heroList[player].OrderByDescending(hero => Mathf.Abs(targetEdge - uiBattlefield.UnityToGrid(hero.transform.position).y));
	}
	public bool CanSummonOnSelectedTile()
	{
		return uiBattlefield.CanSummonOnSelectedTile();
	}
	#endregion HERO_LOGIC

	public SpawnArea[,] GetFields()
	{
		return uiBattlefield.GetFields();
	}
	PlayerType GetPlayerType(Player player)
	{
		return gameController.GetPlayerType(player);
	}
}
