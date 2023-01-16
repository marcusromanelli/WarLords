using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ActionComponent{

	public Action action;
	public int number;
	public MacroType macro;
	public Hero attacked;
	public Hero attacker;
	public Player playerAttacker;
	public Player playerAttacked;
	/// <summary>
	/// Initializes a new instance of the <see cref="ActionComponent"/> class.
	/// This method is used to log Draw, Discard, CreateEnergys and ChangeCards
	/// </summary>
	/// <param name="action">Action.</param>
	/// <param name="number">Number.</param>
	public ActionComponent(Action action, Player player, int number=1){
		this.action = action;
		this.number = number;
		this.playerAttacker = player;
	}

	public ActionComponent(Action action, Player player, MacroType macro){
		this.action = action;
		this.macro = macro;
		this.playerAttacker = player;
	}

	public ActionComponent(Action action, int number, Hero attacker, Hero attacked){
		this.action = action;
		this.attacked = attacked;
		this.attacker = attacker;
		this.number = number;
	}

	public ActionComponent(Action action, Hero attacker){
		this.action = action;
		this.attacker = attacker;
	}

	public ActionComponent(Action action, int number, Player attacker, Player attacked){
		this.action = action;
		this.playerAttacker = attacker;
		this.playerAttacked = attacked;
		this.number = number;
	}

	public string getDescription(){
		string final = "";
		switch (action) {
		case Action.DrawCard:
			final = "Player " + playerAttacker.getName() + " drawed " + number + " card" + ((number > 1) ? "s" : "");
			break;
		case Action.DiscartCard:
			final = "Player " + playerAttacker.getName() + " discarted " + number + " card" + ((number > 1) ? "s" : "");
			break;
		case Action.CreateEnergy:
			final = "Player " + playerAttacker.getName() + " created " + number + " energ" + ((number > 1) ? "ies" : "y");
			break;
		case Action.ChangeCard:
			final = "Player " + playerAttacker.getName() + " exchanged 2 cards into 1 card";
			break;
		case Action.AttackChar:
			final = playerAttacker.getName() + "'s "+attacker.card.name+" attacked "+playerAttacked.getName()+"'s "+attacked.card.name+" with "+number+" damage";
			break;
		case Action.AttackPlayer:
			final = "Player " + playerAttacker.getName() + " attacked player "+playerAttacked.getName()+" with "+number+" damage";
			break;
		case Action.KilledChar:
			final = playerAttacker.getName() + "'s "+attacker.card.name+" killed "+playerAttacked.getName()+"'s "+attacked.card.name;
			break;
		case Action.UseMacro:
			final = "Player " + playerAttacker.getName () + " activated macro " + macro.ToString ();
			break;
		}
		return final;
	}
}

public class LogController : MonoBehaviour {
	static LogController _singleton;
	public static LogController Singleton {
		get{
			if (_singleton == null) {
				LogController aux = GameObject.FindObjectOfType<LogController> ();
				if (aux == null) {
					_singleton = (new GameObject ("----- Log Controller -----", typeof(LogController))).GetComponent<LogController> ();
				} else {
					_singleton = aux;
				}
			}
			return _singleton;
		}
		private set{
			_singleton = value;
		}
	}


	List<ActionComponent> log;
	List<Transform> pivots;

	void Start(){
		pivots = GetComponentsInChildren<Transform> ().ToList ();
		pivots.RemoveAll (a => a == transform);
	}

	void Update(){
		if (log == null) {
			log = new List<ActionComponent> ();
		}
	}

	List<ActionComponent> getLog(){
		if (log == null) {
			log = new List<ActionComponent> ();
		}
		return log;
	}

	public static void Log(Action action, Player player, int number=1){
		Singleton.AddLog(new ActionComponent(action, player, number));
	}

	public static void Log(Action action, Player player, MacroType macro){
		Singleton.AddLog(new ActionComponent(action, player,macro));
	}

	public static void Log(Action action, int number, Hero attacker, Hero attacked){
		Singleton.AddLog(new ActionComponent(action, number, attacker, attacked));
	}

	public static void Log(Action action, int number, Player attacker, Player attacked){
		Singleton.AddLog(new ActionComponent(action, number, attacker, attacked));
	}

	public static void Log(Action action, Hero attacker){
		Singleton.AddLog(new ActionComponent(action, attacker));
	}

	void AddLog(ActionComponent action){
		if (getLog().Count == 8) {
			log.RemoveAt (0);
		}
		log.Add (action);
	}
}