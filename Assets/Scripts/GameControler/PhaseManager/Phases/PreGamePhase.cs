
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Pre-Game Phase", menuName = "ScriptableObjects/Phases/Pre-Game", order = 1)]
public class PreGamePhase : Phase
{
	[HideInInspector] bool didRun = false;

	public override void Setup(PhaseManager phaseManager, Battlefield battlefield)
	{
		base.Setup(phaseManager, battlefield);

		didRun = false;
	}

	public override IEnumerator PrePhase(Player currentPlayer, Player enemyPlayer)
	{
		if (didRun)
			yield break;

		phaseManager.EnablePlayer(currentPlayer);
		phaseManager.EnablePlayer(enemyPlayer);

		currentPlayer.SetupPlayDeck();
		enemyPlayer.SetupPlayDeck();

		yield return currentPlayer.IsInitialized();

		yield return enemyPlayer.IsInitialized();
	}
	public override IEnumerator Resolve(Player currentPlayer, Player enemyPlayer)
	{
		if (didRun)
			yield break;

		StartPreGame(currentPlayer, enemyPlayer);

		yield break;
	}
	void StartPreGame(Player currentPlayer, Player enemyPlayer)
	{
		currentPlayer.TryDrawCards(GameConfiguration.numberOfInitialDrawnCards);
		enemyPlayer.TryDrawCards(GameConfiguration.numberOfInitialDrawnCards);

		currentPlayer.AddCondition(MandatoryConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);
		enemyPlayer.AddCondition(MandatoryConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);

		didRun = true;
	}
}
