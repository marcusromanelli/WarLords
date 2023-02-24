using NaughtyAttributes;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardContent : MonoBehaviour
{
	[SerializeField] Canvas canvas;
	[SerializeField] TMP_Text nameText;
	[SerializeField] TMP_Text manaCostText;
	[SerializeField] TMP_Text attackText;
	[SerializeField] TMP_Text defenseText;
	[SerializeField] TMP_Text skill1DescriptionText;
	[SerializeField] TMP_Text skill1CostText;
	[SerializeField] TMP_Text skill2DescriptionText;
	[SerializeField] TMP_Text skill2CostText;
	[SerializeField] Image heroBackground;
	[SerializeField] GameObject closeButton;
	[SerializeField] EnableSkillButton enableSkill1Button;
	[SerializeField] EnableSkillButton enableSkill2Button;

	RuntimeCardData runtimeCardData;
	OnClickCloseButton onClickCloseButton;
	UICardObject uICardObject;

	public void SetData(RuntimeCardData runtimeCardData, OnSkillButtonEnabledClick onSkillButtonClick)
    {
		this.runtimeCardData = runtimeCardData;

		UpdateCardName();
		UpdateManaCost();
		UpdateAttack();
		UpdateDefense();
		UpdateSkills();
		UpdateFrontCardCover();

		enableSkill1Button?.SetClickCallback(onSkillButtonClick);
		enableSkill2Button?.SetClickCallback(onSkillButtonClick);
	}

    void UpdateCardName()
	{
		SetTextValue(nameText, runtimeCardData.Name);
	}
	void UpdateManaCost()
	{
		SetTextValue(manaCostText, runtimeCardData.CalculateSummonCost(false));
	}
	void UpdateAttack()
	{
		SetTextValue(attackText, runtimeCardData.CalculateAttack());
	}
	void UpdateDefense()
	{
		SetTextValue(defenseText, runtimeCardData.CalculateDefense());
	}
	void UpdateSkills()
	{
		var skills = runtimeCardData.OriginalCardSkills;

		var skill1 = skills[0];
		var skill2 = skills[1];

		if (enableSkill1Button != null)
			enableSkill1Button.Setup(skill1);

		SetTextValue(skill1DescriptionText, skill1);
		SetTextValue(skill1CostText, skill1.GetManaCost());

		if (enableSkill2Button != null)
			enableSkill2Button.Setup(skill2);

		SetTextValue(skill2DescriptionText, skill2);
		SetTextValue(skill2CostText, skill2.GetManaCost());
	}
	void SetTextValue(TMP_Text component, object value)
	{
		if (component == null)
			return;

		component.text = value.ToString();
	}
	void UpdateFrontCardCover()
	{
		if (heroBackground == null)
			return;

		heroBackground.sprite = runtimeCardData.FrontCover;
	}
	public void RegisterCloseCallback(OnClickCloseButton closeCallback)
	{
		if (closeButton == null)
			return;

		this.onClickCloseButton = closeCallback;
		closeButton.SetActive(true);
	}
	public void OnCloseButtonClick()
    {
		onClickCloseButton?.Invoke();
	}
	public void Hide()
	{
		canvas.enabled = false;
	}
	public void Show()
	{
		canvas.enabled = true;
	}
}