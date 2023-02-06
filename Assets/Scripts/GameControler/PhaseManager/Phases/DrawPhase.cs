using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Draw Phase", menuName = "ScriptableObjects/Phases/Draw", order = 1)]
public class DrawPhase : Phase
{
    public override IEnumerator PrePhase(Player currentPlayer, Player enemyPlayer)
    {
        phaseManager.EnablePlayer(currentPlayer);

        currentPlayer.TryDrawCards(GameConfiguration.numberOfCardsToDrawEveryTurn);

        yield break;
    }

    public override IEnumerator Resolve(Player currentPlayer, Player enemyPlayer)
    {

		yield return null;
	}
}
