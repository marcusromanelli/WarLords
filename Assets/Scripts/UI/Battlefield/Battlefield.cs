using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[ExecuteInEditMode]
public class Battlefield : MonoBehaviour
{

	[SerializeField] GameObject[] gridTiles;
	[SerializeField] int numberOfLanes = 5;
	[SerializeField] int numberOfSquares = 7;
	[SerializeField] int numberOfSpawnAreasPerLane = 2;

	[SerializeField] bool doGenerate;
	[SerializeField] bool updateSquareSize;
	[SerializeField] float squareSize = 0.8f;

	List<SpawnArea> tiles;
	SpawnArea selectedTile;


	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (doGenerate)
		{
			doGenerate = false;
			Generate();
		}

		if (numberOfLanes % 2 == 0)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("The number of lanes must be an odd number.");
		}
	}


	void Generate()
	{
		if (numberOfLanes % 2 != 0)
		{
			EraseAllGrid();

			int currentLane = 0;

			Vector3 aux;
			;
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
					var isSummonable = (i <= numberOfSpawnAreasPerLane - 1) || (i >= numberOfSquares - numberOfSpawnAreasPerLane);

					spawnArea.SetSummonable(isSummonable);

					tiles.Add(spawnArea);
				}

				currentLane++;
			}

		}
		else
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("The number of lanes must be an odd number.");
		}
	}

	void EraseAllGrid()
	{
		if (tiles == null)
		{
			tiles = new List<SpawnArea>();
			return;
		}

		tiles.ForEach(delegate (SpawnArea aux) {
			if (aux != null)
			{
				DestroyImmediate(aux);
			}
		});

		GetComponentsInChildren<Transform>().ToList().ForEach(delegate (Transform aux) {
			if (aux != null && aux != transform)
			{
				DestroyImmediate(aux.gameObject);
			}
		});

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
		return tiles.FindAll(tile => tile.player == player && tile.IsSummonable && tile.HasHero() == false);
	}
	public List<SpawnArea> GetFields(Player player)
	{
		return tiles.FindAll(tile => tile.player == player && tile.IsSummonable);
	}

	public int GetNumberOfSquares()
	{
		return numberOfSquares;
	}

	public int GetNumberOfSpawnAreasPerLane()
	{
		return numberOfSpawnAreasPerLane;
	}
}