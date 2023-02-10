using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;


public class GameController : MonoBehaviour
{
	[BoxGroup("Components"), SerializeField] Battlefield battlefield;
	[BoxGroup("Components"), SerializeField] InputController inputController;
	[BoxGroup("Components"), SerializeField] PhaseManager phaseManager;
	[BoxGroup("Components"), SerializeField] Player localPlayer;
	[BoxGroup("Components"), SerializeField] Player remotePlayer;


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

        battlefield.PreSetup(localPlayer, remotePlayer, inputController, this, CanSummonHero);

		localPlayer.PreSetup(battlefield, this, inputController);

		remotePlayer.PreSetup(battlefield, this, inputController);
	}
    private void Update()
    {
		WatchExitGame();
	}
    void WatchExitGame()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


	#region BATTLEFIELD_INTERFACE
	public void Summon(Player player, CardObject cardObject, SpawnArea spawnArea = null)
	{
		battlefield.Summon(player, cardObject, spawnArea);
	}
	bool CanSummonHero(CardObject cardObject)
	{
		return localPlayer.CanPlayerSummonHero(cardObject);
	}
	#endregion BATTLEFIELD_INTERFACE

	public bool CanPlayerInteract(Player player)
	{
		return phaseManager.CanPlayerInteract(player);
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

	/*List<MacroComponent> GetMacrosFromPlayer(Player player)
	{
		return Macros.FindAll(macro => macro.GetPlayer() == player);
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