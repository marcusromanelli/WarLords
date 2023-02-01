using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;


public class GameController : Singleton<GameController>
{
	public delegate void HandleOnPhaseChange(Phase newPhase);
	public event HandleOnPhaseChange OnPhaseChange;

	public static Player LocalPlayer => Instance.localPlayer;
	public static Player CurrentPlayer => Instance.currentPlayer;


	[BoxGroup("Components"), SerializeField] Battlefield battlefield;
	[BoxGroup("Components"), SerializeField] InputController inputController;
	[BoxGroup("Components"), SerializeField] PhaseTitle phaseTitle;
	[BoxGroup("Components"), SerializeField] Player localPlayer;
	[BoxGroup("Components"), SerializeField] Player remotePlayer;

	[BoxGroup("Gameplay"), SerializeField, ReadOnly] Player currentPlayer;
	[BoxGroup("Gameplay"), SerializeField, ReadOnly] Phase currentPhase;

	List<MacroComponent> Macros;

	public Phase Phase => currentPhase;
	public bool MatchHasStarted => Phase != Phase.PreGame;

	private bool IsChangingPhase;

	IEnumerator Start()
	{
		Initialize();

		yield return SolveCurrentPhase();
	}
	void Initialize()
	{
		if (Macros == null)
			Macros = new List<MacroComponent>();

		battlefield.Setup(inputController, this, CanSummonHero);

		LocalPlayer.Setup(battlefield, this, inputController);

		remotePlayer.Setup(battlefield, this, inputController);

		LocalPlayer.SetupConditions();
		remotePlayer.SetupConditions();
	}

	#region PRE_GAME_PHASE
	IEnumerator ResolvePreGame()
	{
		yield return StartupPlayers();

		StartPreGame();

		SetPhase(Phase.PreGame);

		OnPhaseChange?.Invoke(Phase.PreGame);

		yield return AwaitPreGameConditionsToSolve();
	}
	IEnumerator StartupPlayers()
	{
		LocalPlayer.SetupPlayDeck();
		remotePlayer.SetupPlayDeck();

		yield return LocalPlayer.IsInitialized();

		yield return remotePlayer.IsInitialized();
	}
	void StartPreGame()
	{
		LocalPlayer.TryDrawCards(GameConfiguration.numberOfInitialDrawnCards);
		remotePlayer.TryDrawCards(GameConfiguration.numberOfInitialDrawnCards);

		LocalPlayer.AddCondition(MandatoryConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);
		remotePlayer.AddCondition(MandatoryConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);
	}
	IEnumerator AwaitPreGameConditionsToSolve()
	{
		var hasConditions = true;
		while (hasConditions)
		{
			yield return null;
			hasConditions = LocalPlayer.HasConditions() || remotePlayer.HasConditions();
		}
	}
	#endregion PRE_GAME_PHASE

	#region DRAW_PHASE
	IEnumerator ResolveDrawPhase()
	{
		EnablePlayers();
		currentPlayer.StartDrawPhase();

		OnPhaseChange?.Invoke(Phase.Draw);

		yield return currentPlayer.IsResolvingDrawPhase();
    }
	#endregion ACTION_PHASE

	#region ACTION_PHASE
	IEnumerator ResolveActionPhase()
    {
		currentPlayer.StartActionPhase();

		OnPhaseChange?.Invoke(Phase.Action);

		yield return currentPlayer.IsResolvingActionPhase();
    }
	#endregion ACTION_PHASE

	#region MOVEMENT_PHASE
	IEnumerator ResolveMovementPhase()
    {
		DisablePlayers();

		OnPhaseChange?.Invoke(Phase.Movement);

		yield return battlefield.MovementPhase();
	}
	#endregion MOVEMENT_PHASE

	#region BATTLEFIELD_INTERFACE
	public void Summon(Player player, Card card, SpawnArea spawnArea = null)
	{
		battlefield.Summon(player, card, spawnArea);
	}
	bool CanSummonHero(Card card)
    {
		return LocalPlayer.CanPlayerSummonHero(card);
    }
	#endregion BATTLEFIELD_INTERFACE

