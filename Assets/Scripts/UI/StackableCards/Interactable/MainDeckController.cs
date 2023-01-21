using UnityEngine;

public class MainDeckController : InteractiveDeck
{
	protected void Awake()
	{
		base.Awake();
	}

	protected override void OnReleaseCard(CardObject cardObject)
	{
		//if (!isMouseOver)
		//	return;

		//if (GameController.Phase != Phase.Action && GameController.Phase != Phase.Draw) 
		//{
		//	GameConfiguration.PlaySFX(GameConfiguration.denyAction);
		//	Debug.LogWarning("This movement is not allowed now.");
		//	return;
		//}


		//if (Player.HasUsedHability())
		//{
		//	GameConfiguration.PlaySFX(GameConfiguration.denyAction);
		//	Debug.LogWarning("You already used your hability this turn.");
		//	return;
		//}

		//Player.DiscartCardToDrawTwo(cardObject.GetCardData());
	}
}
