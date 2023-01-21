using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using System.Threading.Tasks;

[ExecuteInEditMode]
public class Battlefield : MonoBehaviour, ICardPlaceable
{
	[SerializeField] protected Player localPlayerController;
	[SerializeField] protected Transform CardReferencePosition;
	[SerializeField] List<SpawnArea> generatedTiles;
	[SerializeField] GameController gameController;
	[SerializeField] GameObject[] gridTiles;
	[SerializeField] int numberOfLanes = 5;
	[SerializeField] int numberOfSquares = 7;
	[SerializeField] int numberOfSpawnAreasPerLane = 2;

	[SerializeField] float squareSize = 0.8f;

	SpawnArea selectedTile;
	CardObject cardWaitingForSpawn;
	Dictionary<Player, List<Hero>> heroList = new Dictionary<Player, List<Hero>>();


    private void Awake()
    {
		if (!Application.isPlaying)
			return;

		localPlayerController.OnHoldCard += SetCardBeingHeld;
		localPlayerController.OnReleaseCard += SetCardBeingReleased;

	}

    [Button("Generate Grid")]
	void Generate()
	{
		if (numberOfLanes % 2 == 0)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("The number of lanes must be an odd number.");
			return;
		}

		EraseAllGrid();

		int currentLane = 0;

		Vector3 aux;
		GameObject lane;

