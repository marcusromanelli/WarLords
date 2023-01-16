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


	public List<Player> Players;
	protected List<MacroComponent> Macros;

	public Player currentPlayer;
	public Phase currentPhase;
	public bool MatchHasStarted;
	public static bool isExecutingMacro;


	private Phase nextPhase;
	private bool isChangingPhase;




	void Start()
	{
		initializeList();
		StartCoroutine("waitForDeck");

	}

	IEnumerator waitForDeck()
	{
		foreach (Player player in Players)
		{
			player.Setup();
			player.StartGame();
		}

		var player1 = Players[0];
		var player2 = Players[1];

		while (!player1.IsDeckFull() && !player2.IsDeckFull())
		{
			yield return null;
		}


		while (!player1.IsDeckFull() && !player2.IsDeckFull())
		{
			yield return null;
		}

		foreach (Player player in Players)
		{
			player.DrawCard(GameConfiguration.numberOfInitialDrawnCards);
			player.AddCondition(ConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);
		}

		var hasConditions = true;
        while (hasConditions)
        {
			yield return null;
			hasConditions = player1.hasConditions() || player2.hasConditions();
		}

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
		if (Players[0].GetCurrentLife() <= 0)
		{
			PhasesTitle.setWinner(Players[1]);
			Players[0].enabled = false;
			Players[1].enabled = false;
		}
		else if (Players[1].GetCurrentLife() <= 0)
		{
			PhasesTitle.setWinner(Players[0]);
			Players[0].enabled = false;
			Players[1].enabled = false;
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

	void initializeList()
	{
		if (Players == null)
		{
			Players = new List<Player>();
		}

		if (Macros == null)
		{
			Macros = new List<MacroComponent>();
		}
	}

	void NextTurn(Phase phase = Phase.Draw)
	{
		currentPhase = phase;
		if (currentPlayer == null)
		{
			currentPlayer = Players[0];
			currentPlayer.SetDrawnCard(true);
		}
		else
		{
			currentPlayer = (Players[1]) ? Players[0] : Players[1];
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

			int player = allPlayersOk();
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

	public int allPlayersOk()
	{
		var val = -1;
		foreach (Player player in Players)
		{
			if (player.hasConditions())
			{
				return (int)player.GetCivilization();
			}
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
	enum Actions { Move, Attack }
	IEnumerator doActions(Actions action)
	{
		Players.ForEach(delegate (Player obj) {
			obj.enabled = false;
		});

		List<Hero> heroes = GameObject.FindObjectsOfType<Hero>().ToList();
		heroes.RemoveAll(a => a.player != currentPlayer);

		foreach (Hero hero in heroes)
		{
			if (hero != null)
			{
				switch (action)
				{
					case Actions.Attack:
						hero.Attack();
						while (hero.isAttacking)
						{
							yield return null;
						}
						break;
					case Actions.Move:
						hero.moveForward();
						while (hero.isWalking)
						{
							yield return null;
						}
						break;
				}
				yield return new WaitForSeconds(1f);
			}
		}

		Players.ForEach(delegate (Player obj) {
			obj.enabled = true;
		});

		yield return new WaitForSeconds(1);
		NextPhase();
	}

	public void AttackPlayer(int damage)
	{
		Player target;

		//TODO arg
		if (currentPlayer == Players[0])
			target = Players[1];
		else
			target = Players[0];


		target.TakeDamage(damage);

		if (currentPlayer.GetPlayerType() == PlayerType.Remote)
			ScreenController.Blink(new Color(1f, 0.45f, 0.45f, 0.7f));
	}

	public static void SetTriggerType(TriggerType trigger, CardObject hero)
	{
		List<Skill> aux = hero.cardData.hasSkillType(trigger);
		List<Skill> aux2 = hero.cardData.hasSkillType(TriggerType.Passive);

		foreach (Skill auxs in aux)
		{
			GameController.AddMacro(auxs, hero);
		}


		if (trigger != TriggerType.OnBeforeSpawn)
		{
			foreach (Skill auxs in aux2)
			{
				GameController.AddMacro(auxs, hero);
			}
		}
	}

	public static List<MacroComponent> GetMacrosFromPlayer(Player player)
	{
		return Singleton.GetComponents<MacroComponent>().ToList().FindAll(a => a.originalCard.player = player);
	}
	public static void AddMacro(Skill skill, CardObject hero)
	{
		if (!IsMacroActive(hero, skill))
		{
			MacroComponent aux = GameController.Singleton.gameObject.AddComponent<MacroComponent>();
			aux.SetValues(skill, hero);
			GameController.Singleton.Macros.Add(aux);
			if (!GameController.Singleton.Macros[0].isChecking)
			{
				GameController.Singleton.Macros[0].setActive();
			}
		}
	}

	public static bool IsMacroActive(CardObject card, Skill skill)
	{
		return (GameController.Singleton.Macros.FindAll(a => a.originalCard.cardData.PlayID == card.cardData.PlayID && a.Skill.triggerType == skill.triggerType).Count > 0);
	}

	public static void RemoveMacro(MacroComponent condition)
	{
		GameController.Singleton.Macros.Remove(condition);
		if (GameController.Singleton.Macros.Count > 0)
			GameController.Singleton.Macros[0].setActive();
	}


	public static Player getOpponent(Player player)
	{
		if (player.GetCivilization() == Civilization.Aeterna)
		{
			return GameController.Singleton.Players.Find(a => a.GetCivilization() == Civilization.Arkamore);
		}
		else
		{
			return GameController.Singleton.Players.Find(a => a.GetCivilization() == Civilization.Aeterna);
		}
	}

	public static Player GetLocalPlayer()
	{
		return Singleton.Players.Find(a => a.GetPlayerType() == PlayerType.Local);
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
		int count = 0;
		foreach (Player paux in Players)
		{
			if (paux.hasConditions())
			{
				final += paux.GetPlayerType().ToString();

				final += "\n";

				var conditions = paux.GetConditionList();

				foreach (Condition cond in conditions)
				{
					final += cond.getDescription() + "\n";
				}
				count++;
			}
			final += "\n\n";
		}
		if (count > 0)
		{
			final = "Waiting for these actions: \n\n" + final;
			Rect derp = new Rect(Screen.width - 300, 0, 300, 50 + 50 * count);
			GUI.Box(derp, final);
		}

		final = "Current working macros:\n\n";
		foreach (MacroComponent macro in GetComponents<MacroComponent>())
		{
			final += macro.getDescription() + "\n";
		}

		if (GetComponent<MacroComponent>() != null)
		{
			Rect derp = new Rect(0, 0, 450, 50 * GetComponents<MacroComponent>().Length);
			GUI.Box(derp, final);
		}
	}
}