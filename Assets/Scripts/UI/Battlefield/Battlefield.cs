using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

[ExecuteInEditMode]
public class Battlefield : MonoBehaviour
{

	[SerializeField] GameController gameController;
	[SerializeField] GameObject[] gridTiles;
	[SerializeField] int numberOfLanes = 5;
	[SerializeField] int numberOfSquares = 7;
	[SerializeField] int numberOfSpawnAreasPerLane = 2;

	[SerializeField] float squareSize = 0.8f;

	List<SpawnArea> tiles;
	SpawnArea selectedTile;


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

				tiles.Add(spawnArea);
			}

			currentLane++;
		}
	}

	void EraseAllGrid()
	{
		if (tiles == null)
		{
			tiles = new List<SpawnArea>();
			return;
		}

		foreach (var tile in tiles) {
			if (tile != null)
				DestroyImmediate(tile);
		};

		var children = GetComponentsInChildren<Transform>();

		foreach (var child in children)
        {
			if(child != null && child.gameObject != this.gameObject)
				DestroyImmediate(child.gameObject);
        }

		tiles.Clear();
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

		return tiles.FindAll(tile => tile.playerType == player.GetPlayerType()
										&& tile.IsSummonable && tile.HasHero() == false);
	}
	public List<SpawnArea> GetFields(Player player)
	{
		var isLocalPlayer = player == gameController.GetLocalPlayer();
		var isRemotePlayer = player == gameController.GetRemotePlayer();

		return tiles.FindAll(tile => tile.playerType == player.GetPlayerType()
										&& tile.IsSummonable);
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
		var opponent = gameController.GetOpponent(hero.GetPlayer());

		if (opponent.GetPlayerType() == PlayerType.Local)
			return IsAtLocalPlayerEdge(hero.GridPosition);
		else
			return IsAtRemotePlayerEdge(hero.GridPosition);
	}
	public bool IsAtLocalPlayerEdge(Vector2 position)
	{
		return IsAtLocalPlayerEdge(position.y);
	}
	public bool IsAtLocalPlayerEdge(float YPosition)
	{
		return (YPosition <= numberOfSpawnAreasPerLane - 1);
	}
	public bool IsAtRemotePlayerEdge(Vector2 position)
	{
		return IsAtRemotePlayerEdge(position.y);

	}
	public bool IsAtRemotePlayerEdge(float YPosition)
	{
		return (YPosition >= numberOfSquares - numberOfSpawnAreasPerLane);
	}
	public bool CanSummon(Player player, SpawnArea area)
    {
		if (area == null)
			return false;

		if (!area.IsSummonable && !area.IsTemporarilySummonable)
			return false;

		var type = gameController.GetCurrentPlayer().GetPlayerType();

		if (area.IsSummonable && area.playerType != type)
			return false;

		return true;
    }
	public bool CanSummonOnSelectedTile(Player player)
    {
		return CanSummon(player, selectedTile);
    }

}