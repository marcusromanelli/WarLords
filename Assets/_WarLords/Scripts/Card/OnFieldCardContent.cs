using UnityEngine;

public class OnFieldCardContent : CardContent
{
	[SerializeField] InGameCardContent inGameCardContent;

	public override void SetData(RuntimeCardData runtimeCardData, OnSkillButtonEnabledClick onSkillButtonClick)
	{
		base.SetData(runtimeCardData, onSkillButtonClick);

		inGameCardContent.UpdateData(runtimeCardData);
	}
}