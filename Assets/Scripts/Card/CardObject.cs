using NaughtyAttributes;
using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public delegate void OnClickCloseButton();
public delegate CardPositionData OnGetPositionAndRotation();
public class CardObject : MonoBehaviour, IPoolable
{
	[SerializeField] float cardMovementSpeed = 20;
	[SerializeField] float cardRotationSpeed = 20;
	[SerializeField] float dissolveSpeed = 20;
	[SerializeField] float dissolveMin = 0.7f;

	[BoxGroup("Components"), SerializeField] GameObject cardContents;
	[BoxGroup("Components"), SerializeField] GameObject cardFrame;
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
	[BoxGroup("Components"), SerializeField] EnableSkillButton enableSkill1Button;
	[BoxGroup("Components"), SerializeField] EnableSkillButton enableSkill2Button;


	[BoxGroup("Game"), Expandable, SerializeField] private Card cardData;

	public Card Data => cardData;
	public RuntimeCardData RuntimeCardData => runtimeCardData;

	/*
	GameObject SkillsTree, Status, ;*/

	public bool IsInPosition => isInPosition;
	public bool Interactable => interactable;
	public bool IsVisualizing => isVisualizing;


	private CardPositionData? targetPositionAndRotation;
	private OnGetPositionAndRotation getPositionAndRotationCallback;
	private bool isInPosition = false;
	private OnClickCloseButton closeCallback;
	private bool isBecamingMana = false;
	private Action onManaParticleEnd;
	private Sprite currentCover;
	private Texture currentBackground;
	private float dissolveT = 0;
	private Action onFinishPosition = null;
	private bool interactable = true;
	private Player player;
	private RuntimeCardData runtimeCardData;
	private bool isVisualizing;
	private bool isInvoked;

