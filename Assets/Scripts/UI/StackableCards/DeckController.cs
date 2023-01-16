
using UnityEngine;

public class DeckController : StackableCardController
{
	protected override void Update()
	{
		base.Update();
	}
	protected override void OnClick()
    {
		if (GameController.Singleton.currentPhase == Phase.Draw && GameController.Singleton.GetCurrentPlayer() == player)
		{
			if (!player.HasDrawnCard())
			{
				player.SetDrawnCard(true);
				player.DrawCard();

				GameController.Singleton.GoToPhase(Phase.Action, player);
			}
			else
			{
				if (!player.hasCondition(ConditionType.DrawCard) && !Application.isEditor)
				{
					Debug.LogWarning("You already drawn your card this turn");
				}
				else
				{
					player.DrawCard();
				}
			}
		}
		else
		{
			if (!player.hasCondition(ConditionType.DrawCard) && !Application.isEditor)
			{
				GameConfiguration.PlaySFX(GameConfiguration.denyAction);
				Debug.LogWarning("You only draw a card in your Drawn Phase");
			}
			else
			{
				player.DrawCard();
			}
		}
	}

}
