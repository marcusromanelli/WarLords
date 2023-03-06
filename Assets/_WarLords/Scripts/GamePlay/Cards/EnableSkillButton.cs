using TMPro;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnSkillButtonEnabledClick(SkillData skillData, bool value);

public class EnableSkillButton : MonoBehaviour 
{
	[SerializeField] Button button;
	[SerializeField] Image image;
	[SerializeField] TMP_Text manaCostLabel;
	[SerializeField] TMP_Text descriptionLabel;
	[SerializeField] Color green;

	private OnSkillButtonEnabledClick onSkillButtonClick;
	private bool _enabled = false; 
	private SkillData skillData;
	private bool active;

	public void Setup(SkillData skillData, OnSkillButtonEnabledClick onSkillButtonClick)
	{
		this.onSkillButtonClick = onSkillButtonClick;
		this.skillData = skillData;

		button.interactable = active;

		manaCostLabel.text = skillData.GetManaCost().ToString();
		descriptionLabel.text = skillData.ToString();
	}
	public void SetActive(bool value)
	{
		active = value;

		button.interactable = active;

		UpdateVisuals();
	}
	public void Enable()
	{
		_enabled = true;

		UpdateVisuals();
	}
	public void Disable()
    {
        _enabled = false;

		UpdateVisuals();
	}

    public void onClick(){
		if (onSkillButtonClick == null || !active)
			return;

		_enabled = !_enabled;

		onSkillButtonClick?.Invoke(skillData, _enabled);

		GameRules.PlaySFX(GameRules.confirmAction);

		UpdateVisuals();
	}

	void UpdateVisuals()
    {
        if (!active)
			return;

        if (_enabled)
			image.color = green;
        else
			image.color = Color.clear;
	}
}
