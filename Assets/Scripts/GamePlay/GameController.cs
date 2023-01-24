﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

public class GameController : Singleton<GameController>
{
	[SerializeField] InputController inputController;
	[SerializeField] Battlefield battlefield;
	[SerializeField] Player LocalPlayer;
	[SerializeField] Player RemotePlayer;
	[SerializeField, ReadOnly] Player currentPlayer;
	[SerializeField, ReadOnly] Phase currentPhase;


	protected List<MacroComponent> Macros;


	public static Phase Phase
    {
        get
        {
			return Instance.currentPhase;
        }
    }
	public static bool MatchHasStarted
	{
		get
		{
			return Phase != Phase.PreGame;
		}
	}
	public static bool isExecutingMacro;

	enum Actions { Move, Attack }



	IEnumerator Start()
	{
		Initialize();

		yield return AwaitForGameStart();
	}

	IEnumerator StartupPlayers()
	{
		LocalPlayer.SetupConditions();
		RemotePlayer.SetupConditions();

		LocalPlayer.SetupPlayDeck();
		RemotePlayer.SetupPlayDeck();

		LocalPlayer.SetupHand();
		RemotePlayer.SetupHand();

		yield return LocalPlayer.IsInitialized();

		yield return RemotePlayer.IsInitialized();

		currentPlayer = LocalPlayer;
	}
	IEnumerator AwaitPreGame()
	{
		var hasConditions = true;
		while (hasConditions)
		{
			yield return null;
			hasConditions = LocalPlayer.HasConditions() || RemotePlayer.HasConditions();
		}
	}
	void StartPreGame() {
		LocalPlayer.TryDrawCards(GameConfiguration.numberOfInitialDrawnCards);
		LocalPlayer.AddCondition(MandatoryConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);

		RemotePlayer.TryDrawCards(GameConfiguration.numberOfInitialDrawnCards);
		RemotePlayer.AddCondition(MandatoryConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);
	}
	IEnumerator AwaitForGameStart()
	{
		yield return StartupPlayers();

		StartPreGame();

		yield return AwaitPreGame();

		StartGame();
	}
	void Update()
	{
		WatchExitGame();

		WatchEndGame();
	}
	void WatchEndGame()
	{
		/*if (!MatchHasStarted)
			return;

		//TODO ARRRRG
		if (LocalPlayer.GetCurrentLife() <= 0)
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
	void StartGame()
	{
		NextTurn(Phase.Draw);
	}

	void Initialize()
	{
		if (Macros == null)
			Macros = new List<MacroComponent>();

		currentPhase = Phase.PreGame;

		LocalPlayer.Setup(inputController);
		RemotePlayer.Setup(inputController);
	}

	void NextTurn(Phase phase = Phase.Draw)
	{
		/*currentPhase = phase;
		if (currentPlayer == null)
		{
			currentPlayer = LocalPlayer;
			currentPlayer.SetDrawnCard(true);
		}
		else
		{
			currentPlayer = (RemotePlayer) ? LocalPlayer : RemotePlayer;
		}

		currentPlayer.StartTurn();

		Debug.Log("Turno do jogador: " + currentPlayer + " começando na fase: " + currentPhase.ToString());*/
	}
	public void NextPhase()
	{
		/*if (isChangingPhase)
			return;

		if (currentPhase == Phase.End)
		{

			if (currentPlayer.GetCurrentHandNumber() > GameConfiguration.maxNumberOfCardsInHand)
			{
				if (!currentPlayer.hasCondition(ConditionType.DiscartCard))
				{
					currentPlayer.AddCondition(ConditionType.DiscartCard, (currentPlayer.GetCurrentHandNumber() - GameConfiguration.maxNumberOfCardsInHand));
				}
				Debug.LogWarning("Player " + currentPlayer + " have " + currentPlayer.GetCurrentHandNumber() + " cards in his hand. He can have at maximum " + GameConfiguration.maxNumberOfCardsInHand);
				return;
			}
			else
			{
				NextTurn();
			}
		}
		else
		{

			int player = AllPlayersOk();
			if (player < 0)
			{
				nextPhase = (Phase)((int)currentPhase) + 1;
				if (Enum.GetNames(typeof(Phase)).Length < ((int)currentPhase + 2))
				{
					NextTurn();
				}
				else
				{
					if (!isChangingPhase)
					{
						StartCoroutine(startChangingPhases());
					}
				}
			}
			else
			{
				Debug.LogWarning("Cannot change phase. Waiting for Player " + player + " to finish it's conditions");
			}
		}*/
	}

