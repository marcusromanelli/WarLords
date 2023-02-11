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

	private Dictionary<Player, List<CardObject>> tokenList = new Dictionary<Player, List<CardObject>>();
	private GameController gameController;
	private InputController inputController;
	private Player localPlayer, remotePlayer;

	public bool isVisualizingTokenCard = false;

	public void PreSetup(Player LocalPlayer, Player RemotePlayer, InputController InputController, GameController GameController, HandleCanSummonToken CanSummonToken)
	{
		gameController = GameController;
		inputController = InputController;

		localPlayer = LocalPlayer;
		remotePlayer = RemotePlayer;

		localPlayer.OnHoldCard += OnLocalPlayerHoldingCard;

		uiBattlefield.Setup(LocalPlayer, InputController, CanSummonToken);
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
		if (!tokenList.ContainsKey(currentPlayer))
			yield break;

		List<CardObject> tokenes = tokenList[currentPlayer];

		foreach (CardObject token in tokenes)
		{
			yield return MoveToken(currentPlayer, token);
		}
	}
	IEnumerator MoveToken(Player currentPlayer, CardObject token)
	{
		var tile = GetTokenTile(currentPlayer, token);
		tile.RemoveToken();
		var newGridPosition = uiBattlefield.Normalize(CalculateTokenEndPosition(currentPlayer, token));
		var newPosition = uiBattlefield.GridToUnity(newGridPosition);

		token.Move(newPosition);

		Debug.Log("Token " + token.name + " moved to " + newGridPosition);

		yield return token.IsWalking();

		var newTile = GetTileByPosition(newPosition);
		token.SetPosition(newGridPosition);

		SetTokenTile(token, newTile);
		EvaulateTokenTarget(currentPlayer, token);
	}
	SpawnArea GetTileByPosition(Vector3 position)
	{
		return uiBattlefield.GetTileByPosition(position);
	}
	Vector2 CalculateTokenEndPosition(Player currentPlayer, CardObject token)
	{
		Vector2 gridPos = token.GridPosition;
		var currentField = GetFields()[(int)gridPos.x, (int)gridPos.y];

		gridPos.y += GetTokenMovementDirection(currentPlayer) * token.CalculateWalkSpeed();

		if (uiBattlefield.IsOnEnemyEdge(currentPlayer, currentField))
			return token.GridPosition;

		var targetField = GetFields()[(int)gridPos.x, (int)gridPos.y];

		if (targetField.Token != null)
			return token.GridPosition;

		return gridPos;
	}
	void EvaulateTokenTarget(Player currentPlayer, CardObject token)
	{
		Vector2 gridPos = token.GridPosition;
		var currentField = GetFields()[(int)gridPos.x, (int)gridPos.y];

		var targetPlayer = currentPlayer == localPlayer ? remotePlayer : localPlayer;

		if (uiBattlefield.IsOnEnemyEdge(currentPlayer, currentField))
		{
			Debug.Log(token.name + " is now targeting " + targetPlayer.name);
			token.SetTarget(targetPlayer);
			return;
		}

		gridPos.y += GetTokenMovementDirection(currentPlayer) * token.CalculateWalkSpeed();
		var nextField = GetFields()[(int)gridPos.x, (int)gridPos.y];
		var targetToken = nextField.Token;

		if (targetToken != null && !PlayerHasToken(currentPlayer, targetToken))
		{

			Debug.Log(token.name + " is now targeting " + targetToken.name);
			token.SetTarget(targetToken);
		}
		else
		{
			Debug.Log("Reseted targetting for " + token.name);
			token.ResetTarget();
		}
	}
	bool PlayerHasToken(Player currentPlayer, CardObject token)
	{
		return PlayerHasTokenSummoned(currentPlayer, token);
	}
	int GetTokenMovementDirection(Player player)
	{
		return player == localPlayer ? 1 : -1;
	}
	public SpawnArea GetTokenTile(Player player, CardObject token)
	{
		return uiBattlefield.GetTokenTile(player, token);
	}
	#endregion MOVEMENT_PHASE

	#region ATTACK_PHASE
	public IEnumerator AttackPhase(Player currentPlayer, Player enemyPlayer)
	{
		yield return DoAttack(currentPlayer, enemyPlayer);
	}
	IEnumerator DoAttack(Player currentPlayer, Player enemyPlayer)
	{
		if (!tokenList.ContainsKey(currentPlayer))
			yield break;

		List<CardObject> tokenes = tokenList[currentPlayer];

		foreach (CardObject token in tokenes)
			TokenAttack(token, enemyPlayer);
	}
	void TokenAttack(CardObject token, Player enemyPlayer)
	{
		if (!token.HasTarget())
			return;

		var target = token.GetTarget();

		Debug.Log(token.name + " attacking " + target);

		token.Attack();

		var isAlive = TargetIsAlive(target, enemyPlayer);
				
		Debug.Log("Target is alive? " + isAlive);

		if (!isAlive)
			token.ResetTarget();
	}
	bool TargetIsAlive(IAttackable target, Player ownerPlayer)
	{
		if (target.GetLife() > 0)
			return true;

		if (!(target is CardObject))
			return true;

		var CardObject = target as CardObject;

		DestroyToken(CardObject, ownerPlayer);

		return false;
	}
	#endregion ATTACK_PHASE

	#region HERO_LOGIC
	public void Summon(Player player, CardObject cardObject, SpawnArea spawnArea = null)
	{
		spawnArea ??= uiBattlefield.SelectedTile;

		cardObject.transform.position = spawnArea.transform.position;
		cardObject.transform.localRotation = spawnArea.GetRotationReference();
		cardObject.transform.SetParent(transform, true);

		cardObject.Invoke();
		//var token = TokenFactory.Create(cardObject, transform, spawnArea.transform.position, spawnArea.GetRotationReference());

		//token.SetPosition(uiBattlefield.UnityToGrid(spawnArea.transform.position));

		//gameController.SetTriggerType(TriggerType.OnAfterSpawn, tokenCard);

		AddToken(player, cardObject);

		ReorderTokenList(player);

		SetTokenTile(cardObject, spawnArea);
	}
	public bool PlayerHasTokenSummoned(Player player, CardObject cardObject)
	{
		if (!tokenList.ContainsKey(player))
			return false;

		return tokenList[player].Any(c => c.Data.Id == cardObject.Data.Id);
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
	void AddToken(Player player, CardObject token)
	{
		if (!tokenList.ContainsKey(player))
			tokenList.Add(player, new List<CardObject>());

		tokenList[player].Add(token);
	}
	void RemoveToken(Player player, CardObject token)
	{
		if (!tokenList.ContainsKey(player))
			return;

		tokenList[player].Remove(token);
	}
	void ReorderTokenList(Player player)
	{
		var targetEdge = player == localPlayer ? uiBattlefield.GetRemotePlayerEdge() : uiBattlefield.GetLocalPlayerEdge();

		tokenList[player].OrderByDescending(token => Mathf.Abs(targetEdge - uiBattlefield.UnityToGrid(token.transform.position).y));
	}
	void SetTokenTile(CardObject token, SpawnArea spawnArea)
	{
		if (spawnArea.Token != null)
			Debug.LogError("[ERROR] TILE NOT EMPTY");

		spawnArea.SetToken(token);
	}
	void DestroyToken(CardObject CardObject, Player ownerPlayer)
    {
		var tile = uiBattlefield.GetTokenTile(ownerPlayer, CardObject);
		tile.RemoveToken();

		RemoveToken(ownerPlayer, CardObject);

		ownerPlayer.OnTokenDied(CardObject, tile);
	}
	#endregion HERO_LOGIC

	public SpawnArea[,] GetFields()
	{
		return uiBattlefield.GetFields();
	}
}
