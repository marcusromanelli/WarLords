
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Action Phase", menuName = "ScriptableObjects/Phases/Action", order = 1)]
public class ActionPhase : Phase
{
    public override IEnumerator PrePhase(Player currentPlayer, Player enemyPlayer)
    {
        phaseManager.EnablePlayer(currentPlayer);

        currentPlayer.StartActionPhase();
        yield break;
    }
    public override IEnumerator Resolve(Player currentPlayer, Player enemyPlayer)
    {
        yield return currentPlayer.IsResolvingActionPhase();
    }
}
