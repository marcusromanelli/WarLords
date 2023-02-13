using NaughtyAttributes;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UICardObject : MonoBehaviour
{
	enum CardVisualizationType
    {
		Hidden, Default, DefaultVisualization, OnField, OnFieldVisualization
	}

	[SerializeField] float cardMovementSpeed = 20;
	[SerializeField] float cardRotationSpeed = 20;
	[SerializeField] float dissolveSpeed = 20;
	[SerializeField] float dissolveMin = 0.7f;

	[BoxGroup("Components"), SerializeField] Transform physicalCardObject;
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


	public bool IsInPosition => isInPosition;

	private CardObject parentCardObject;
	private CardPositionData? targetLocalPositionAndRotation;
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
	private bool isLocalPlayer;

	public void Setup(CardObject cardObject, bool isLocalPlayer)
	{
		this.isLocalPlayer = isLocalPlayer;
		parentCardObject = cardObject;

		ToggleSkillButtonsInteraction(true);

		SetCardData();

		RefreshCardUI();
	}
	public void SetLocalPositionAndRotation(CardPositionData cardData, Action onFinish = null)
	{
		getPositionAndRotationCallback = null;
		targetPositionAndRotation = null;
		targetLocalPositionAndRotation = cardData;

		isInPosition = false;

		onFinishPosition = onFinish;
	}
	public void SetPositionAndRotation(CardPositionData cardData, Action onFinish = null)
	{
		targetLocalPositionAndRotation = null;
		getPositionAndRotationCallback = null;
		targetPositionAndRotation = cardData;

		isInPosition = false;

		onFinishPosition = onFinish;
	}
	public void SetPositionCallback(OnGetPositionAndRotation getPositionAndRotation)
	{
		targetLocalPositionAndRotation = null;
		targetPositionAndRotation = null;
		getPositionAndRotationCallback = getPositionAndRotation;

		isInPosition = false;
	}
	public void RegisterCloseCallback(OnClickCloseButton closeCallback)
	{
		this.closeCallback = closeCallback;
		closeButton.SetActive(true);
	}
	public void OnCloseButtonClick ()
    {
		closeCallback?.Invoke();
	}
	public void BecameMana(Action onFinishesAnimation)
	{
		isBecamingMana = true;
		cardContents.SetActive(false);
		
		fadeIntoManaParticle.Play();
		onManaParticleEnd = onFinishesAnimation;
		StartCoroutine(AwaitManaParticleEnd());
	}
	public GameObject GetPhysicalCardObject()
    {
		return physicalCardObject.gameObject;
	}
	public Transform GetCurrentActiveObject(){
		var isInvoked = parentCardObject.IsInvoked;

		return isInvoked ? physicalCardObject : transform;
	}
	void SetCardData()
	{
		UpdateBackCardCover();

		UpdateCardName();
		UpdateManaCost();
		UpdateAttack();
		UpdateDefense();
		UpdateSkills();
		UpdateFrontCardCover();
	}
	public void RefreshCardUI()
    {
		CardVisualizationType visualizationType = CardVisualizationType.Default;

		if (!isLocalPlayer)
			visualizationType = CardVisualizationType.Hidden;

		if (parentCardObject.IsInvoked)
        {
			visualizationType = CardVisualizationType.OnField;

			if (parentCardObject.IsVisualizing)
				visualizationType = CardVisualizationType.OnFieldVisualization;				
        }else if (parentCardObject.IsVisualizing)
			visualizationType = CardVisualizationType.DefaultVisualization;


		RefreshCardUI(visualizationType);
	}
	void RefreshCardUI(CardVisualizationType type)
    {
		switch (type)
        {
			case CardVisualizationType.Default:
				ShowDefault();   //Default on-game card display. Aka sitting on hand field
				break;
			case CardVisualizationType.Hidden:
				ShowHidden();         //The default card appearance. Showing only the cover
				break;
			case CardVisualizationType.DefaultVisualization:
				ShowDefaultWithSkills(true);  //"Default" one, but with skills button activated
				break;
			case CardVisualizationType.OnField:
				ShowOnField();      //On field. Shownig only background image
				break;
			case CardVisualizationType.OnFieldVisualization:
				ShowFieldVisualization();  //On field, but being visualized.
				break;
        }
	}
	void ShowOnField()
	{
		ShowHidden();

		cardContents.SetActive(true);
	}
	void ShowHidden()
	{
		cardContents.SetActive(false);
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
	void ShowDefault()
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

		UnselectSkillButtons();

		enableSkill1Button.gameObject.SetActive(false);
		enableSkill2Button.gameObject.SetActive(false);

		ToggleSkillButtonsInteraction(false);
	}
	void ShowFieldVisualization()
    {
		ShowDefaultWithSkills(false);
    }
	void ShowDefaultWithSkills(bool skillsInteractable)
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

		ToggleSkillButtonsInteraction(skillsInteractable);
	}
	public void DetachPhsyicalCard()
	{
		physicalCardObject.transform.SetParent(null, true);
	}
	public void AttachPhsyicalCard(Transform transform)
	{
		physicalCardObject.transform.SetParent(transform, true);
	}
	public void DettachPhsyicalCard()
	{
		physicalCardObject.transform.SetParent(null, true);
	}
	void UpdateCardName()
    {
		nameText.text = parentCardObject.GetCardName();
	}
	void UpdateManaCost()
    {
		SetTextValue(manaCostText, parentCardObject.CalculateSummonCost(false));
	}
	void UpdateAttack()
	{
		SetTextValue(attackText, parentCardObject.CalculateAttack());
	}
	void UpdateDefense()
	{
		SetTextValue(defenseText, parentCardObject.CalculateLife());
	}
	void UpdateSkills()
	{
		var skills = parentCardObject.GetSkillList();

		var skill1 = skills[0];
		var skill2 = skills[1];

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
	void UnselectSkillButtons()
	{
		parentCardObject.UnselectAllSkills();

		enableSkill1Button.Disable();
		enableSkill2Button.Disable();
	}
	void OnToggleSkill(SkillData skill, bool enabled)
	{
		if(parentCardObject.IsInvoked)
			return;

		parentCardObject?.ToggleSkill(skill, enabled);
	}
	void UpdateBackCardCover()
	{
		var cardData = parentCardObject.Data;
		cardRenderer.gameObject.SetActive(true);
		var texture = cardData.Civilization.BackCover;

		if (currentBackground == texture) return;

		cardRenderer.material.SetTexture("_MainTex", texture);

		currentBackground = texture;
	}
	void UpdateFrontCardCover()
	{
		var cardData = parentCardObject.Data;
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

	public void ResetUI()
	{
		UnselectSkillButtons();
		ToggleSkillButtonsInteraction(false);

		closeCallback = null;
		isBecamingMana = false;
		isInPosition = false;
		targetLocalPositionAndRotation = null;
		targetPositionAndRotation = null;
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
		if (!isInPosition && targetLocalPositionAndRotation != null)
		{
			GoToPresetTargetLocalPosition();
		}
	}
	void GoToDynamicTargetPosition()
	{
		var targetPosition = getPositionAndRotationCallback();

		Transform opjTransform = GetCurrentActiveObject();

		opjTransform.position = targetPosition.Position;
		opjTransform.localRotation = Quaternion.RotateTowards(opjTransform.localRotation, targetPosition.Rotation, Time.deltaTime * cardRotationSpeed);
	}
	void GoToPresetTargetLocalPosition()
	{
		Transform opjTransform = GetCurrentActiveObject();

		opjTransform.localPosition = Vector3.MoveTowards(opjTransform.localPosition, targetLocalPositionAndRotation.Value.Position, Time.deltaTime * cardMovementSpeed);
		opjTransform.localRotation = Quaternion.RotateTowards(opjTransform.localRotation, targetLocalPositionAndRotation.Value.Rotation, Time.deltaTime * cardRotationSpeed);

		if (opjTransform.localPosition == targetLocalPositionAndRotation.Value.Position && opjTransform.localRotation == targetLocalPositionAndRotation.Value.Rotation)
		{
			isInPosition = true;
			onFinishPosition?.Invoke();
		}
	}
	void GoToPresetTargetPosition()
	{
		Transform opjTransform = GetCurrentActiveObject();

		opjTransform.position = Vector3.MoveTowards(opjTransform.position, targetPositionAndRotation.Value.Position, Time.deltaTime * cardMovementSpeed);
		opjTransform.localRotation = Quaternion.RotateTowards(opjTransform.localRotation, targetPositionAndRotation.Value.Rotation, Time.deltaTime * cardRotationSpeed);

		if (opjTransform.position == targetPositionAndRotation.Value.Position && opjTransform.localRotation == targetPositionAndRotation.Value.Rotation)
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
}