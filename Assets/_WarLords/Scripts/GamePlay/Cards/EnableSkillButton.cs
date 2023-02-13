using UnityEngine;

public delegate void OnSkillButtonEnabledClick(SkillData skillData, bool value);

public class EnableSkillButton : MonoBehaviour 
{
	[SerializeField] new Renderer renderer;
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

    void OnMouseDown(){
		if (onSkillButtonClick == null)
			return;

		Enabled = !Enabled;

		onSkillButtonClick?.Invoke(skillData, Enabled);

		GameConfiguration.PlaySFX(GameConfiguration.confirmAction);

		UpdateVisuals();
	}

	void UpdateVisuals()
    {
        if (Enabled)
			renderer.material.color = green;
        else
			renderer.material.color = Color.white;
	}
}
