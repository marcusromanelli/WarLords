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

	[BoxGroup("Components"), SerializeField] Transform physicalCardObject;
	[BoxGroup("Components"), SerializeField] ParticleSystem fadeIntoManaParticle;
	[BoxGroup("Components"), SerializeField] Transform coverBackgroundPivot;
	[BoxGroup("Components"), SerializeField] CardContent defaultCardContentView;
	[BoxGroup("Components"), SerializeField] CardContent hiddenCardContentView;
	[BoxGroup("Components"), SerializeField] CardContent defaultVisualizationCardContentView;
	[BoxGroup("Components"), SerializeField] CardContent onFieldCardContentView;
	[BoxGroup("Components"), SerializeField] CardContent onFieldVisualizationCardContentView;


	public bool IsInPosition => isInPosition;

	private CardObject parentCardObject;
	private CardPositionData? targetLocalPositionAndRotation;
	private CardPositionData? targetPositionAndRotation;
	private OnGetPositionAndRotation getPositionAndRotationCallback;
	private bool isInPosition = false;
	private OnClickCloseButton closeCallback;
	private bool isBecamingMana = false;
	private Action onManaParticleEnd;
	private Action onFinishPosition = null;
	private bool isLocalPlayer;
	private CardContent currentDisplayingContent;
	private GameObject currentBackgroundObject;

	public void Setup(CardObject cardObject, bool isLocalPlayer)
	{
		this.isLocalPlayer = isLocalPlayer;
		parentCardObject = cardObject;

		ToggleSkillButtonsInteraction(true);

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
		currentDisplayingContent.RegisterCloseCallback(closeCallback);
	}
	public void OnCloseButtonClick ()
    {
		closeCallback?.Invoke();
	}
	public void BecameMana(Action onFinishesAnimation)
	{
		isBecamingMana = true;
		currentDisplayingContent.Hide();
		
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
				ShowDefaultVisualization();  //"Default" one, but with skills button activated
				break;
			case CardVisualizationType.OnField:
				ShowOnField();      //On field. Shownig only background image
				break;
			case CardVisualizationType.OnFieldVisualization:
				ShowOnFieldVisualization();  //On field, but being visualized.
				break;
        }
	}
	void DisablePreviousCardContent()
	{
		if (currentDisplayingContent == null)
			return;

		currentDisplayingContent.Hide();
	}
	void SetNewCardContent(CardContent cardContent)
	{
		UpdateBackCardCover();

		cardContent.SetData(parentCardObject.RuntimeCardData, OnToggleSkill);
		cardContent.Show();

		currentDisplayingContent = cardContent;
	}
	void UpdateBackCardCover()
	{
		var obj = parentCardObject.RuntimeCardData.Civilization.GetBackCoverObject();

		if (currentBackgroundObject == obj) return;

		if (currentBackgroundObject != null)
			Destroy(currentBackgroundObject.gameObject);

		currentBackgroundObject = (GameObject)ElementFactory.CreateObject(obj, coverBackgroundPivot);
		currentBackgroundObject.transform.localPosition = Vector3.zero;
		currentBackgroundObject.transform.localRotation = Quaternion.identity;
	}
	void ShowDefault()
	{
		DisablePreviousCardContent();

		SetNewCardContent(defaultCardContentView);
	}
	void ShowHidden()
	{
		DisablePreviousCardContent();

		SetNewCardContent(hiddenCardContentView);
	}
	void ShowDefaultVisualization()
	{
		DisablePreviousCardContent();

		SetNewCardContent(defaultVisualizationCardContentView);
	}
	void ShowOnField()
	{
		DisablePreviousCardContent();

		SetNewCardContent(onFieldCardContentView);
	}
	void ShowOnFieldVisualization()
	{
		DisablePreviousCardContent();

		SetNewCardContent(onFieldVisualizationCardContentView);
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

	void ToggleSkillButtonsInteraction(bool enabled)
    {
		OnSkillButtonEnabledClick action = enabled ? OnToggleSkill : null;
    }
	void UnselectSkillButtons()
	{
		parentCardObject?.UnselectAllSkills();
	}
	void OnToggleSkill(SkillData skill, bool enabled)
	{
		if(parentCardObject.IsInvoked)
			return;

		parentCardObject?.ToggleSkill(skill, enabled);
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

		isBecamingMana = false;
		onManaParticleEnd();
	}
}