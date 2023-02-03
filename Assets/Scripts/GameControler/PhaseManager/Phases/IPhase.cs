using System.Collections;

public interface IPhase
{
    void Setup(PhaseManager phaseManager);
    IEnumerator PrePhase(Player currentPlayer, Player enemyPlayer);
    IEnumerator Resolve(Player currentPlayer, Player enemyPlayer);
    PhaseType GetPhaseType();
}
