using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class UIBattlefield : MonoBehaviour
{
	[BoxGroup("Functionality"), Expandable, SerializeField] BattlefieldData battlefieldData;
	[BoxGroup("Functionality"), SerializeField] SpawnArea[,] battlefieldTiles;
	[BoxGroup("Functionality"), SerializeField, ReadOnly] SpawnArea selectedTile;

	public SpawnArea SelectedTile => selectedTile;

	private InputController inputController;
	private HandleCanSummonToken canSummonToken;
	private Player localPlayer;

	public void Setup(Player LocalPlayer, InputController InputController, HandleCanSummonToken CanSummonToken)
    {
		localPlayer = LocalPlayer;
		inputController = InputController;
		canSummonToken = CanSummonToken;

		Generate();
	}

    #region FIELD_GENERATION
    public void Generate()
	{
		if (battlefieldData.generateEveryGame)
			GenerateField();
	}
    void GenerateField()
	{
		if (battlefieldData.columnNumber % 2 == 0)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("The number of lanes must be an odd number.");
			return;
		}

		battlefieldTiles = new SpawnArea[battlefieldData.columnNumber, battlefieldData.rowNumber];

		for (int x = 0; x < battlefieldTiles.GetLength(0); x++)
		{
			for (int y = 0; y < battlefieldTiles.GetLength(1); y++)
			{
				var tileGridPosition = new Vector2(x, y);
				var tilePosition = GridToUnity(tileGridPosition);
				var prefab = GetRandomTilePrefab();

				battlefieldTiles[x, y] = GenerateTile(prefab, tilePosition, IsLocalSpawnRow(y));

				var tile = battlefieldTiles[x, y];
				tile.SetPosition(x, y);

				inputController.RegisterTargetCallback(MouseEventType.StartHover, tile.gameObject, SetSelectedTile);
				inputController.RegisterTargetCallback(MouseEventType.EndHover, tile.gameObject, SetUnselectedTile);
			}
		}
	}
	SpawnArea GenerateTile(SpawnArea prefab, Vector3 position, bool isSpawnArea)
	{
		var tile = ElementFactory.CreateObject<SpawnArea>(prefab, transform);
		tile.transform.position = position;

		return tile;
	}
	SpawnArea GetRandomTilePrefab()
	{
		return battlefieldData.tilePrefabs[UnityEngine.Random.Range(0, battlefieldData.tilePrefabs.Length)];
	}
	#endregion FIELD_GENERATION

	#region SUMMON_HELPER
	public bool CanPlayerSummonOnTile(Player player, SpawnArea spawnArea)
    {
		var isLocalPlayer = player == localPlayer;
		var gridPosition = UnityToGrid(spawnArea.transform.position);

		if (spawnArea == null)
			return false;

		if (isLocalPlayer)
			return IsLocalSpawnRow((int)gridPosition.y);

		return IsRemoteSpawnRow((int)gridPosition.y);
	}
	public SpawnArea[,] GetFields()
	{
		return battlefieldTiles;
	}
	#endregion SUMMON_HELPER

	#region FIELD_INTERACTION
	public List<SpawnArea> GetEmptyFields(Player player)
	{
		var isRemote = player != localPlayer;
		List<SpawnArea> result = new List<SpawnArea>();

		foreach (var tile in battlefieldTiles)
			if(CanPlayerSummonOnTile(player, tile))
				result.Add(tile);

		return result;
	}
	void SetSelectedTile(GameObject area)
	{

		if (selectedTile != null && selectedTile.gameObject == area.gameObject)
			return;

		var tile = area.GetComponent<SpawnArea>();

		if (selectedTile != null)
			selectedTile.SetSelected(false);

		selectedTile = tile;
		selectedTile.SetSelected(true);
	}
	void ShowSpawnTiles(Player player)
	{
		foreach (var tile in battlefieldTiles)
			if(CanPlayerSummonOnTile(player, tile))
				tile.SetSelectingSpawnArea();
	}
	void HideSpawnTiles()
	{
		foreach (var tile in battlefieldTiles)
		{
			tile.StopSelectingSpawnArea();
		}
	}
	void SetUnselectedTile(GameObject area)
	{
		if (selectedTile == null || selectedTile.gameObject != area.gameObject)
			return;

		selectedTile.SetSelected(false);
		selectedTile = null;
	}
	bool IsLocalSpawnRow(int rowNumber)
	{
		return (rowNumber <= battlefieldData.spawnAreaSize - 1);
	}
	bool IsRemoteSpawnRow(int rowNumber)
	{
		return (rowNumber >= battlefieldData.rowNumber - battlefieldData.spawnAreaSize);
	}
	bool IsLocalEdge(int rowNumber)
	{
		return rowNumber < GetLocalPlayerEdge();
	}
	bool IsRemoteEdge(int rowNumber)
	{
		return rowNumber > GetRemotePlayerEdge();
	}
	public void OnLocalPlayerHoldCard(Player player, CardObject cardObject)
	{
		if(cardObject == null || !canSummonToken(cardObject))
		{
			HideSpawnTiles();
			return;
		}

        ShowSpawnTiles(player);
	}
	#endregion FIELD_INTERACTION

	#region FIELD_HELPER
	public bool IsOnEnemyEdge(Player player, Vector2 position)
	{
		var isLocalPlayer = player == localPlayer;

		if (isLocalPlayer && IsRemoteEdge((int)position.y))
			return true;

		if (!isLocalPlayer && IsLocalEdge((int)position.y))
			return true;

		return false;
	}
	public bool IsOnEnemyEdge(Player player, SpawnArea spawnArea)
    {
		return IsOnEnemyEdge(player, spawnArea.GridPosition);
	}
	public SpawnArea GetTileByPosition(Vector3 unityPosition)
	{
		var position = UnityToGrid(unityPosition);

		return battlefieldTiles[(int)position.x, (int)position.y];
	}
	public SpawnArea GetTokenTile(Player player, CardObject hero)
	{
		foreach (var tile in battlefieldTiles)
			if (tile.Token == hero)
				return tile;

		return null;
	}
	public Vector3 GridToUnity(Vector2 pos)
	{
		return new Vector3((pos.x * battlefieldData.squareSize) + transform.position.x, transform.position.y, (pos.y * battlefieldData.squareSize) + transform.position.z);
	}
	public Vector2 UnityToGrid(Vector3 pos)
	{
		float x = Mathf.RoundToInt((pos.x - (transform.position.x)) / battlefieldData.squareSize);
		x = Mathf.Clamp(x, 0, x);
		x = x > (battlefieldData.columnNumber - 1) ? battlefieldData.columnNumber - 1 : x;

		float y = Mathf.RoundToInt((pos.z - (transform.position.z)) / battlefieldData.squareSize);
		y = Mathf.Clamp(y, 0, y);
		y = y > (battlefieldData.rowNumber - 1) ? battlefieldData.rowNumber - 1 : y;


		return new Vector2(x, y);
	}
	public Vector2 Normalize(Vector2 pos)
	{
		float x = pos.x;
		x = Mathf.Clamp(x, 0, x);
		x = x > (battlefieldData.columnNumber - 1) ? battlefieldData.columnNumber - 1 : x;

		float y = pos.y;
		y = Mathf.Clamp(y, 0, y);
		y = y > (battlefieldData.rowNumber - 1) ? battlefieldData.rowNumber - 1 : y;


		return new Vector2(x, y);
	}
	public float GetRemotePlayerEdge()
	{
		return battlefieldData.rowNumber - battlefieldData.spawnAreaSize;
	}
	public float GetLocalPlayerEdge()
	{
		return battlefieldData.spawnAreaSize - 1;
	}
    #endregion FIELD_HELPER
}
