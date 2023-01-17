using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

	private static GameController _singleton;
	public static GameController Singleton
	{
		get
		{
			if (_singleton == null)
			{
				GameController aux = GameObject.FindObjectOfType<GameController>();
				if (aux == null)
				{
					_singleton = (new GameObject("-----Game Controller-----", typeof(GameController))).GetComponent<GameController>();
				}
				else
				{
					_singleton = aux;
				}
			}
			return _singleton;
		}
	}


	[SerializeField] Player LocalPlayer;
	[SerializeField] Player RemotePlayer;


	protected List<MacroComponent> Macros;

	public Player currentPlayer;
	public Phase currentPhase;
	public bool MatchHasStarted;
	public static bool isExecutingMacro;


	private Phase nextPhase;
	private bool isChangingPhase;

	enum Actions { Move, Attack }



	void Start()
	{
		Initialize();
		StartCoroutine("waitForDeck");

	}

	void StartupPlayers()
	{
		LocalPlayer.Setup();
		LocalPlayer.StartGame();

		RemotePlayer.Setup();
		RemotePlayer.StartGame();

		currentPlayer = LocalPlayer;
	}
	IEnumerator AwaitDeckFull()
	{
		while (!LocalPlayer.IsDeckFull() && !RemotePlayer.IsDeckFull())
		{
			yield return null;
		}
	}
	IEnumerator AwaitConditionsResolve()
	{
		var hasConditions = true;
		while (hasConditions)
		{
			yield return null;
			hasConditions = LocalPlayer.HasConditions() || RemotePlayer.HasConditions();
		}
	}
	void InitializePlayers() {
		LocalPlayer.DrawCard(GameConfiguration.numberOfInitialDrawnCards);
		LocalPlayer.AddCondition(ConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);

		RemotePlayer.DrawCard(GameConfiguration.numberOfInitialDrawnCards);
		RemotePlayer.AddCondition(ConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);
	}
	IEnumerator waitForDeck()
	{
		StartupPlayers();

		yield return AwaitDeckFull();

		InitializePlayers();

		yield return AwaitConditionsResolve();

		StartGame();
	}
	void Update()
	{
		WatchExitGame();

		WatchEndGame();
	}
	void WatchEndGame()
	{
		if (!MatchHasStarted)
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
		}
	}
	void WatchExitGame()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	void StartGame()
	{
		MatchHasStarted = true;
		NextTurn(Phase.Draw);
	}

	void Initialize()
	{
		if (Macros == null)
			Macros = new List<MacroComponent>();
	}

	void NextTurn(Phase phase = Phase.Draw)
	{
		currentPhase = phase;
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

		Debug.Log("Turno do jogador: " + currentPlayer + " começando na fase: " + currentPhase.ToString());
	}
	public void NextPhase()
	{
		if (isChangingPhase)
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
						StartCoroutine("startChangingPhases");
					}
				}
			}
			else
			{
				Debug.LogWarning("Cannot change phase. Waiting for Player " + player + " to finish it's conditions");
			}
		}
	}

	public int AllPlayersOk()
	{
		var val = -1;

		if (LocalPlayer.HasConditions())
		{
			return (int)LocalPlayer.GetCivilization();
		}

		if (RemotePlayer.HasConditions())
		{
			return (int)RemotePlayer.GetCivilization();
		}

		return val;
	}

	public void GoToPhase(Phase phase, Player player)
	{
		if (currentPlayer != player)
			return;

		nextPhase = phase;
		StartCoroutine("startChangingPhases");
	}

	public Player GetCurrentPlayer()
	{
		if (MatchHasStarted)
			return currentPlayer;

		return null;
	}

	IEnumerator startChangingPhases()
	{
		isChangingPhase = true;
		PhasesTitle.ChangePhase(nextPhase);

		while (PhasesTitle.isFading)
		{
			yield return null;
		}

		Debug.LogWarning("New Phase: " + nextPhase.ToString());
		currentPhase = nextPhase;
		isChangingPhase = false;
		finishChangeingPhases();
	}

	void finishChangeingPhases()
	{
		switch (currentPhase)
		{
			case Phase.Action:
				//
				break;
			case Phase.Movement:
				StartCoroutine("doActions", Actions.Move);
				break;
			case Phase.Attack:
				StartCoroutine("doActions", Actions.Attack);
				break;
			case Phase.End:
				Debug.LogError("Troca Fase 1");
				NextPhase();
				break;
		}
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
		RemotePlayer.enabled = value;
	}
	IEnumerator doActions(Actions action)
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
	}

	public void AttackPlayer(int damage)
	{
		Player target;

		//TODO arg
		if (currentPlayer == LocalPlayer)
			target = RemotePlayer;
		else
			target = LocalPlayer;


		target.TakeDamage(damage);

		if (currentPlayer.GetPlayerType() == PlayerType.Remote)
			ScreenController.Blink(new Color(1f, 0.45f, 0.45f, 0.7f));
	}

	public void SetTriggerType(TriggerType trigger, CardObject hero)
	{
		List<Skill> aux = hero.cardData.hasSkillType(trigger);
		List<Skill> aux2 = hero.cardData.hasSkillType(TriggerType.Passive);

		foreach (Skill auxs in aux)
		{
			AddMacro(auxs, hero);
		}


		if (trigger != TriggerType.OnBeforeSpawn)
		{
			foreach (Skill auxs in aux2)
			{
				AddMacro(auxs, hero);
			}
		}
	}

	public List<MacroComponent> GetMacrosFromPlayer(Player player)
	{
		return Macros.FindAll(macro => macro.GetPlayer() == player);
	}
	public void AddMacro(Skill skill, CardObject hero)
	{
		if (!IsMacroActive(hero, skill))
		{
			MacroComponent aux = Singleton.gameObject.AddComponent<MacroComponent>();

			aux.Setup(this, skill, hero);

			Singleton.Macros.Add(aux);

			if (!Singleton.Macros[0].IsResolving)
				Singleton.Macros[0].setActive();
		}
	}

	public bool IsMacroActive(CardObject card, Skill skill)
	{
		return Macros.FindAll(macro => macro.GetCardObject().cardData.PlayID == card.cardData.PlayID && macro.GetSkill().triggerType == skill.triggerType).Count > 0;
	}

	public static void RemoveMacro(MacroComponent condition)
	{
		Singleton.Macros.Remove(condition);
		if (Singleton.Macros.Count > 0)
			Singleton.Macros[0].setActive();
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
			final += player.GetPlayerType().ToString();

			final += "\n";

			var conditions = player.GetConditionList();

			foreach (Condition cond in conditions)
			{
				final += cond.getDescription() + "\n";
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
		return player.GetConditionList().Count;
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