	#region PHASE_LOGIC

	IEnumerator SolveCurrentPhase()
	{
		var gameHasEnded = WatchEndGame();

		while (!gameHasEnded)
		{
			switch (currentPhase)
			{
				case Phase.PreGame:
					yield return ResolvePreGame();
					break;
				case Phase.Draw:
					yield return ResolveDrawPhase();
					break;
				case Phase.Action:
					yield return ResolveActionPhase();
					break;
				case Phase.Movement:
					yield return ResolveMovementPhase();
					break;
				case Phase.Attack:
					Debug.Log("Skipping Attack phase");
					//StartCoroutine(AwaitAttackPhase());
					break;
				case Phase.End:
					Debug.Log("Skipping End phase");
					//Debug.LogError("Gone to next turn");
					//GoToNextPhase();
					break;
			}

			gameHasEnded = WatchEndGame();
			yield return GoToNextPhase();
		}


		Debug.Log("Game ended");
		yield break;
	}
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
	void WatchExitGame()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	void SetPhase(Phase phase)
    {
		currentPhase = phase;
    }
	void NextTurn()
	{		
		if (currentPlayer == null)
		{
			currentPlayer = LocalPlayer;
		}
		else
		{
			currentPlayer = (currentPlayer == remotePlayer) ? LocalPlayer : remotePlayer;
		}

		Debug.Log("Turno do jogador: " + currentPlayer + " começando na fase: " + currentPhase.ToString());
	}
	IEnumerator GoToNextPhase()
	{
		Debug.LogWarning("There are players with conditions to solve.");

		yield return AwaitPreGameConditionsToSolve();

		Debug.Log("Going to next phase");

		if(currentPhase == Phase.PreGame)
			NextTurn();
		else if (currentPhase == Phase.End)
			yield return SolveEndPhaseLogic();

		yield return StartChangingPhases();

		SetPhase(GetNextPhase());
	}
	IEnumerator SolveEndPhaseLogic()
    {
		var numberOfCardsInHand = currentPlayer.GetHandCardsNumber();

		if (currentPhase != Phase.End)
			yield break;

		if (numberOfCardsInHand > GameConfiguration.maxNumberOfCardsInHand)
		{
			var numberOfCardsToDiscard = numberOfCardsInHand - GameConfiguration.maxNumberOfCardsInHand;

			currentPlayer.AddCondition(MandatoryConditionType.DiscartCard, numberOfCardsToDiscard);

			Debug.LogWarning("Player " + currentPlayer + " have " + numberOfCardsInHand + " cards in his hand. He can have at maximum " + GameConfiguration.maxNumberOfCardsInHand);

			yield return AwaitPreGameConditionsToSolve();
		}

		NextTurn();
	}
	Phase GetNextPhase()
	{
		if (Phase == Phase.End)
			return Phase.Draw;

		return (Phase)((int)Phase + 1);
    }
	IEnumerator AwaitPhaseChange()
    {
		while (phaseTitle.IsChanging)
		{
			yield return null;
		}
	}
	IEnumerator StartChangingPhases()
	{
		var nextPhase = GetNextPhase();
		phaseTitle.ChangePhase(nextPhase, currentPlayer == LocalPlayer);

		yield return AwaitPhaseChange();		

		Debug.LogWarning("New Phase: " + nextPhase.ToString());
	}
	IEnumerator AwaitMovementPhase()
	{
		yield return battlefield.MovementPhase();

		GoToNextPhase();
	}
	IEnumerator AwaitAttackPhase()
	{
		yield return battlefield.AttackPhase();

		GoToNextPhase();
	}
	#endregion PHASE_LOGIC




	public bool CanPlayerInteract(Player player)
	{
		return Phase == Phase.PreGame || currentPlayer == player;
	}
	void Update()
	{
		WatchExitGame();
	}


	void DisablePlayers()
	{
		TogglePlayers(false);
	}
	void EnablePlayers()
	{
		TogglePlayers(true);
	}
	void TogglePlayers(bool value)
	{
		LocalPlayer.enabled = value;
		remotePlayer.enabled = value;
	}

