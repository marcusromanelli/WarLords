using UnityEngine;
using System;
using System.Collections.Generic;


/*
public int numberAux;
public string stringAux;
public MacroType macro;
public CardObject enemyPlayerCard;
public CardObject currentPlayerCard;
public Player enemyPlayer;
/// <summary>
/// Initializes a new instance of the <see cref="GameActionComponent"/> class.
/// This method is used to log Draw, Discard, CreateEnergys and ChangeCards
/// </summary>
/// <param name="action">GameAction.</param>
/// <param name="number">Number.</param>
public GameActionComponent(GameAction action, Player player, int number = 1)
{
	this.action = action;
	this.numberAux = number;
	this.currentPlayer = player;
}
public GameActionComponent(GameAction action, Player player, string str = "")
{
	this.action = action;
	this.stringAux = str;
	this.currentPlayer = player;
}
public GameActionComponent(GameAction action, string str = "")
{
	this.action = action;
	this.stringAux = str;
}
public GameActionComponent(GameAction action)
{
	this.action = action;
}
public GameActionComponent(GameAction action, Player player, MacroType macro)
{
	this.action = action;
	this.macro = macro;
	this.currentPlayer = player;
}

public GameActionComponent(GameAction action, int number, CardObject currentPlayerCard, CardObject enemyPlayerCard)
{
	this.action = action;
	this.enemyPlayerCard = enemyPlayerCard;
	this.currentPlayerCard = currentPlayerCard;
	this.numberAux = number;
}

public GameActionComponent(GameAction action, CardObject currentPlayerCard)
{
	this.action = action;
	this.currentPlayerCard = currentPlayerCard;
}

public GameActionComponent(GameAction action, int number, Player currentPlayer, Player enemyPlayer)
{
	this.action = action;
	this.currentPlayer = currentPlayer;
	this.enemyPlayer = enemyPlayer;
	this.numberAux = number;
}

public string GetDescription()
{
	StringBuilder str = new StringBuilder("Player ");

	string cardName;
	switch (action)
	{
		case GameAction.PhaseChange:
			str.Append("Next phase: ");
			str.Append(stringAux);
			break;
		case GameAction.TurnChange:
			str.Append("Next turn: ");
			str.Append(currentPlayer.name);
			break;
		case GameAction.DrawCard:
			break;
		case GameAction.DiscartCard:
			break;
		case GameAction.GenerateMana:
			str.Append(currentPlayer.name);
			str.Append(" created ");
			str.Append(numberAux);
			str.Append(" energ" + ((numberAux > 1) ? "ies" : "y"));
			break;
		case GameAction.GraveyardHability:
			str.Append(currentPlayer.name);
			str.Append(" exchanged 2 cards into 1 card");
			break;
		case GameAction.AttackChar:
			break;
		case GameAction.AttackPlayer:
			str.Append(currentPlayer.name);
			str.Append(" enemyPlayer player ");
			str.Append(enemyPlayer.name);
			str.Append(" with ");
			str.Append(numberAux);
			str.Append(" damage");
			break;
		case GameAction.KilledChar:
			cardName = currentPlayerCard.Data.name;

			str.Append(currentPlayer.name);
			str.Append("'s ");
			str.Append(cardName);
			str.Append(" killed ");
			str.Append(enemyPlayer.name);
			str.Append("'s ");
			str.Append(cardName);
			break;
		case GameAction.UseMacro:
			str.Append(currentPlayer.name);
			str.Append(" activated macro ");
			str.Append(macro.ToString());
			break;
	}

	return str.ToString();
}
}*/


public class LogController : MonoBehaviour
{
	static LogController _singleton;
	public static LogController Singleton
	{
		get
		{
			if (_singleton == null)
			{
				LogController aux = GameObject.FindObjectOfType<LogController>();
				if (aux == null)
				{
					_singleton = (new GameObject("----- Log Controller -----", typeof(LogController))).GetComponent<LogController>();
				}
				else
				{
					_singleton = aux;
				}
			}
			return _singleton;
		}
		private set
		{
			_singleton = value;
		}
	}

	List<IGameAction> logs = new List<IGameAction>();

	public static void LogAttack(uint damage, CardObject attacker, IAttackable target)
	{
		var action = AttackAction.Create(damage, attacker, target);
		Singleton.AddLog(action);
	}	
	public static void LogDiscard(Player currentPlayer)
	{
		var action = DiscardCardAction.Create(currentPlayer);
		Singleton.AddLog(action);
	}
	public static void LogManaGeneration(Player currentPlayer)
	{
		var action = ManaGenerateAction.Create(currentPlayer);
		Singleton.AddLog(action);
	}	
	public static void LogCardDrawn(Player currentPlayer, int number)
	{
		var action = DrawCardAction.Create(currentPlayer, number);
		Singleton.AddLog(action);
	}
	public static void LogGraveyardHability(Player currentPlayer, int numberDiscarded, int numberDrawn)
	{
		var action = GraveyardHabilityAction.Create(currentPlayer, numberDiscarded, numberDrawn);
		Singleton.AddLog(action);
	}
	public static void LogTurnChange(Player currentPlayer)
	{
		var action = TurnChangeAction.Create(currentPlayer);
		Singleton.AddLog(action);
	}
	public static void LogPhaseChange(Phase currentPhase)
	{
		var action = PhaseChangeAction.Create(currentPhase);
		Singleton.AddLog(action);
	}
	public static void LogMovement(CardObject currentToken, Vector2 originalPosition, Vector2 targetPosition, float speed)
	{
		var action = MoveAction.Create(currentToken, originalPosition, targetPosition, speed);
		Singleton.AddLog(action);
	}
	public static void LogChangeTarget(CardObject currentToken, IAttackable newTarget)
	{
		var action = ChangeTargetAction.Create(currentToken, newTarget);
		Singleton.AddLog(action);
	}
	public static void LogTokenDestroyed(Player ownerPlayer, CardObject token)
	{
		var action = TokenDestroyedAction.Create(token);
		Singleton.AddLog(action);
	}
	public static void LogElementTookDamage(IAttackable element, uint damage)
	{
		var action = TokenTookDamageAction.Create(element, damage);
		Singleton.AddLog(action);
	}
	public static void LogSummonToken(CardObject token, Vector2 position, uint manaCost)
	{
		var action = SummonTokenAction.Create(token, position, manaCost);
		Singleton.AddLog(action);
	}

	/*public static void Log(GameAction action, Player player, string str)
	{
		Singleton.AddLog(new GameActionComponent(action, player, str));
	}
	public static void Log(GameAction action, Player player, int number = 1)
	{
		Singleton.AddLog(new GameActionComponent(action, player, number));
	}

	public static void Log(GameAction action, Player player, MacroType macro)
	{
		Singleton.AddLog(new GameActionComponent(action, player, macro));
	}

	public static void Log(GameAction action, int number, CardObject currentPlayer, CardObject enemyPlayer)
	{
		Singleton.AddLog(new GameActionComponent(action, number, currentPlayer, enemyPlayer));
	}

	public static void Log(GameAction action, int number, Player currentPlayer, Player enemyPlayer)
	{
		Singleton.AddLog(new GameActionComponent(action, number, currentPlayer, enemyPlayer));
	}

	public static void Log(GameAction action, CardObject currentPlayer)
	{
		Singleton.AddLog(new GameActionComponent(action, currentPlayer));
	}*/

	void AddLog(IGameAction action)
	{
		logs.Add(action);

		Debug.Log(action.GetDescription());
	}
}