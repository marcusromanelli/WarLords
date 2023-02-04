using NaughtyAttributes;
using System;
using UnityEngine;
using TMPro;
using System.Collections;

public delegate void OnClickCloseButton();
public delegate CardPositionData OnGetPositionAndRotation();
public class CardObject : MonoBehaviour, IPoolable
{
	[SerializeField] float cardMovementSpeed = 20;
	[SerializeField] float cardRotationSpeed = 20;
	[SerializeField] float dissolveSpeed = 20;
	[SerializeField] float dissolveMin = 0.7f;

	[BoxGroup("Components"), SerializeField] GameObject cardContents;
	[BoxGroup("Components"), SerializeField] TMP_Text nameText;
	[BoxGroup("Components"), SerializeField] TMP_Text manaCostText;
	[BoxGroup("Components"), SerializeField] TMP_Text attackText;
	[BoxGroup("Components"), SerializeField] TMP_Text defenseText;
	[BoxGroup("Components"), SerializeField] TMP_Text skill1DescriptionText;
	[BoxGroup("Components"), SerializeField] TMP_Text skill1CostText;
	[BoxGroup("Components"), SerializeField] TMP_Text skill2DescriptionText;
	[BoxGroup("Components"), SerializeField] TMP_Text skill2CostText;
	[BoxGroup("Components"), SerializeField] SpriteRenderer backgroundImageSpriteRenderer;
	[BoxGroup("Components"), SerializeField] Renderer cardRenderer;
	[BoxGroup("Components"), SerializeField] ParticleSystem fadeIntoManaParticle;
	[BoxGroup("Components"), SerializeField] GameObject closeButton;


	[BoxGroup("Game"), Expandable, SerializeField] private Card originalCard;

	public Card Data => originalCard;
	/*
	public Player player;

	GameObject summonButton, skillButton1, skillButton2, SkillsTree, Status, ;*/
	public bool IsInPosition => isInPosition;


	private Nullable<CardPositionData> targetPositionAndRotation;
	private OnGetPositionAndRotation getPositionAndRotationCallback;
	private bool isInPosition = false;
	private OnClickCloseButton closeCallback;
	private bool isBecamingMana = false;
	private Action onManaParticleEnd;
	private Sprite currentCover;
	private Texture currentBackground;
	private float dissolveT = 0;

	public void SetPositionAndRotation(CardPositionData cardData)
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
		closeButton.SetActive(true);
	}
	public void UnregisterCloseCallback()
	{
		this.closeCallback = null;
		closeButton.SetActive(false);
	}
	public void OnCloseButtonClick ()
    {
		closeCallback?.Invoke();
	}
	public void Pool()
	{
		ResetCard();
	}
	public void Setup(Card card, bool hideInfo)
	{
		originalCard = card;

		UpdateCardData(hideInfo);
	}
	private void UpdateCardData(bool hideInfo)
	{
		UpdateBackCardCover();

		if (hideInfo)
		{
			cardContents.SetActive(false);
			return;
		}

		cardContents.SetActive(true);
		UpdateCardName();
		UpdateManaCost();
		UpdateAttack();
		UpdateDefense();
		UpdateSkills();
		UpdateFrontCardCover();
	}
	public void BecameMana(Action onFinishesAnimation)
	{
		isBecamingMana = true;
		cardContents.SetActive(false);
		
		fadeIntoManaParticle.Play();
		onManaParticleEnd = onFinishesAnimation;
		StartCoroutine(AwaitManaParticleEnd());
	}
	void UpdateCardName()
    {
		nameText.text = originalCard.Name;
	}
	void UpdateManaCost()
    {
		SetTextValue(manaCostText, originalCard.CalculateSummonCost());
	}
	void UpdateAttack()
	{
		SetTextValue(attackText, originalCard.Data.Attack);
	}
	void UpdateDefense()
	{
		SetTextValue(defenseText, originalCard.Data.Defense);
	}
	void UpdateSkills()
	{
		SetTextValue(skill1DescriptionText, originalCard.Data.Skills[0]);
		SetTextValue(skill1CostText, originalCard.Data.Skills[0].ManaCost);

		SetTextValue(skill2DescriptionText, originalCard.Data.Skills[1]);
		SetTextValue(skill2CostText, originalCard.Data.Skills[1].ManaCost);
	}
	void UpdateBackCardCover()
	{
		cardRenderer.gameObject.SetActive(true);
		var texture = originalCard.Civilization.BackCover;

		if (currentBackground == texture) return;

		cardRenderer.material.SetTexture("_MainTex", texture);

		currentBackground = texture;
	}
	void UpdateFrontCardCover()
	{
		var texture = originalCard.FrontCover;

		if (currentCover == texture) return;

		backgroundImageSpriteRenderer.sprite = texture;

		currentCover = texture;
	}

	void SetTextValue(TMP_Text component, object value)
    {
		component.text = value.ToString();
    }

	void ResetCard()
	{
		closeCallback = null;
		isBecamingMana = false;
		isInPosition = false;
		targetPositionAndRotation = null;
		originalCard = default(Card);
		getPositionAndRotationCallback = null;
		onManaParticleEnd = null;
		dissolveT = 0;
		ResetDissolve();
	}
	void MoveToTargetPosition()
    {
		if (isBecamingMana)
			return;

		if (getPositionAndRotationCallback != null) {
			GoToDynamicTargetPosition();
			return;
        }
		if (!isInPosition && targetPositionAndRotation != null)
		{
			GoToPresetTargetPosition();
		}
	}
	void GoToDynamicTargetPosition()
	{
		var targetPosition = getPositionAndRotationCallback();

		transform.position = targetPosition.Position;
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetPosition.Rotation, Time.deltaTime * cardRotationSpeed);
	}
	void GoToPresetTargetPosition()
    {
		transform.position = Vector3.MoveTowards(transform.position, targetPositionAndRotation.Value.Position, Time.deltaTime * cardMovementSpeed);
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetPositionAndRotation.Value.Rotation, Time.deltaTime * cardRotationSpeed);

		if (transform.position == targetPositionAndRotation.Value.Position && transform.localRotation == targetPositionAndRotation.Value.Rotation)
			isInPosition = true;
	}
	void Update()
	{
		MoveToTargetPosition();
	}
	IEnumerator AwaitManaParticleEnd()
    {
		if (!isBecamingMana)
			yield break;

		while(dissolveT < dissolveMin)
		{
			DissolveCard();
			yield return null;
		}

		isBecamingMana = false;
		onManaParticleEnd();
	}
	void DissolveCard()
	{
		dissolveT += dissolveSpeed * Time.deltaTime;
		dissolveT = Mathf.Clamp(dissolveT, 0, 1);

		SetDissolveCard(dissolveT);
	}
	void ResetDissolve()
    {
		SetDissolveCard(0);
    }
	void SetDissolveCard(float value)
	{
		cardRenderer.material.SetFloat("_DissolveAmount", value);
	}



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

	/*
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

	*/
	

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