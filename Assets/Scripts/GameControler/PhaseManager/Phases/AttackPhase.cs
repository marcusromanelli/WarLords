
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack Phase", menuName = "ScriptableObjects/Phases/Attack", order = 1)]
public class AttackPhase : Phase
{
    public override IEnumerator PrePhase(Player currentPlayer, Player enemyPlayer)
    {
        yield break;
    }

    public override IEnumerator Resolve(Player currentPlayer, Player enemyPlayer)
    {
        yield return battlefield.AttackPhase(currentPlayer);
    }
}