		while (currentLane < numberOfLanes)
		{
			aux = transform.position;
			aux.x += currentLane * squareSize;
			lane = new GameObject("Lane " + (currentLane + 1));
			lane.transform.position = aux;
			lane.transform.SetParent(transform, true);

			for (int i = 0; i < numberOfSquares; i++)
			{
				aux.z = i * squareSize;
				GameObject aux2 = Instantiate(gridTiles[currentLane], Vector3.zero, Quaternion.identity);
				aux2.transform.position = aux;
				aux2.transform.SetParent(lane.transform);

				var spawnArea = aux2.GetComponent<SpawnArea>();

				var isLocal = (i <= numberOfSpawnAreasPerLane - 1);
				var isRemote = (i >= numberOfSquares - numberOfSpawnAreasPerLane);
					
				if(isLocal || isRemote)
					spawnArea.Setup(this, gameController, isLocal ? PlayerType.Local : PlayerType.Remote);
				else
					spawnArea.Setup(this, gameController);

				generatedTiles.Add(spawnArea);
			}

			currentLane++;
		}
	}

	void EraseAllGrid()
	{
		if (generatedTiles == null)
		{
			generatedTiles = new List<SpawnArea>();
			return;
		}

		foreach (var tile in generatedTiles) {
			if (tile != null)
				DestroyImmediate(tile);
		};

		var children = GetComponentsInChildren<Transform>();

		foreach (var child in children)
        {
			if(child != null && child.gameObject != this.gameObject)
				DestroyImmediate(child.gameObject);
        }

		generatedTiles.Clear();
	}


	public Vector3 GridToUnity(Vector2 pos)
	{
		return new Vector3((pos.x * squareSize) + transform.position.x, transform.position.y, (pos.y * squareSize) + transform.position.z);
	}

	public Vector2 UnityToGrid(Vector3 pos)
	{
		float x = Mathf.RoundToInt((pos.x - (transform.position.x)) / squareSize);
		x = x < 0 ? 0 : x;
		x = x > (numberOfLanes - 1) ? numberOfLanes - 1 : x;

		float y = Mathf.RoundToInt((pos.z - (transform.position.z)) / squareSize);
		y = y < 0 ? 0 : y;
		y = y > (numberOfSquares - 1) ? numberOfSquares - 1 : y;


		return new Vector2(x, y);
	}

	public Vector2 Normalize(Vector2 pos)
	{
		float x = pos.x;
		x = x < 0 ? 0 : x;
		x = x > (numberOfLanes - 1) ? numberOfLanes - 1 : x;

		float y = pos.y;
		y = y < 0 ? 0 : y;
		y = y > (numberOfSquares - 1) ? numberOfSquares - 1 : y;


		return new Vector2(x, y);
	}

	public SpawnArea GetSelectedTile()
	{
		return selectedTile;
	}

	public void SetSelectedTile(SpawnArea area)
	{
		selectedTile = area;
	}

	public void SetUnselectedTile(SpawnArea area)
	{
		if (selectedTile != area)
			return;

		selectedTile = null;
	}

	public List<SpawnArea> GetEmptyFields(Player player)
	{
		var isLocalPlayer = player == gameController.GetLocalPlayer();
		var isRemotePlayer = player == gameController.GetRemotePlayer();

		return generatedTiles.FindAll(tile => tile.playerType == player.GetPlayerType()
										&& tile.IsSummonArea && tile.HasHero() == false);
	}
	public List<SpawnArea> GetFields(Player player)
	{
		var isLocalPlayer = player == gameController.GetLocalPlayer();
		var isRemotePlayer = player == gameController.GetRemotePlayer();

		return generatedTiles.FindAll(tile => tile.playerType == player.GetPlayerType()
										&& tile.IsSummonArea);
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
	public bool CanSummon(Player player, SpawnArea area)
    {
		if (area == null)
			return false;

		if (!area.IsSummonable)
			return false;

		if (!GameController.MatchHasStarted)
			return false;

		var type = gameController.GetCurrentPlayer().GetPlayerType();

		if (area.IsSummonArea && area.playerType != type)
			return false;

		return true;
    }
	public bool CanSummonOnSelectedTile(Player player)
    {
		return CanSummon(player, selectedTile);
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
	}*/

	public void SummonByClick(CardObject hero)
	{
		//hero.transform.SetParent(transform);

		//hero.player.AddCondition(ConditionType.PickSpawnArea);

		//gameController.SetTriggerType(TriggerType.OnBeforeSpawn, hero);

		//cardWaitingForSpawn = hero;
	}

	public void Summon(CardObject cardObject, SpawnArea area)
	{
		cardObject.transform.SetParent(transform);

		SummonCard(cardObject, area);
	}

	public void Kill(CardObject card)
	{
		//Hero hero = card.Character;
		//Destroy(hero.gameObject);
		//gameController.SetTriggerType(TriggerType.OnAfterDeath, card);
		//Destroy(card.gameObject);
	}

	public Vector3 getTopPosition()
	{
		return transform.position;
	}

	public Quaternion getTopRotation()
	{
		return Quaternion.Euler(90, 0, 0);
	}

	public Vector3 getTopScale()
	{
		return Vector3.one;
	}

	public List<Hero> GetHeroes(Player player)
    {
		return heroList[player];
	}
	public IEnumerator MovementPhase()
    {
		yield return DoMovement();
    }
	public async Task AttackPhase()
    {
		/*hasFinishedAttack = false;

		//StartCoroutine(DoMovement());

        while (!hasFinishedAttack)
        {
			await Task.Delay(25);
		};*/
    }
	IEnumerator DoMovement()
    {
		gameController.DisablePlayers();

		var currentPlayer = gameController.GetCurrentPlayer();

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

		gameController.EnablePlayers();

		yield return new WaitForSeconds(1);
	}
	public void SummonCard(CardObject heroCard, SpawnArea spawnArea)
	{
		//var player = heroCard.player;
		//var areaPosition = spawnArea.transform.position;
		//var hero = heroCard.GetCharacterResource();
				
		//hero = Instantiate(hero, areaPosition, Quaternion.identity);

		//hero.Setup(gameController, this, heroCard);

		//GameConfiguration.PlaySFX(GameConfiguration.Summon);

		//gameController.SetTriggerType(TriggerType.OnAfterSpawn, heroCard);

		//player.SpendMana(heroCard.CalculateSummonCost());

		//AddHero(player, hero);

		//ReorderHeroList(player);

		//SetHeroTile(hero, spawnArea);

		//heroCard.transform.SetParent(hero.transform, true);

		//player.Summon(heroCard);
	}
	void AddHero(Player player, Hero hero)
    {
		if (!heroList.ContainsKey(player))
			heroList.Add(player, new List<Hero>());

		heroList[player].Add(hero);
	}

	void ReorderHeroList(Player player)
    {
		var targetEdge = player.GetPlayerType() == PlayerType.Local ? GetRemotePlayerEdge() : GetLocalPlayerEdge();

		heroList[player].OrderByDescending(hero => Mathf.Abs(targetEdge - hero.GridPosition.y));
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

		var currentPlayer = gameController.GetCurrentPlayer();

        if (!CanSummon(currentPlayer, tile))
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("You cannot summon on this tile.");
			return;
		}

		/*if (!currentPlayer.CanSpendMana(cardWaitingForSpawn.CalculateSummonCost()))
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("Not enought mana.");
			return;
		}*/

		SummonCard(cardWaitingForSpawn, tile);

		cardWaitingForSpawn = null;
	}

	public SpawnArea GetHeroTile(Player player, Hero hero)
	{
		return generatedTiles.First(tile => tile.Hero == hero);
	}
	public SpawnArea GetTileByPosition(Vector2 position)
	{
		return generatedTiles.First(tile => UnityToGrid(tile.transform.position) == position);
	}
	//public bool HasSummonedHero(Player player, int cardId)
    //{
		//return heroList[player].Any(hero => hero.CardObject.GetCardData().CardID == cardId);
    //}
	public void SetHeroTile(Hero hero, SpawnArea spawnArea)
    {
		if (spawnArea.Hero != null)
			Debug.LogError("[ERROR] TILE NOT EMPTY");

		spawnArea.Hero = hero;
    }

    public void CheckMouseOver(bool requiresClick)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetTopPosition()
    {
        throw new System.NotImplementedException();
    }

    public Quaternion GetTopRotation()
    {
        throw new System.NotImplementedException();
    }

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
}