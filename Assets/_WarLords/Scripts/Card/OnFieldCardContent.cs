using UnityEngine;

public class OnFieldCardContent : CardContent
{
	[SerializeField] InGameCardContent inGameCardContent;

	public override void SetData(RuntimeCardData runtimeCardData)
	{
		base.SetData(runtimeCardData);

		inGameCardContent.UpdateData(runtimeCardData);
	}
}