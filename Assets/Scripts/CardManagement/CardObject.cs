using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;

public enum ManaStatus
{
	Active, Used, Preview
}

public class CardObject : MonoBehaviour
{

	
	public Card cardData;
	public bool isBeingHeld;
	public bool isBeingVisualized;
	public bool isBeingHeroVisualized;
	public SummonType SummonType;
	public float cardMovementSpeed = 20;
	bool isMouseDown;

	public Player player;

	GameObject summonButton, skillButton1, skillButton2, SkillsTree, Status, CloseButton;
	public Hero Character;
	public Vector3 originalPosition;
	Vector3 offset;
	Vector3 originalScale;
	Vector3 lastKnownMousePosition;
	Vector3 destinyPosition, destinyScale;
	Quaternion destinyRotation;
	ActionType nextActionType;
	string civilizationName;
	Hero targetHero;
	SpawnArea targetSpawnArea;
	DeckController deckController;
	BattlefieldController battlefieldController;
	ManaPoolController manaPoolController;
	GraveyardController graveyardController;
	GameController gameController;
	Battlefield battlefield;
	HandController handController;

	void Awake()
	{
		if (SummonType != SummonType.Mana)
		{
			if (transform.Find("Summon"))
			{
				summonButton = transform.Find("Summon").gameObject;
				summonButton.SetActive(false);
			}
			if (transform.Find("Skill1Button"))
			{
				skillButton1 = transform.Find("Skill1Button").gameObject;
				skillButton1.SetActive(false);
			}
			if (transform.Find("Skill2Button"))
			{
				skillButton2 = transform.Find("Skill2Button").gameObject;
				skillButton2.SetActive(false);
			}
			if (transform.Find("Skills"))
			{
				SkillsTree = transform.Find("Skills").gameObject;
				SkillsTree.SetActive(false);
			}
			if (transform.Find("Status"))
			{
				Status = transform.Find("Status").gameObject;
				Status.SetActive(false);
			}
			if (transform.Find("CloseButton"))
			{
				CloseButton = transform.Find("CloseButton").gameObject;
				CloseButton.SetActive(false);
			}
		}
	}

	void Start()
	{
		originalScale = transform.localScale;

	}

	void showSkillTree()
	{
		if (SkillsTree != null)
		{
			SkillsTree.SetActive(true);
			Status.SetActive(true);
			CloseButton.SetActive(true);
		}
	}

	void hideSkillTree()
	{
		if (SkillsTree != null)
		{
			SkillsTree.SetActive(false);
			Status.SetActive(true);
			CloseButton.SetActive(true);
		}
	}

	void showSummonButtons()
	{
		if (summonButton != null)
		{
			if (Character == null)
			{
				summonButton.SetActive(true);
			}
			skillButton1.SetActive(true);
			skillButton2.SetActive(true);
			CloseButton.SetActive(true);
		}
	}

	void hideSummonButtons()
	{
		if (summonButton != null)
		{
			summonButton.SetActive(false);
			skillButton1.SetActive(false);
			skillButton2.SetActive(false);
			CloseButton.SetActive(false);
		}
	}