	void AttackPlayer(int damage)
	{
		/*Player target;

		//TODO arg
		if (currentPlayer == LocalPlayer)
			target = RemotePlayer;
		else
			target = LocalPlayer;


		target.TakeDamage(damage);

		if (currentPlayer.GetPlayerType() == PlayerType.Remote)
			ScreenController.Blink(new Color(1f, 0.45f, 0.45f, 0.7f));*/
	}

	void SetTriggerType(TriggerType trigger, CardObject cardObject)
	{
		//List<Skill> aux = cardObject.GetCardData().hasSkillType(trigger);
		//List<Skill> aux2 = cardObject.GetCardData().hasSkillType(TriggerType.Passive);

		//foreach (Skill auxs in aux)
		//{
		//	AddMacro(auxs, cardObject);
		//}


		//if (trigger != TriggerType.OnBeforeSpawn)
		//{
		//	foreach (Skill auxs in aux2)
		//	{
		//		AddMacro(auxs, cardObject);
		//	}
		//}
	}

	List<MacroComponent> GetMacrosFromPlayer(Player player)
	{
		return Macros.FindAll(macro => macro.GetPlayer() == player);
	}


	/*IEnumerator doActions(Actions action)
	{
		DisablePlayers();

		List<Hero> heroes = GameObject.FindObjectsOfType<Hero>().ToList();
		heroes.RemoveAll(a => a.GetPlayer() != currentPlayer);

		foreach (Hero hero in heroes)
		{
			if (hero != null)
			{
				switch (action)
				{
					case Actions.Attack:
						hero.Attack();
						while (hero.IsAttacking())
						{
							yield return null;
						}
						break;
					case Actions.Move:
						hero.moveForward();
						while (hero.IsWalking())
						{
							yield return null;
						}
						break;
				}
				yield return new WaitForSeconds(1f);
			}
		}

		EnablePlayers();

		yield return new WaitForSeconds(1);
		NextPhase();
	}*/
	void AddMacro(Skill skill, CardObject hero)
	{
		/*if (!IsMacroActive(hero, skill))
		{
			MacroComponent aux = Singleton.gameObject.AddComponent<MacroComponent>();

			aux.Setup(this, skill, hero);

			Singleton.Macros.Add(aux);

			if (!Singleton.Macros[0].IsResolving)
				Singleton.Macros[0].setActive();
		}*/
	}

	bool IsMacroActive(CardObject card, Skill skill)
	{
		return false;
		//return Macros.FindAll(macro => macro.GetCardObject().GetCardData().PlayID == card.GetCardData().PlayID && macro.GetSkill().triggerType == skill.triggerType).Count > 0;
	}

	static void RemoveMacro(MacroComponent condition)
	{
		/*Singleton.Macros.Remove(condition);
		if (Singleton.Macros.Count > 0)
			Singleton.Macros[0].setActive();*/
	}

	public PlayerType GetPlayerType(Player player)
    {
		return player == LocalPlayer ? PlayerType.Local : PlayerType.Remote;
    }

	Player GetOpponent(Player player)
	{
		return player == LocalPlayer ? remotePlayer : LocalPlayer;
	}

	Player GetLocalPlayer()
	{
		return LocalPlayer;
	}
	Player GetRemotePlayer()
	{
		return remotePlayer;
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
		return GetPlayerConditionPrint(LocalPlayer) + GetPlayerConditionPrint(remotePlayer);
	}

	int GetPlayerConditionsNumber(Player player)
	{
		return player.GetConditions().Count;
	}

	int GetPlayersConditionsNumber()
	{
		return GetPlayerConditionsNumber(LocalPlayer) + GetPlayerConditionsNumber(remotePlayer);
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

		final = "Current working macros:\n\n";
		foreach (MacroComponent macro in Macros)
		{
			final += macro.getDescription() + "\n";
		}

		if (Macros.Count > 0)
		{
			Rect derp = new Rect(0, 0, 450, 50 * GetComponents<MacroComponent>().Length);
			GUI.Box(derp, final);
		}
	}
	#endregion HELPER_GUI
}