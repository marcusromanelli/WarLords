
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "End Phase", menuName = "ScriptableObjects/Phases/End", order = 1)]
public class EndPhase : Phase
{
    public override IEnumerator PrePhase(Player currentPlayer, Player enemyPlayer)
    {
		yield break;
    }

    public override IEnumerator Resolve(Player currentPlayer, Player enemyPlayer)
    {
		var numberOfCardsInHand = currentPlayer.GetHandCardsNumber();

		if (numberOfCardsInHand > GameConfiguration.maxNumberOfCardsInHand)
		{
			var numberOfCardsToDiscard = numberOfCardsInHand - GameConfiguration.maxNumberOfCardsInHand;

			currentPlayer.AddCondition(MandatoryConditionType.DiscartCard, numberOfCardsToDiscard);

			Debug.LogWarning("Player " + currentPlayer + " have " + numberOfCardsInHand + " cards in his hand. He can have at maximum " + GameConfiguration.maxNumberOfCardsInHand);
		}

		yield break;
	}
}