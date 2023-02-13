using UnityEngine;

[CreateAssetMenu(fileName = "BattlefieldData", menuName = "ScriptableObjects/BattlefieldData", order = 1)]
public class BattlefieldData : ScriptableObject
{
    public bool generateEveryGame;
    public SpawnArea[] tilePrefabs;
    public int columnNumber;
    public int rowNumber;
    public int spawnAreaSize;
    public float squareSize;
}
