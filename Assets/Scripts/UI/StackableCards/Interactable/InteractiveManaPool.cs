using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

/*public class InteractiveManaPool : ManaPool
{
	protected bool isMouseOver;
	public virtual bool IsMouseOver { get { return isMouseOver; } }

	void Awake()
	{
		localPlayerController.OnReleaseCard += OnReleaseCard;
	}
	protected void OnReleaseCard(CardObject releasedCard)
	{
		if (!isMouseOver)
			return;

		if (!GameController.MatchHasStarted)
		{
			if (!localPlayerController.hasCondition(ConditionType.SendCardToManaPool))
			{
				GameConfiguration.PlaySFX(GameConfiguration.denyAction);
				Debug.LogWarning("This movement is not allowed now.");
			}

			return;
		}

		if (GameController.Phase == Phase.Attack || GameController.Phase == Phase.End || GameController.Phase == Phase.Movement)
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("This movement is not allowed in this phase.");
			return;
		}

		if (localPlayerController.HasUsedHability())
		{
			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
			Debug.LogWarning("You already used your hability this turn.");
			return;
		}

		localPlayerController.SendCardToManaPool(releasedCard.GetCardData());
	}
}*/