
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Movement Phase", menuName = "ScriptableObjects/Phases/Movement", order = 1)]
public class MovementPhase : Phase
{
    public override IEnumerator PrePhase(Player currentPlayer, Player enemyPlayer)
	{
		yield break;
	}

    public override IEnumerator Resolve(Player currentPlayer, Player enemyPlayer)
	{
		//yield return battlefield.MovementPhase();
		yield break;
	}
}
