using UnityEngine;
using UnityEngine.UI;

public delegate void OnSkillButtonEnabledClick(SkillData skillData, bool value);

public class EnableSkillButton : MonoBehaviour 
{
	[SerializeField] bool active;
	[SerializeField] Image image;
	[SerializeField] Color green;

	private OnSkillButtonEnabledClick onSkillButtonClick;
	private bool Enabled = false; 
	private SkillData skillData;

	public void Setup(SkillData skillData)
    {
		this.skillData = skillData;
	}
    public void Disable()
    {
        Enabled = false;

		UpdateVisuals();
	}
	public void SetClickCallback(OnSkillButtonEnabledClick onSkillButtonClick)
    {
		this.onSkillButtonClick = onSkillButtonClick;
	}

    public void onClick(){
		if (onSkillButtonClick == null || !active)
			return;

		Enabled = !Enabled;

		onSkillButtonClick?.Invoke(skillData, Enabled);

		GameRules.PlaySFX(GameRules.confirmAction);

		UpdateVisuals();
	}

	void UpdateVisuals()
    {
        if (Enabled)
			image.color = green;
        else
			image.color = Color.white;
	}
}
