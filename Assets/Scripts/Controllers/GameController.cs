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



	bool waitingForStart;

	public List<Player> Players;
	protected List<MacroComponent> Macros;

	public int currentPlayerNumber = -1;
	public Phase currentPhase;
	public bool MatchHasStarted;
	public static bool isExecutingMacro;




	void Start()
	{
		initializeList();
		StartCoroutine("waitForDeck");

	}

	IEnumerator waitForDeck()
	{
		foreach (Player player in Players)
		{
			player.SetCivilization((Civilization)Players.Count);
			Players.Add(player);
			player.StartGame();
		}

		var player1 = Players[0];
		var player2 = Players[1];

		while (player1.IsDeckFull() && player2.IsDeckFull())
		{
			yield return null;
		}

		foreach (Player player in Players)
		{
			player.DrawCard(GameConfiguration.numberOfInitialDrawnCards);
			player.addCondition(ConditionType.SendCardToManaPool, GameConfiguration.numberOfInitialMana);
		}

		waitingForStart = true;
	}
	void Update()
	{
		if (MatchHasStarted && Players[0].enabled)
		{
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
	}

	void StartGame()
	{
		MatchHasStarted = true;
		nextTurn(Phase.Draw);
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

	void nextTurn(Phase phase = Phase.Draw)
	{
		currentPhase = phase;
		if (currentPlayerNumber == -1)
		{
			currentPlayerNumber = 1;
			Players[currentPlayerNumber].SetDrawnCard(true);
		}
		else
		{
			currentPlayerNumber = (currentPlayerNumber == 1) ? 0 : 1;
		}
		Players[currentPlayerNumber].StartTurn();
		Debug.Log("Turno do jogador: " + (currentPlayerNumber + 1) + " começando na fase: " + currentPhase.ToString());
	}
	Phase next;
	bool isChangingPhase;
	public void nextPhase()
	{
		if (isChangingPhase)
			return;

		if (currentPhase == Phase.End)
		{
			var currentPlayer = Players[currentPlayerNumber];

			if (currentPlayer.GetCurrentHandNumber() > GameConfiguration.maxNumberOfCardsInHand)
			{
				if (!currentPlayer.hasCondition(ConditionType.DiscartCard))
				{
					currentPlayer.addCondition(ConditionType.DiscartCard, (currentPlayer.GetCurrentHandNumber() - GameConfiguration.maxNumberOfCardsInHand));
				}
				Debug.LogWarning("Player " + currentPlayerNumber + " have " + currentPlayer.GetCurrentHandNumber() + " cards in his hand. He can have at maximum " + GameConfiguration.maxNumberOfCardsInHand);
				return;
			}
			else
			{
				nextTurn();
			}
		}
		else
		{

			int player = allPlayersOk();
			if (player < 0)
			{
				next = (Phase)((int)currentPhase) + 1;
				if (Enum.GetNames(typeof(Phase)).Length < ((int)currentPhase + 2))
				{
					nextTurn();
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

	public void goToPhase(Phase phase, Civilization playerNumber)
	{
		if (currentPlayerNumber == ((int)playerNumber))
		{
			next = phase;
			StartCoroutine("startChangingPhases");
		}
	}

	public Player getCurrentPlayer()
	{
		if (MatchHasStarted)
		{
			return Players[currentPlayerNumber];
		}
		return null;
	}
	IEnumerator startChangingPhases()
	{
		isChangingPhase = true;
		PhasesTitle.ChangePhase(next);

		while (PhasesTitle.isFading)
		{
			yield return null;
		}

		Debug.LogWarning("New Phase: " + next.ToString());
		currentPhase = next;
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
				nextPhase();
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
		heroes.RemoveAll(a => a.player != Players[currentPlayerNumber]);
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
		nextPhase();
	}

	public void AttackPlayer(int damage)
	{
		switch (currentPlayerNumber)
		{
			case 0:
				Players[1].doDamage(damage);
				break;
			case 1:
				Players[0].doDamage(damage);
				break;
		}
		if (Players[currentPlayerNumber].GetPlayerType() == PlayerType.Remote)
		{
			ScreenController.Blink(new Color(1f, 0.45f, 0.45f, 0.7f));
		}
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
				if (paux.GetPlayerType() == PlayerType.Remote)
				{
					final += "-----Remote Player:";
				}
				else
				{
					final += "-----You:";
				}
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