using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum ManaStatus
{
	Active, Used, Preview
}

public delegate void OnClickCloseButton();
public delegate CardObjectData OnGetPositionAndRotation();
public class CardObject : MonoBehaviour, IPoolable
{
	[SerializeField] float cardMovementSpeed = 20;
	[SerializeField] float cardRotationSpeed = 20;
	[SerializeField] GameObject CloseButton;
	/*Card originalCardData;
	Card currentCardData;
	bool isBeingHeld;
	public bool isBeingVisualized;
	public bool isBeingHeroVisualized;
	public SummonType SummonType;
	bool isMouseDown;

	public Player player;

	GameObject summonButton, skillButton1, skillButton2, SkillsTree, Status, ;
	public Hero Character;
	public Vector3 originalPosition;*/
	public List<GameObject> cardCoversPerCivilization;
	public bool IsInPosition => isInPosition;

	private Card originalCard;
	private GameObject currentActiveCardCover;
	private Nullable<CardObjectData> targetPositionAndRotation;
	private OnGetPositionAndRotation getPositionAndRotationCallback;
	private bool isInPosition = true;
	private OnClickCloseButton closeCallback;
	/*Vector3 offset;
	Vector3 originalScale;
	Vector3 lastKnownMousePosition;
	Vector3 destinyPosition, destinyScale;
	Quaternion destinyRotation;
	ActionType nextActionType;
	string civilizationName;
	Hero targetHero;*/
	/*InteractiveDeck deckController;
	BaseManaPool manaPoolController;
	InteractiveDeck graveyardController;
	GameController gameController;
	Battlefield battlefield;*/
	//HandController handController;

	public void SetPositionAndRotation(CardObjectData cardData)
	{
		targetPositionAndRotation = cardData;

		isInPosition = false;
	}
	public void SetPositionAndCallback(OnGetPositionAndRotation getPositionAndRotation)
	{
		targetPositionAndRotation = null;
		getPositionAndRotationCallback = getPositionAndRotation;

		isInPosition = false;
	}
	public void RegisterCloseCallback(OnClickCloseButton closeCallback)
	{
		this.closeCallback = closeCallback;
	}
	public void UnregisterCloseCallback()
	{
		this.closeCallback = null;
	}
	public void OnCloseButtonClick ()
    {
		closeCallback?.Invoke();
	}

	

