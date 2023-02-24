using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;


public class GameController : Singleton<GameController>
{
	[BoxGroup("Components"), SerializeField] DataReferenceLibrary dataReferenceLibrary;
	[BoxGroup("Components"), SerializeField] Battlefield battlefield;
	[BoxGroup("Components"), SerializeField] InputController inputController;
	[BoxGroup("Components"), SerializeField] PhaseManager phaseManager;
	[BoxGroup("Components"), SerializeField] Player localPlayer;
	[BoxGroup("Components"), SerializeField] Player remotePlayer;


	public PhaseManager PhaseManager => phaseManager;

	//List<MacroComponent> Macros;

	void Start()
	{
		Initialize();

		phaseManager.StartCycle();
	}
	void Initialize()
    {
        //if (Macros == null)
        //			Macros = new List<MacroComponent>();

        phaseManager.Setup(localPlayer, remotePlayer, battlefield);

        battlefield.PreSetup(localPlayer, remotePlayer, inputController, this, CanSummonToken);

		localPlayer.PreSetup(battlefield, this, inputController, dataReferenceLibrary);

		remotePlayer.PreSetup(battlefield, this, inputController, dataReferenceLibrary);
	}
    void Update()
    {
		WatchExitGame();
	}
    void WatchExitGame()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	void setPlayersDeck(UserDeck localDeckData, UserDeck remoteDeckData)
	{
		localPlayer.SetDeck(localDeckData);
		remotePlayer.SetDeck(remoteDeckData);
	}

	public static void SetPlayersDeck(UserDeck localDeckData, UserDeck remoteDeckData)
    {
		Instance.setPlayersDeck(localDeckData, remoteDeckData);
    }

	#region BATTLEFIELD_INTERFACE
	public void Summon(Player player, CardObject cardObject, SpawnArea spawnArea = null)
	{
		battlefield.Summon(player, cardObject, spawnArea);
	}
	bool CanSummonToken(CardObject cardObject, bool isSkillOnly)
	{
		return localPlayer.CanPlayerSummonToken(cardObject, isSkillOnly);
	}
	#endregion BATTLEFIELD_INTERFACE

	public bool CanPlayerInteract(Player player)
	{
		return phaseManager.CanPlayerInteract(player);
	}

	#region HELPER_GUI
	string GetPlayerConditionPrint(Player player)
    {
		string final = "";

		if (player.HasConditions())
		{
			final += player.name;

			final += "\n";

			var conditions = player.GetConditions();

			foreach (MandatoryCondition cond in conditions)
			{
				final += cond.GetDescription() + "\n";
			}
		}
		final += "\n\n";

		return final;
	}

	string GetPlayersConditionsPrint()
	{
		return GetPlayerConditionPrint(localPlayer) + GetPlayerConditionPrint(remotePlayer);
	}

	int GetPlayerConditionsNumber(Player player)
	{
		return player.GetConditions().Count;
	}

	int GetPlayersConditionsNumber()
	{
		return GetPlayerConditionsNumber(localPlayer) + GetPlayerConditionsNumber(remotePlayer);
	}

	void OnGUI()
	{
		GUIStyle aux = new GUIStyle();
		aux.alignment = TextAnchor.UpperCenter;
		aux.fontSize = 20;
		aux.normal.textColor = Color.white;

		//if (MatchHasStarted) {
		//GUI.Label (new Rect (Screen.width / 2 - 200, 0, 400, 70), "Player " + currentPlayer + "\nPhase: " + currentPhase.ToString (), aux);
		//}

		string final = "";

		
		int count = GetPlayersConditionsNumber();

		final += GetPlayersConditionsPrint();

		if (count > 0)
		{
			final = "Waiting for these actions: \n\n" + final;
			Rect derp = new Rect(Screen.width - 300, 0, 300, 50 + 50 * count);
			GUI.Box(derp, final);
		}

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
	#endregion HELPER_GUI
}