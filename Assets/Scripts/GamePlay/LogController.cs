using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class ActionComponent
{

	/*public Action action;
	public int number;
	public MacroType macro;
	public HeroObject attacked;
	public HeroObject attacker;
	public Player playerAttacker;
	public Player playerAttacked;
	/// <summary>
	/// Initializes a new instance of the <see cref="ActionComponent"/> class.
	/// This method is used to log Draw, Discard, CreateEnergys and ChangeCards
	/// </summary>
	/// <param name="action">Action.</param>
	/// <param name="number">Number.</param>
	public ActionComponent(Action action, Player player, int number = 1)
	{
		this.action = action;
		this.number = number;
		this.playerAttacker = player;
	}

	public ActionComponent(Action action, Player player, MacroType macro)
	{
		this.action = action;
		this.macro = macro;
		this.playerAttacker = player;
	}

	public ActionComponent(Action action, int number, HeroObject attacker, HeroObject attacked)
	{
		this.action = action;
		this.attacked = attacked;
		this.attacker = attacker;
		this.number = number;
	}

	public ActionComponent(Action action, HeroObject attacker)
	{
		this.action = action;
		this.attacker = attacker;
	}

	public ActionComponent(Action action, int number, Player attacker, Player attacked)
	{
		this.action = action;
		this.playerAttacker = attacker;
		this.playerAttacked = attacked;
		this.number = number;
	}
	*
	public string getDescription()
	{
		StringBuilder str = new StringBuilder("Player ");

		/*string cardName;
		switch (action)
		{
			case Action.DrawCard:
				str.Append(playerAttacker.getName());
				str.Append(" drawed ");
				str.Append(number);
				str.Append(" card");
				str.Append((number > 1) ? "s" : "");
				break;
			case Action.DiscartCard:
				str.Append(playerAttacker.getName());
				str.Append(" discarted ");
				str.Append(number);
				str.Append(" card");
				str.Append((number > 1) ? "s" : "");
				break;
			case Action.CreateEnergy:
				str.Append(playerAttacker.getName());
				str.Append(" created ");
				str.Append(number);
				str.Append(" energ" + ((number > 1) ? "ies" : "y"));
				break;
			case Action.ChangeCard:
				str.Append(playerAttacker.getName());
				str.Append(" exchanged 2 cards into 1 card");
				break;
			case Action.AttackChar:
				cardName = attacker.CardObject.name;

				str.Append(playerAttacker.getName());
				str.Append("'s " + cardName + " attacked ");
				str.Append(playerAttacked.getName());
				str.Append("'s ");
				str.Append(cardName);
				str.Append(" with ");
				str.Append(number);
				str.Append(" damage");
				break;
			case Action.AttackPlayer:
				str.Append(playerAttacker.getName());
				str.Append(" attacked player ");
				str.Append(playerAttacked.getName());
				str.Append(" with ");
				str.Append(number);
				str.Append(" damage");
				break;
			case Action.KilledChar:
				cardName = attacker.CardObject.name;

				str.Append(playerAttacker.getName());
				str.Append("'s ");
				str.Append(cardName);
				str.Append(" killed ");
				str.Append(playerAttacked.getName());
				str.Append("'s ");
				str.Append(cardName);
				break;
			case Action.UseMacro:
				str.Append(playerAttacker.getName());
				str.Append(" activated macro ");
				str.Append(macro.ToString());
				break;
		}*

		return str.ToString();
	}
}

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


	List<ActionComponent> log;
	List<Transform> pivots;

	void Start()
	{
		pivots = GetComponentsInChildren<Transform>().ToList();
		pivots.RemoveAll(a => a == transform);
	}

	void Update()
	{
		if (log == null)
		{
			log = new List<ActionComponent>();
		}
	}

	List<ActionComponent> getLog()
	{
		if (log == null)
		{
			log = new List<ActionComponent>();
		}
		return log;
	}

	public static void Log(Action action, Player player, int number = 1)
	{
		Singleton.AddLog(new ActionComponent(action, player, number));
	}

	public static void Log(Action action, Player player, MacroType macro)
	{
		Singleton.AddLog(new ActionComponent(action, player, macro));
	}

	public static void Log(Action action, int number, HeroObject attacker, HeroObject attacked)
	{
		Singleton.AddLog(new ActionComponent(action, number, attacker, attacked));
	}

	public static void Log(Action action, int number, Player attacker, Player attacked)
	{
		Singleton.AddLog(new ActionComponent(action, number, attacker, attacked));
	}

	public static void Log(Action action, HeroObject attacker)
	{
		Singleton.AddLog(new ActionComponent(action, attacker));
	}

	void AddLog(ActionComponent action)
	{
		if (getLog().Count == 8)
		{
			log.RemoveAt(0);
		}
		log.Add(action);
	}*/
}