	void MoveToTargetPosition()
    {
		if (getPositionAndRotationCallback != null)
        {
			GoToDynamicTargetPosition();
        }else
			GoToPresetTargetPosition();
	}
	void GoToDynamicTargetPosition()
	{
		var targetPosition = getPositionAndRotationCallback();

		transform.position = targetPosition.Position;
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetPosition.Rotation, Time.deltaTime * cardRotationSpeed);
	}
	void GoToPresetTargetPosition()
    {
		if (isInPosition)
			return;

		transform.position = Vector3.MoveTowards(transform.position, targetPositionAndRotation.Value.Position, Time.deltaTime * cardMovementSpeed);
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetPositionAndRotation.Value.Rotation, Time.deltaTime * cardRotationSpeed);

		if (transform.position == targetPositionAndRotation.Value.Position && transform.localRotation == targetPositionAndRotation.Value.Rotation)
			isInPosition = true;
	}
	void Update()
	{
		MoveToTargetPosition();
	}

    public void SetupCover(Civilization civilization)
    {
		var cardCover = cardCoversPerCivilization[(int)civilization];

		cardCover.SetActive(true);

		currentActiveCardCover = cardCover;
	}

    public void Pool()
    {
		currentActiveCardCover.SetActive(false);
		closeCallback = null;
	}
	public void Setup(Card card)
	{
		originalCard = card;

		LoadCardData();
	}
	private void LoadCardData()
	{
		GameObject cardObj = ElementFactory.LoadResource<GameObject>(GetCardResourcePath(), transform);

		if (cardObj == null)
		{
			Debug.LogError(GetResourceName() + " não invocado. Civ:" + originalCard.civilization.ToString());
			return;
		}

		this.name = GetResourceName();
		cardObj.transform.localPosition = Vector3.zero;
		cardObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}
	string GetCardResourcePath()
	{
		return "Prefabs/Cards/" + originalCard.civilization.ToString() + "/" + GetResourceName();
	}
	string GetResourceName()
	{
		return originalCard.name.Replace(" ", "");
	}

	// void Awake()
	// {
	//	if (transform.Find("Summon"))
	//	{
	//		summonButton = transform.Find("Summon").gameObject;
	//		summonButton.SetActive(false);
	//	}
	//	if (transform.Find("Skill1Button"))
	//	{
	//		skillButton1 = transform.Find("Skill1Button").gameObject;
	//		skillButton1.SetActive(false);
	//	}
	//	if (transform.Find("Skill2Button"))
	//	{
	//		skillButton2 = transform.Find("Skill2Button").gameObject;
	//		skillButton2.SetActive(false);
	//	}
	//	if (transform.Find("Skills"))
	//	{
	//		SkillsTree = transform.Find("Skills").gameObject;
	//		SkillsTree.SetActive(false);
	//	}
	//	if (transform.Find("Status"))
	//	{
	//		Status = transform.Find("Status").gameObject;
	//		Status.SetActive(false);
	//	}
	//	if (transform.Find("CloseButton"))
	//	{
	//		CloseButton = transform.Find("CloseButton").gameObject;
	//		CloseButton.SetActive(false);
	//	}
	// }

	//void Start()
	//{
	//	//originalScale = transform.localScale;

	//}

	//void showSkillTree()
	//{
	//	if (SkillsTree != null)
	//	{
	//		SkillsTree.SetActive(true);
	//		Status.SetActive(true);
	//		CloseButton.SetActive(true);
	//	}
	//}

	//void hideSkillTree()
	//{
	//	if (SkillsTree != null)
	//	{
	//		SkillsTree.SetActive(false);
	//		Status.SetActive(true);
	//		CloseButton.SetActive(true);
	//	}
	//}

	//void showSummonButtons()
	//{
	//	if (summonButton != null)
	//	{
	//		if (Character == null)
	//		{
	//			summonButton.SetActive(true);
	//		}
	//		skillButton1.SetActive(true);
	//		skillButton2.SetActive(true);
	//		CloseButton.SetActive(true);
	//	}
	//}

	//void hideSummonButtons()
	//{
	//	if (summonButton != null)
	//	{
	//		summonButton.SetActive(false);
	//		skillButton1.SetActive(false);
	//		skillButton2.SetActive(false);
	//		CloseButton.SetActive(false);
	//	}
	//}

	/*string GetEmptyCardResourcePath(Civilization civilization)
	{
		return "Prefabs/Cards/" + civilizationName + "/" + GetResourceName();
	}
	

	/*void Update()
	{
		MoveToTargetPosition();
		*switch (SummonType)
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
					transform.position = Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / ((currentCardData.Skills.Count > 2) ? 2.25f : 2), 70, 5f)), Time.deltaTime * cardMovementSpeed);
					transform.localRotation = Quaternion.Euler(-50, 180, 0);
					transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale * 1.5f, Time.deltaTime * 15f);
					showSkillTree();
					showSummonButtons();
				}
				else
				{
					hideSkillTree();
					hideSummonButtons();
					transform.position = Vector3.MoveTowards(transform.position, battlefield.GetTopPosition(), Time.deltaTime * cardMovementSpeed);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, battlefield.GetTopRotation(), Time.deltaTime * 600f);
					hideSummonButtons();
				}

				break;
			case SummonType.DoNotApply:
				if (currentCardData.CardID != 0)
				{
					if (currentCardData.CardID <= 0)
					{
						Destroy(gameObject);
						Debug.LogWarning("Card created empty! Deleting...");
					}
					else
					{
						if (nextActionType == ActionType.NoAction)
						{
							var dist = Vector3.Distance(lastKnownMousePosition, Input.mousePosition);

							if (isMouseDown && dist > 0.2)
							{
								isBeingVisualized = false;
								SetCardBeingHeld(true);
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
											//transform.position = Vector3.MoveTowards(transform.position, selectedTile.GetTopPosition(), Time.deltaTime * cardMovementSpeed);
											//transform.rotation = Quaternion.RotateTowards(transform.rotation, selectedTile.GetTopRotation(), Time.deltaTime * 600f);
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
									player.PreviewSpendMana(CalculateSummonCost());

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
									case ActionType.BuffHero:
										/*GameConfiguration.PlaySFX(GameConfiguration.buffCard);
										var cardObject = targetHero.GetCardObject();
										cardObject.AddSkill(currentCardData.Skills[1]);
										gameController.SetTriggerType(TriggerType.OnBeforeSpawn, cardObject);
										gameController.SetTriggerType(TriggerType.OnAfterSpawn, cardObject);
										targetHero = null;
										Destroy(this.gameObject);*
										break;
									case ActionType.SummonHero:
										/*var hasSummon = player.Summon(this, targetSpawnArea);

                                        if (!hasSummon)
										{
											nextActionType = ActionType.NoAction;
											isBeingHeld = false;
											return;
										}*
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
	}*/

	/*void CheckDeckInteraction()
	{
		if (!deckController.IsMouseOver)
			return;




		destinyPosition = deckController.GetTopPosition();
		destinyRotation = deckController.GetTopRotation();
	}
	void CheckManaPoolInteraction()
	{
		if (!manaPoolController.IsMouseOver)
			return;


		destinyPosition = manaPoolController.GetBasePosition();
		destinyRotation = manaPoolController.GetTopRotation();
	}
	void CheckGraveyardInteraction()
	{
		if (!graveyardController.IsMouseOver)
			return;



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

		if (player.CanSpendMana(currentCardData.Skills[1].manaCost))
		{
			targetHero = Hero.selectedHero;
			player.SpendMana(currentCardData.Skills[1].manaCost);
			nextActionType = ActionType.BuffHero;

			destinyPosition = targetHero.GetPivotPosition();
			destinyRotation = deckController.GetTopRotation();
			return;
		}
	}
	void CheckClickInteraction()
	{
		if (isBeingVisualized || !isBeingHeld)
		{

			if (Vector3.Distance(Input.mousePosition, lastKnownMousePosition) > 0.1f)
				return;

			isBeingVisualized = true;

			handController.SetCardBeingHeld(this);

			return;
		}
	}

	public void OnMouseUp()
	{
		isMouseDown = false;

		CheckDeckInteraction();

		CheckManaPoolInteraction();

		CheckGraveyardInteraction();

		CheckHeroInteraction();

		CheckClickInteraction();

		if(!isBeingVisualized)
			SetCardBeingHeld(false);
	}*/
	//public int CalculateSummonCost()
	//{
	//	return currentCardData.CalculateSummonCost();
	//}

	//public void AddSKill(int number)
	//{
	//	if (number > 1) number = 1;
	//	Skill aux = currentCardData.Skills[number];
	//	aux.isActive = true;
	//	currentCardData.Skills[number] = aux;
	//}

	//public void RemoveSkill(int number)
	//{
	//	if (number > 1) number = 1;
	//	Skill aux = currentCardData.Skills[number];
	//	aux.isActive = false;
	//	currentCardData.Skills[number] = aux;
	//}


	/*ParticleSystem system;
	public void BecameMana()
	{
		system = GetComponentInChildren<ParticleSystem>();
		system.Play();
		system.transform.SetParent(null);
		Destroy(gameObject);
	}
	*/

	//public void SetPlayer(Player player)
	//{
	//	this.player = player;
	//}

	//public void Die()
	//{
	//	//gameController.SetTriggerType(TriggerType.OnBeforeDeath, this);
	//	player.killCard(this);
	//}

	//public void AddSkill(Skill skills)
	//{
	//	Debug.LogWarning("Adicionando skill " + skills.name + " para a carda " + name);
	//	Debug.LogWarning("-----Comecou----");
	//	if (Character != null)
	//	{
	//		Debug.LogWarning("1");
	//		currentCardData.AddSkill(skills);
	//	}
	//	Debug.LogWarning("-----Terminou----");
	//}

	//public void Close()
	//{
	//	if (isBeingHeroVisualized)
	//	{
	//		isBeingHeroVisualized = false;
	//	}
	//	if (isBeingVisualized)
	//	{
	//		player.ResetPreviewMana();
	//		isBeingVisualized = false;
	//       }
	//   }
	/*string GetCharacterResourcePath()
	{
		return "Prefabs/Characters/" + civilizationName + "/" + GetResourceName();
	}
	public Hero GetCharacterResource()
    {
		var path = GetCharacterResourcePath();
		Hero hero = Resources.Load<Hero>(path);

		if (hero == null)
		{
			Debug.LogError("Could not instantiate: " + civilizationName + "/" + path);
			return null;
		}
	
		return hero;
	}
	public void SummonClick()
    {
		SetCardBeingHeld(true);
    }
	void SetCardBeingHeld(bool value)
    {
		if (!isBeingHeld && !value)
			return;

		isBeingHeld = value;

		if(value)
			handController.SetCardBeingHeld(this);
		else
			handController.SetCardBeingHeld(null);
	}
	public void AddAttack(int number)
	{
		if (number < 0) number = 0;
		currentCardData.attack += number;
	}
	public void RemoveAttack(int number)
	{
		if (number < 0) number = 0;
		currentCardData.attack -= number;
	}
	public void AddLife(int number)
	{
		if (number < 0) number = 0;
		DamageCounter.New(transform.position, number);
		currentCardData.life += number;
	}
	public void ResetAttack()
	{
		currentCardData.attack = originalCardData.attack;
	}
	public void ResetLife()
	{
		if (currentCardData.life > originalCardData.life)
			currentCardData.life = originalCardData.life;
	}
	public void DisableSkills()
	{
		Skill aux;
		for (int i = 0; i < currentCardData.Skills.Count; i++)
		{
			aux = currentCardData.Skills[i];
			aux.isActive = false;
			currentCardData.Skills[i] = aux;
		}
	}
	public Card GetCardData()
    {
		return currentCardData;
	}
	public void RemoveLife(int number)
    {
		currentCardData.life -= number;
	}

	public List<Skill> GetSkills()
    {
		return currentCardData.Skills;
    }*/

}