	public int AllPlayersOk()
	{
		/*var val = -1;

		if (LocalPlayer.HasConditions())
		{
			return (int)LocalPlayer.GetCivilization();
		}

		if (RemotePlayer.HasConditions())
		{
			return (int)RemotePlayer.GetCivilization();
		}

		return val;*/
		return 0;
	}

	public void GoToPhase(Phase phase, Player player)
	{
		if (currentPlayer != player)
			return;

		currentPhase = phase;
		StartCoroutine(StartChangePhase());
	}

	public Player GetCurrentPlayer()
	{
		if (MatchHasStarted)
			return currentPlayer;

		return null;
	}

	IEnumerator StartChangePhase()
	{
		PhasesTitle.ChangePhase(currentPhase);

		while (PhasesTitle.isFading)
		{
			yield return null;
		}

		Debug.LogWarning("New Phase: " + currentPhase.ToString());

		EndPhaseChange();
	}

	void EndPhaseChange()
	{
		switch (currentPhase)
		{
			case Phase.Movement:
				StartCoroutine(AwaitMovementPhase());
				break;
			case Phase.Attack:
				StartCoroutine(AwaitAttackPhase());
				break;
			case Phase.End:
				Debug.LogError("Gone to next turn");
				NextPhase();
				break;
		}
	}
	public void DisablePlayers()
	{
		TogglePlayers(false);
	}
	public void EnablePlayers()
	{
		TogglePlayers(true);
	}
	void TogglePlayers(bool value)
	{
		LocalPlayer.enabled = value;
		RemotePlayer.enabled = value;
	}

	IEnumerator AwaitMovementPhase()
	{
		yield return battlefield.MovementPhase();

		NextPhase();
	}
	IEnumerator AwaitAttackPhase()
	{
		yield return battlefield.AttackPhase();

		NextPhase();
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

	public void AttackPlayer(int damage)
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

	public void SetTriggerType(TriggerType trigger, CardObject cardObject)
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

	public List<MacroComponent> GetMacrosFromPlayer(Player player)
	{
		return Macros.FindAll(macro => macro.GetPlayer() == player);
	}
	public void AddMacro(Skill skill, CardObject hero)
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

	public bool IsMacroActive(CardObject card, Skill skill)
	{
		return false;
		//return Macros.FindAll(macro => macro.GetCardObject().GetCardData().PlayID == card.GetCardData().PlayID && macro.GetSkill().triggerType == skill.triggerType).Count > 0;
	}

	public static void RemoveMacro(MacroComponent condition)
	{
		/*Singleton.Macros.Remove(condition);
		if (Singleton.Macros.Count > 0)
			Singleton.Macros[0].setActive();*/
	}


	public Player GetOpponent(Player player)
	{
		return player == LocalPlayer ? RemotePlayer : LocalPlayer;
	}

	public Player GetLocalPlayer()
	{
		return LocalPlayer;
	}
	public Player GetRemotePlayer()
	{
		return RemotePlayer;
	}

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
		return GetPlayerConditionPrint(LocalPlayer) + GetPlayerConditionPrint(RemotePlayer);
	}

	int GetPlayerConditionsNumber(Player player)
	{
		return player.GetConditions().Count;
	}

	int GetPlayersConditionsNumber()
	{
		return GetPlayerConditionsNumber(LocalPlayer) + GetPlayerConditionsNumber(RemotePlayer);
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
}