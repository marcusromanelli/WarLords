using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class PhaseManager : MonoBehaviour, IPhaseManager
{
    public delegate void HandleOnPhaseChange(PhaseType newPhase);
    public event HandleOnPhaseChange OnPhaseChange;

    [BoxGroup("Components"), SerializeField] PhaseTitle phaseTitle;
    [BoxGroup("Components"), SerializeField, Expandable] Phase[] phaseCycle;

	[BoxGroup("Gameplay"), SerializeField, ReadOnly] Player CurrentPlayer;
	[BoxGroup("Gameplay"), SerializeField, ReadOnly] Player EnemyPlayer;

	private int currentPhaseIndex = 0;
	private Player localPlayer, remotePlayer;
	private bool GameHasEnded = false;
	private Phase CurrentPhase => phaseCycle[currentPhaseIndex];


	#region GAMECONTROLLER_INTERFACE
	public void Setup(Player LocalPlayer, Player RemotePlayer)
    {
		localPlayer = LocalPlayer;
		remotePlayer = RemotePlayer;

		InitializePhases();
	}
	public bool CanPlayerInteract(Player player)
	{
		return player.enabled;
	}
	public void StartCycle()
    {
		StartPhaseCycle();
	}
	#endregion GAMECONTROLLER_INTERFACE

	#region PHASES_INTERFACE
	public void DisablePlayer(Player player)
	{
		TogglePlayer(player, false);
	}
	public void EnablePlayer(Player player)
	{
		TogglePlayer(player, true);
	}
	public
	void NextTurn()
	{
		if (CurrentPlayer == null)
		{
			CurrentPlayer = localPlayer;
			EnemyPlayer = remotePlayer;
		}
		else
		{
			CurrentPlayer = (CurrentPlayer == remotePlayer) ? localPlayer : remotePlayer;
			EnemyPlayer = (CurrentPlayer == localPlayer) ? remotePlayer : localPlayer;
		}

		Debug.Log("Player " + CurrentPlayer + " turn.");
	}
	#endregion PHASES_INTERFACE




	#region PHASE_LOGIC


	bool WatchEndGame()
	{
		return false;

		//TODO ARRRRG
		/*if (LocalPlayer.GetCurrentLife() <= 0)
		{
			PhasesTitle.SetWinner(RemotePlayer);
			LocalPlayer.enabled = false;
			RemotePlayer.enabled = false;
		}
		else if (RemotePlayer.GetCurrentLife() <= 0)
		{
			PhasesTitle.SetWinner(LocalPlayer);
			LocalPlayer.enabled = false;
			RemotePlayer.enabled = false;
		}*/
	}

	#endregion PHASE_LOGIC


	void InitializePhases()
    {
		foreach (var phase in phaseCycle)
			phase.Setup(this);
    }
	void TogglePlayer(Player player, bool value)
	{
		player.enabled = value;
	}
	void StartPhaseCycle()
    {
		StartCoroutine(PhaseCycle());
    }
	void NextPhase()
    {
		currentPhaseIndex = (currentPhaseIndex + 1) % phaseCycle.Length;
	}
	IEnumerator PhaseCycle()
    {
		NextTurn();

        while (!GameHasEnded)
        {
			yield return ResolvePhase(CurrentPhase);

			yield return FinishCurrentPhase();
		}


		Debug.Log("Game ended");
	}
	IEnumerator FinishCurrentPhase()
    {
		yield return AwaitConditionsToSolve();

		DisablePlayer(localPlayer);
		DisablePlayer(remotePlayer);

		NextPhase();

		yield return StartChangingPhases();
	}
	IEnumerator ResolvePhase(Phase phase)
    {
		yield return phase.PrePhase(CurrentPlayer, EnemyPlayer);

		OnPhaseChange?.Invoke(CurrentPhase.GetPhaseType());

		yield return phase.Resolve(CurrentPlayer, EnemyPlayer);
	}
	IEnumerator AwaitConditionsToSolve()
	{
		var hasConditions = true;
		while (hasConditions)
		{
			yield return null;

			if (CurrentPlayer.HasConditions())
				EnablePlayer(CurrentPlayer);

			if (EnemyPlayer.HasConditions())
				EnablePlayer(EnemyPlayer);

			hasConditions = localPlayer.HasConditions() || EnemyPlayer.HasConditions();
		}
	}
	IEnumerator StartChangingPhases()
	{
		phaseTitle.ChangePhase(CurrentPhase.GetPhaseType(), CurrentPlayer == localPlayer);

		yield return AwaitPhaseChange();

		Debug.LogWarning("New Phase: " + CurrentPhase.GetPhaseType().ToString() + " player: " + CurrentPlayer.name);
	}
	IEnumerator AwaitPhaseChange()
	{
		while (phaseTitle.IsChanging)
			yield return null;
	}
	IEnumerator AwaitToSolvePhase(Phase phase)
	{
		yield return phase.Resolve(CurrentPlayer, EnemyPlayer);
	}
}