	void Update()
	{
		switch (SummonType)
		{
			case SummonType.Mana:

				hideSummonButtons();
				break;
			case SummonType.Monster:
				if (isMouseDown && Vector3.Distance(lastKnownMousePosition, Input.mousePosition) > 0.2)
				{
					isBeingHeroVisualized = false;
				}

				if (isBeingHeroVisualized)
				{
					transform.position = Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / ((cardData.Skills.Count > 2) ? 2.25f : 2), 70, 5f)), Time.deltaTime * cardMovementSpeed);
					transform.localRotation = Quaternion.Euler(-50, 180, 0);
					transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale * 1.5f, Time.deltaTime * 15f);
					showSkillTree();
					showSummonButtons();
				}
				else
				{
					hideSkillTree();
					hideSummonButtons();
					transform.position = Vector3.MoveTowards(transform.position, battlefieldController.GetTopPosition(), Time.deltaTime * cardMovementSpeed);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, battlefieldController.GetTopRotation(), Time.deltaTime * 600f);
					hideSummonButtons();
				}

				break;
			case SummonType.DoNotApply:
				if (cardData.CardID != 0)
				{
					if (cardData.CardID <= 0)
					{
						Destroy(gameObject);
						Debug.LogWarning("Card created empty! Deleting...");
					}
					else
					{
						if (nextActionType == ActionType.NoAction)
						{
							if (isMouseDown && Vector3.Distance(lastKnownMousePosition, Input.mousePosition) > 0.2)
							{
								isBeingHeld = true;
								isBeingVisualized = false;
							}
							lastKnownMousePosition = Input.mousePosition;
							if (isBeingHeld)
							{
								hideSummonButtons();
								offset.z = offset.x = 0;
								if (gameController.GetCurrentPlayer() == player || player.hasCondition(ConditionType.DiscartCard) || player.hasCondition(ConditionType.DrawCard) || player.hasCondition(ConditionType.SendCardToManaPool))
								{
									if (deckController.IsMouseOver)
									{
										transform.position = Vector3.MoveTowards(transform.position, deckController.GetTopPosition(), Time.deltaTime * cardMovementSpeed);
										transform.rotation = Quaternion.RotateTowards(transform.rotation, deckController.GetTopRotation(), Time.deltaTime * 600f);
									}
									else if (manaPoolController.IsMouseOver)
									{
										transform.position = Vector3.MoveTowards(transform.position, manaPoolController.GetBasePosition(), Time.deltaTime * cardMovementSpeed);
										transform.rotation = Quaternion.RotateTowards(transform.rotation, manaPoolController.GetTopRotation(), Time.deltaTime * 600f);
									}
									else if (graveyardController.IsMouseOver)
									{
										transform.position = Vector3.MoveTowards(transform.position, graveyardController.GetTopPosition(), Time.deltaTime * cardMovementSpeed);
										transform.rotation = Quaternion.RotateTowards(transform.rotation, graveyardController.GetTopRotation(), Time.deltaTime * 600f);
									}
									else if (Hero.selectedHero != null)
									{
										transform.position = Vector3.MoveTowards(transform.position, Hero.selectedHero.GetTopPosition(), Time.deltaTime * cardMovementSpeed);
										transform.rotation = Quaternion.RotateTowards(transform.rotation, deckController.GetTopRotation(), Time.deltaTime * 600f);
									}
									else
									{
										var selectedTile = battlefield.GetSelectedTile();

										var hasTile = selectedTile != null;
										var player = gameController.GetCurrentPlayer();

										if (hasTile && (player != null && selectedTile.playerType == player.GetPlayerType()))
										{
											transform.position = Vector3.MoveTowards(transform.position, selectedTile.GetTopPosition(), Time.deltaTime * cardMovementSpeed);
											transform.rotation = Quaternion.RotateTowards(transform.rotation, selectedTile.GetTopRotation(), Time.deltaTime * 600f);
										}
										else
										{
											transform.position = Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(lastKnownMousePosition.x, lastKnownMousePosition.y, 10f)), Time.deltaTime * cardMovementSpeed);
											transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(Vector3.right * 270), Time.deltaTime * handController.cardFoldSpeed);
											transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale, Time.deltaTime * 15f);
										}
									}
								}
								else
								{
									transform.position = Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(lastKnownMousePosition.x, lastKnownMousePosition.y, 10f)), Time.deltaTime * cardMovementSpeed);
									transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(Vector3.right * 270), Time.deltaTime * handController.cardFoldSpeed);
									transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale, Time.deltaTime * 15f);
								}
							}
							else
							{
								if (isBeingVisualized)
								{
									transform.position = Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 70, 5f)), Time.deltaTime * cardMovementSpeed);
									transform.localRotation = Quaternion.Euler(310, 0, 0);
									transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale * 1.5f, Time.deltaTime * 15f);
									player.PreviewSpendMana(cardData.calculateCost());

									showSummonButtons();
								}
								else
								{
									hideSummonButtons();
									//if (transform.localPosition != Vector3.zero) {
									//transform.localPosition = Vector3.MoveTowards (transform.localPosition, originalPosition, Time.deltaTime * cardMovementSpeed);
									//}
									transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale, Time.deltaTime * 15f);
								}
							}
						}
						else
						{
							if (Vector3.Distance(transform.position, destinyPosition) > 0.1f)
							{
								transform.position = Vector3.MoveTowards(transform.position, destinyPosition, Time.deltaTime * cardMovementSpeed);
								transform.rotation = Quaternion.RotateTowards(transform.rotation, destinyRotation, Time.deltaTime * 600f);
								transform.localScale = Vector3.MoveTowards(transform.localScale, destinyScale, Time.deltaTime * 15f);
							}
							else
							{
								switch (nextActionType)
								{
									case ActionType.CardToDeck:
										player.DiscartCardToDrawTwo(cardData);
										break;
									case ActionType.CardToManaPool:
										player.SendCardToManaPool(cardData);
										break;
									case ActionType.DiscartCard:
										player.DiscartCard(cardData);
										break;
									case ActionType.BuffHero:
										GameConfiguration.PlaySFX(GameConfiguration.buffCard);
										var cardObject = targetHero.GetCardObject();
										cardObject.AddSkill(cardData.Skills[1]);
										gameController.SetTriggerType(TriggerType.OnBeforeSpawn, cardObject);
										gameController.SetTriggerType(TriggerType.OnAfterSpawn, cardObject);
										targetHero = null;
										Destroy(this.gameObject);
										break;
									case ActionType.SummonHero:
										var hasSummon = player.Summon(this, targetSpawnArea);

                                        if (!hasSummon)
										{
											nextActionType = ActionType.NoAction;
											isBeingHeld = false;
											return;
										}
										break;
								}
							}
						}
					}
				}
				break;
		}
	}

	public void OnMouseDown()
	{
		if (player.hasCondition(ConditionType.PickSpawnArea) == false && player.GetPlayerType() == PlayerType.Local)
		{
			lastKnownMousePosition = Input.mousePosition;
			offset = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 11)) - transform.localPosition;
			isMouseDown = true;
		}
	}
	
	void CheckDeckInteraction()
    {
		if (!deckController.IsMouseOver)
			return;

		if(gameController.currentPhase == Phase.Attack || gameController.currentPhase == Phase.End || gameController.currentPhase == Phase.Movement || !gameController.MatchHasStarted)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("This movement is not allowed now.");
			return;
		}


		if (player.HasUsedHability())
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("You already used your hability this turn.");
			return;
		}


		nextActionType = ActionType.CardToDeck;
		destinyPosition = deckController.GetTopPosition();
		destinyRotation = deckController.GetTopRotation();
	}
	void CheckManaPoolInteraction()
	{
		if (!manaPoolController.IsMouseOver)
			return;

		if (!gameController.MatchHasStarted)
        {
			if (!player.hasCondition(ConditionType.SendCardToManaPool))
			{
				GameConfiguration.PlaySFX(GameConfiguration.denyAction);
				Debug.LogWarning("This movement is not allowed now.");
			}
			else
			{
				nextActionType = ActionType.CardToManaPool;
				destinyPosition = manaPoolController.GetBasePosition();
				destinyRotation = manaPoolController.GetTopRotation();
			}

			return;
		}

		if (gameController.currentPhase == Phase.Attack || gameController.currentPhase == Phase.End || gameController.currentPhase == Phase.Movement)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("This movement is not allowed in this phase.");
			return;
		}

		if (player.HasUsedHability())
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("You already used your hability this turn.");
			return;
		}

		nextActionType = ActionType.CardToManaPool;
		destinyPosition = manaPoolController.GetBasePosition();
		destinyRotation = manaPoolController.GetTopRotation();
    }
	void CheckGraveyardInteraction()
	{
		if (!graveyardController.IsMouseOver)
			return;

		if (!player.hasCondition(ConditionType.DiscartCard))
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("You cannot discart a cart without a reason.");
			return;
		}

		nextActionType = ActionType.DiscartCard;
		destinyPosition = graveyardController.GetTopPosition();
		destinyRotation = graveyardController.GetTopRotation();
	}
	void CheckHeroInteraction()
    {
		if (Hero.selectedHero == null || isBeingVisualized || !isBeingHeld)
			return;

		if (!gameController.MatchHasStarted)
        {
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("This movement is not allowed now.");
			return;
		}
		
		if (player.CanSpendMana(cardData.Skills[1].manaCost))
		{
			targetHero = Hero.selectedHero;
			player.SpendMana(cardData.Skills[1].manaCost);
			nextActionType = ActionType.BuffHero;

			destinyPosition = targetHero.GetPivotPosition();
			destinyRotation = deckController.GetTopRotation();
			return;
		}
	}
	void CheckBattleFieldInteraction()
    {
		if (battlefield.GetSelectedTile() == null || !battlefield.CanSummonOnSelectedTile(player) || isBeingVisualized || !isBeingHeld)
		{

			if (Vector3.Distance(Input.mousePosition, lastKnownMousePosition) > 0.1f)
				return;

			isBeingVisualized = true;

			return;
		}


		var selectedTile = battlefield.GetSelectedTile();

		if (!gameController.MatchHasStarted)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("This movement is not allowed now.");
			return;
		}


		if (!player.CanSpendMana(cardData.calculateCost()))
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("Not enought mana.");
			return;
		}

		nextActionType = ActionType.SummonHero;

		destinyPosition = selectedTile.GetTopPosition();
		destinyRotation = selectedTile.GetTopRotation();
		targetSpawnArea = selectedTile;
	}
	public void OnMouseUp()
	{
		isMouseDown = false;

		CheckDeckInteraction();

		CheckManaPoolInteraction();

		CheckGraveyardInteraction();

		CheckHeroInteraction();

		CheckBattleFieldInteraction();

		isBeingHeld = false;
	}

	public void AddSKill(int number)
	{
		if (number > 1) number = 1;
		Skill aux = cardData.Skills[number];
		aux.isActive = true;
		cardData.Skills[number] = aux;
	}

	public void RemoveSkill(int number)
	{
		if (number > 1) number = 1;
		Skill aux = cardData.Skills[number];
		aux.isActive = false;
		cardData.Skills[number] = aux;
	}


	ParticleSystem system;
	public void BecameMana()
	{
		system = GetComponentInChildren<ParticleSystem>();
		system.Play();
		system.transform.SetParent(null);
		Destroy(gameObject);
	}


	public void SetPlayer(Player player)
	{
		this.player = player;
	}

	public void Die()
	{
		gameController.SetTriggerType(TriggerType.OnBeforeDeath, this);
		player.killCard(this);
	}

	public void AddSkill(Skill skills)
	{
		Debug.LogWarning("Adicionando skill " + skills.name + " para a carda " + name);
		Debug.LogWarning("-----Comecou----");
		if (Character != null)
		{
			Debug.LogWarning("1");
			cardData.AddSkill(skills);
		}
		Debug.LogWarning("-----Terminou----");
	}

	public void Close()
	{
		if (isBeingHeroVisualized)
		{
			isBeingHeroVisualized = false;
		}
		if (isBeingVisualized)
		{
			player.ResetPreviewMana();
			isBeingVisualized = false;
		}
	}

	public void setCharacterSpawnArea(SpawnArea spawnArea)
	{
		var areaPosition = spawnArea.transform.position;

		GameObject res = Resources.Load<GameObject>("Prefabs/Characters/" + civilizationName + "/" + cardData.name.Replace(" ", ""));
		if (res == null)
		{
			Debug.LogError("Could not instantiate: " + civilizationName + "/" + cardData.name.Replace(" ", ""));
		}

		areaPosition = battlefield.GridToUnity(battlefield.UnityToGrid(areaPosition));

		Character = ((GameObject)Instantiate(res, areaPosition, Quaternion.identity)).AddComponent<Hero>();

		Character.setCard(cardData, player);
		Character.Setup(gameController, battlefield, this);
		GameConfiguration.PlaySFX(GameConfiguration.Summon);
		gameController.SetTriggerType(TriggerType.OnAfterSpawn, this);
	}
	public void Setup(Card card, Player player, Battlefield battlefield, HandController handController, GameController gameController)
	{
		if (cardData.Initialized())
			return;

		this.gameController = gameController;
		this.handController = handController;
		this.battlefield = battlefield;
		civilizationName = player.GetCivilization().ToString();
		deckController = player.GetDeckController();
		battlefieldController = player.GetBattlefieldController();
		manaPoolController = player.GetManaPoolController();
		graveyardController = player.GetGraveyardController();


		cardData = new Card(card.CardID);
		cardData.PlayID = card.PlayID;
		this.player = player;
		GameObject cardObj = Resources.Load<GameObject>("Prefabs/Cards/" + civilizationName + "/" + cardData.name.Replace(" ", ""));
		GameObject aux;

		if (cardObj == null)
		{
			Debug.Log(cardData.name.Replace(" ", "") + " não invocado. Civ:" + civilizationName);
			cardObj = Resources.Load<GameObject>("Prefabs/Cards/Aeterna/AnittaAshervind");
		}

		aux = (GameObject)Instantiate(cardObj, Vector3.zero, Quaternion.identity);
		aux.transform.SetParent(this.transform);
		aux.transform.localPosition = Vector3.zero;
		aux.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}
}