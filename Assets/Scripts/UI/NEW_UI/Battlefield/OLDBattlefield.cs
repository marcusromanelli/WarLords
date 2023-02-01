using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using System.Threading.Tasks;
using System;


public class OLD____Battlefield : MonoBehaviour
{
	/*[BoxGroup("Functionality"), SerializeField] Player player;
	[BoxGroup("Functionality"), SerializeField] SpawnArea[,] battlefieldTiles;

	[BoxGroup("Components"), SerializeField] InputController inputController;
    [BoxGroup("Components"), SerializeField] Transform CardReferencePosition;
	[BoxGroup("Components"), SerializeField] GameController gameController;

	
	CardObject cardWaitingForSpawn;
	Dictionary<Player, List<Card>> heroList = new Dictionary<Player, List<Card>>();
	HandleCanSummonHero canSummonHero;

    private void Start()
	{
    }
	public void Setup(HandleCanSummonHero canSummonHero)
    {
		this.canSummonHero = canSummonHero;
	}*/


	/*

	/*
	public List<SpawnArea> GetFields(Player player)
	{
		return null;
		/*var isLocalPlayer = player == gameController.GetLocalPlayer();
		var isRemotePlayer = player == gameController.GetRemotePlayer();

		return generatedTiles.FindAll(tile => tile.playerType == player.GetPlayerType()
										&& tile.IsSummonArea);*
	}

	public int GetNumberOfSquares()
	{
		return numberOfSquares;
	}

	public int GetNumberOfSpawnAreasPerLane()
	{
		return numberOfSpawnAreasPerLane;
	}

	public bool IsAtEnemyEdge(Hero hero)
    {
		return false;
		//var opponent = gameController.GetOpponent(hero.CardObject.player);

		//if (opponent.GetPlayerType() == PlayerType.Local)
		//	return IsAtLocalPlayerEdge(hero.GridPosition);
		//else
		//	return IsAtRemotePlayerEdge(hero.GridPosition);
	}
	public bool IsAtLocalPlayerEdge(Vector2 position)
	{
		return IsAtLocalPlayerEdge(position.y);
	}
	public bool IsAtLocalPlayerEdge(float YPosition)
	{
		return (YPosition <= GetLocalPlayerEdge());
	}
	public bool IsAtRemotePlayerEdge(Vector2 position)
	{
		return IsAtRemotePlayerEdge(position.y);
	}
	public float GetRemotePlayerEdge()
	{
		return numberOfSquares - numberOfSpawnAreasPerLane;
	}
	public float GetLocalPlayerEdge()
	{
		return numberOfSpawnAreasPerLane - 1;
	}
	public bool IsAtRemotePlayerEdge(float YPosition)
	{
		return (YPosition >= GetRemotePlayerEdge());
	}



	/*void CheckEmptySpawnArea()
	{
		var currentTile = GetSelectedTile();

		if (!awaitingToSelectSpawnTile || currentTile == null || !Input.GetMouseButton(0))
			return;

		if (currentTile.Hero != null)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			return;
		}

		SummonCard(cardWaitingForSpawn, currentTile);
		awaitingToSelectSpawnTile = false;
		cardWaitingForSpawn = null;
	}*

	public void SummonByClick(CardObject hero)
	{
		//hero.transform.SetParent(transform);

		//hero.player.AddCondition(ConditionType.PickSpawnArea);

		//gameController.SetTriggerType(TriggerType.OnBeforeSpawn, hero);

		//cardWaitingForSpawn = hero;
	}



	public void Kill(CardObject card)
	{
		//Hero hero = card.Character;
		//Destroy(hero.gameObject);
		//gameController.SetTriggerType(TriggerType.OnAfterDeath, card);
		//Destroy(card.gameObject);
	}

	public List<Card> GetHeroes(Player player)
    {
		return heroList[player];
	}
	void AddHero(Player player, Card hero)
    {
		if (!heroList.ContainsKey(player))
			heroList.Add(player, new List<Card>());

		heroList[player].Add(hero);
	}

	void ReorderHeroList(Player player)
    {
		/*var targetEdge = player.GetPlayerType() == PlayerType.Local ? GetRemotePlayerEdge() : GetLocalPlayerEdge();

		heroList[player].OrderByDescending(hero => Mathf.Abs(targetEdge - hero.GridPosition.y));*
    }
	public void SetCardBeingHeld(CardObject card)
    {
		cardWaitingForSpawn = card;
	}
	public void SetCardBeingReleased(CardObject card)
    {
		//Card is released. Summon it
		ClickedTile(selectedTile);
	}
	public void ClickedTile(SpawnArea tile)
    {
		if (tile == null)
			return;

		if (cardWaitingForSpawn == null)
			return;

		//var currentPlayer = gameController.GetCurrentPlayer();

        /*if (!CanSummon(currentPlayer, tile))
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("You cannot summon on this tile.");
			return;
		}*/

	/*if (!currentPlayer.CanSpendMana(cardWaitingForSpawn.CalculateSummonCost()))
	{
		GameConfiguration.PlaySFX(GameConfiguration.denyAction);
		Debug.LogWarning("Not enought mana.");
		return;
	}*

	SummonCard(cardWaitingForSpawn, tile);

	cardWaitingForSpawn = null;
}



//public bool HasSummonedHero(Player player, int cardId)
//{
	//return heroList[player].Any(hero => hero.CardObject.GetCardData().CardID == cardId);
//}


/*public void Attack()
{
	isAttacking = true;
	StartCoroutine(DoAttack());
}
IEnumerator DoAttack()
{
	isAttacking = true;

	for (int i = 0; i < numberOfAttacks; i++)
	{

		gameController.SetTriggerType(TriggerType.OnBeginAttack, CardObject);
		Hero targetHero = checkForEnemiesInFront();
		if (targetHero == null)
		{
			var isAtEdgeOfOpponent = battlefield.IsAtEnemyEdge(this);

			if (isAtEdgeOfOpponent)
			{
				Debug.LogWarning("Attacked player with " + calculateAttackPower() + " damage");
				LogController.Log(Action.AttackPlayer, calculateAttackPower(), player, gameController.GetOpponent(player));
				gameController.AttackPlayer(calculateAttackPower());
				lastGivenDamage = calculateAttackPower();
			}
		}
		else
		{
			if (targetHero.player != player)
			{
				LogController.Log(Action.AttackChar, calculateAttackPower(), this, targetHero);
				Debug.LogWarning("Attacked " + targetHero.name + " with " + calculateAttackPower() + " damage");
				targetHero.doDamage(calculateAttackPower());
				lastGivenDamage = calculateAttackPower();
			}
		}

		gameController.SetTriggerType(TriggerType.OnAfterAttack, CardObject);
		yield return new WaitForSeconds(0.25f);
	}



	numberOfAttacks = 1;
	isAttacking = false;
}*/
	

	/*
	

	
	public SpawnArea[,] GetFields()
	{
		return battlefieldTiles;
	}*/
}