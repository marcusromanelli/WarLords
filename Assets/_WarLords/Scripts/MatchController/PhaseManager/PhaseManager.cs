using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class PhaseManager : MonoBehaviour, IPhaseManager
{
    public delegate void HandleOnPhaseChange(PhaseType newPhase);
    public event HandleOnPhaseChange OnPhaseChange;

    [BoxGroup("Components"), SerializeField] PhaseTitle phaseTitle;
    [BoxGroup("Components"), SerializeField, Expandable] Phase[] phaseCycle;

	[BoxGroup("Gameplay"), SerializeField, ReadOnly] Battlefield battlefield;
	[BoxGroup("Gameplay"), SerializeField, ReadOnly] Player currentPlayer;
	[BoxGroup("Gameplay"), SerializeField, ReadOnly] Player enemyPlayer;

	public Phase CurrentPhase => phaseCycle[currentPhaseIndex];
	public Player CurrentPlayer => currentPlayer;


	private int currentPhaseIndex = 0;
	private Player localPlayer, remotePlayer;
	private bool GameHasEnded = false;


	#region GAMECONTROLLER_INTERFACE
	public void Setup(Player LocalPlayer, Player RemotePlayer, Battlefield Battlefield)
    {
		localPlayer = LocalPlayer;
		remotePlayer = RemotePlayer;
		remotePlayer = RemotePlayer;
		battlefield = Battlefield;

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
		if (currentPlayer == null)
		{
			currentPlayer = localPlayer;
			enemyPlayer = remotePlayer;
		}
		else
		{
			currentPlayer = (currentPlayer == remotePlayer) ? localPlayer : remotePlayer;
			enemyPlayer = (currentPlayer == localPlayer) ? remotePlayer : localPlayer;
		}

		LogController.LogTurnChange(currentPlayer);
	}
	#endregion PHASES_INTERFACE



	bool HasGameEnded()
	{
		if (currentPlayer.GetLife() <= 0)
		{
			phaseTitle.SetWinner(enemyPlayer == localPlayer);
		}

		if (enemyPlayer.GetLife() <= 0)
		{
			phaseTitle.SetWinner(currentPlayer == localPlayer);
		}

		currentPlayer.enabled = false;
		enemyPlayer.enabled = false;

		return currentPlayer.GetLife() <= 0 || enemyPlayer.GetLife() <= 0;
	}
	void InitializePhases()
    {
		foreach (var phase in phaseCycle)
			phase.Setup(this, battlefield);
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

			GameHasEnded = HasGameEnded();

			if (GameHasEnded)
				break;

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
		yield return phase.PrePhase(currentPlayer, enemyPlayer);

		OnPhaseChange?.Invoke(CurrentPhase.GetPhaseType());

		yield return phase.Resolve(currentPlayer, enemyPlayer);
	}
	IEnumerator AwaitConditionsToSolve()
	{
		var hasConditions = true;
		while (hasConditions)
		{
			yield return null;

			if (currentPlayer.HasConditions())
				EnablePlayer(currentPlayer);

			if (enemyPlayer.HasConditions())
				EnablePlayer(enemyPlayer);

			hasConditions = localPlayer.HasConditions() || enemyPlayer.HasConditions();
		}
	}
	IEnumerator StartChangingPhases()
	{
		phaseTitle.ChangePhase(CurrentPhase.GetPhaseType(), currentPlayer == localPlayer);

		yield return AwaitPhaseChange();

		LogController.LogPhaseChange(CurrentPhase);
	}
	IEnumerator AwaitPhaseChange()
	{
		while (phaseTitle.IsChanging)
			yield return null;
	}
	IEnumerator AwaitToSolvePhase(Phase phase)
	{
		yield return phase.Resolve(currentPlayer, enemyPlayer);
	}
	void OnGUI()
	{
		GUIStyle aux = new GUIStyle();
		aux.alignment = TextAnchor.UpperCenter;
		aux.fontSize = 60;
		aux.normal.textColor = Color.white;

		//if (MatchHasStarted) {
		//GUI.Label (new Rect (Screen.width / 2 - 200, 0, 400, 70), "Player " + currentPlayer + "\nPhase: " + currentPhase.ToString (), aux);
		//}

		string final = "\n";
		
		if(CurrentPhase.GetPhaseType() != PhaseType.PreGame)
			final += currentPlayer.name + " turn. \n";

		final += CurrentPhase.GetPhaseType().ToString();

		Rect derp = new Rect(0, 0, 300, 70);
		GUI.Box(derp, final);

		/*final += "Current working macros:\n\n";
		foreach (MacroComponent macro in Macros)
		{
			final += macro.getDescription() + "\n";
		}

		if (Macros.Count > 0)
		{
			Rect derp = new Rect(0, 0, 450, 50 * GetComponents<MacroComponent>().Length);
			GUI.Box(derp, final);
		}*/
	}
}