	public void Setup(Player player, Card card, bool hideInfo)
	{
		this.player = player;
		cardData = card;

		runtimeCardData = new RuntimeCardData(card);

		ToggleSkillButtonsInteraction(true);
		UpdateCardData(hideInfo);
	}
	public void Setup(Card card, bool hideInfo)
	{
		Setup(null, card, hideInfo);
	}
	public void SetInvoked()
    {
		isInvoked = true;

		ToggleSkillButtonsInteraction(!isInvoked);
	}
	public void SetPositionAndRotation(CardPositionData cardData, Action onFinish = null)
	{
		targetPositionAndRotation = cardData;

		isInPosition = false;

		onFinishPosition = onFinish;
	}
	public void SetPositionCallback(OnGetPositionAndRotation getPositionAndRotation)
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
	public void SetVisualizing(bool isVisualizing)
    {
        if (!isVisualizing)
        {
			HideInfo(true);
        }
        else
        {
			ShowInfo();
		}
	}
	public void ShowInfo()
	{
		cardContents.SetActive(true);

		cardFrame.SetActive(true);
		nameText.gameObject.SetActive(true);
		manaCostText.gameObject.SetActive(true);
		attackText.gameObject.SetActive(true);
		defenseText.gameObject.SetActive(true);
		skill1CostText.gameObject.SetActive(true);
		skill1DescriptionText.gameObject.SetActive(true);
		skill2CostText.gameObject.SetActive(true);
		skill2DescriptionText.gameObject.SetActive(true);

		enableSkill1Button.gameObject.SetActive(true);
		enableSkill2Button.gameObject.SetActive(true);

		if (isInvoked)
			return;

		enableSkill1Button.SetClickCallback(OnToggleSkill);
		enableSkill2Button.SetClickCallback(OnToggleSkill);
	}
	public void HideInfo(bool showBackground = false)
	{
		closeButton.SetActive(false);

		if (!isInvoked)
        {
			enableSkill1Button.gameObject.SetActive(false);
			enableSkill2Button.gameObject.SetActive(false);

			ToggleSkillButtonsInteraction(false);

			DisableSkills();
			return;
        }

		if (!showBackground)
		{
			cardContents.SetActive(false);
		}
		else
		{
			cardContents.SetActive(true);

			cardFrame.SetActive(false);
			nameText.gameObject.SetActive(false);
			manaCostText.gameObject.SetActive(false);
			attackText.gameObject.SetActive(false);
			defenseText.gameObject.SetActive(false);
			skill1CostText.gameObject.SetActive(false);
			skill1DescriptionText.gameObject.SetActive(false);
			skill2CostText.gameObject.SetActive(false);
			skill2DescriptionText.gameObject.SetActive(false);
			enableSkill1Button.gameObject.SetActive(false);
			enableSkill2Button.gameObject.SetActive(false);

			ToggleSkillButtonsInteraction(false);
		}
	}
	public void Lock()
	{
		interactable = false;
	}
	public void Unlock()
	{
		interactable = true;
	}
	public void BecameMana(Action onFinishesAnimation)
	{
		isBecamingMana = true;
		cardContents.SetActive(false);
		
		fadeIntoManaParticle.Play();
		onManaParticleEnd = onFinishesAnimation;
		StartCoroutine(AwaitManaParticleEnd());
	}
	public uint CalculateAttack()
	{
		return runtimeCardData.CalculateAttack();
	}
	public uint CalculateLife()
	{
		return runtimeCardData.CalculateDefense();
	}
	public uint CalculateSummonCost()
	{
		return runtimeCardData.CalculateSummonCost();
	}
	public uint CalculateWalkSpeed()
	{
		return runtimeCardData.CalculateWalkSpeed();
	}
	void UpdateCardData(bool hideInfo)
	{
		UpdateBackCardCover();

		if (hideInfo)
		{
			HideInfo();
			cardContents.SetActive(false);
			return;
		}

		cardContents.SetActive(true);
		cardFrame.SetActive(true);
		UpdateCardName();
		UpdateManaCost();
		UpdateAttack();
		UpdateDefense();
		UpdateSkills();
		UpdateFrontCardCover();
	}
	void UpdateCardName()
    {
		nameText.text = runtimeCardData.Name ?? cardData.Name;
	}
	void UpdateManaCost()
    {
		SetTextValue(manaCostText, runtimeCardData?.CalculateSummonCost() ?? cardData.Data.ManaCost);
	}
	void UpdateAttack()
	{
		SetTextValue(attackText, runtimeCardData?.Attack ?? cardData.Data.Attack);
	}
	void UpdateDefense()
	{
		SetTextValue(defenseText, runtimeCardData?.Defense ?? cardData.Data.Defense);
	}
	void UpdateSkills()
	{
		var skill1 = runtimeCardData?.Skills[0] ?? cardData.Data.Skills[0];
		var skill2 = runtimeCardData?.Skills[1] ?? cardData.Data.Skills[1];

		enableSkill1Button.Setup(skill1);
		SetTextValue(skill1DescriptionText, skill1);
		SetTextValue(skill1CostText, skill1.GetManaCost());

		enableSkill2Button.Setup(skill2);
		SetTextValue(skill2DescriptionText, skill2);
		SetTextValue(skill2CostText, skill2.GetManaCost());
	}
	void ToggleSkillButtonsInteraction(bool enabled)
    {
		OnSkillButtonEnabledClick action = enabled ? OnToggleSkill : null;

		enableSkill1Button.SetClickCallback(action);
		enableSkill2Button.SetClickCallback(action);
    }
	void DisableSkills()
	{
		runtimeCardData?.DisableAllSkills();

		enableSkill1Button.Disable();
		enableSkill2Button.Disable();
	}
	void OnToggleSkill(SkillData skill, bool enabled)
	{
		if(isInvoked)
			return;

		runtimeCardData?.ToggleSkill(skill, enabled);
		player?.SummonCostChanged(CalculateSummonCost());
	}
	void UpdateBackCardCover()
	{
		cardRenderer.gameObject.SetActive(true);
		var texture = cardData.Civilization.BackCover;

		if (currentBackground == texture) return;

		cardRenderer.material.SetTexture("_MainTex", texture);

		currentBackground = texture;
	}
	void UpdateFrontCardCover()
	{
		var texture = cardData.FrontCover;

		if (currentCover == texture) return;

		backgroundImageSpriteRenderer.sprite = texture;

		currentCover = texture;
	}

	void SetTextValue(TMP_Text component, object value)
    {
		component.gameObject.SetActive(true);
		component.text = value.ToString();
    }

	void ResetCard()
	{
		DisableSkills();
		ToggleSkillButtonsInteraction(false);
		runtimeCardData = null;
		isVisualizing = false;
		isInvoked = false;
		player = null;
		interactable = true;
		closeCallback = null;
		isBecamingMana = false;
		isInPosition = false;
		targetPositionAndRotation = null;
		cardData = default;
		runtimeCardData = default;
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
		{
			isInPosition = true;
			onFinishPosition?.Invoke();
		}
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
	

	/*void Update()
	{
		MoveToTargetPosition();
		*switch (SummonType)
		{
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

	/*
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