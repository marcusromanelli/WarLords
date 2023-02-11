using UnityEngine;

[CreateAssetMenu(fileName = "CardToGraveyard", menuName = "ScriptableObjects/Habilities/CardToGraveyard", order = 1)]
public class CardToGraveyardHability : HabilityBase
{
	[SerializeField] int NumberOfCardsToDraw = 2;

    public override bool CanUse(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
    {
		return IsUsingHability(getHabilityManager, getPlayer);
	}
    public override void Use(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
	{
		var player = getPlayer();

		player.DiscardCurrentHoldingCard();
		player.TryDrawCards(NumberOfCardsToDraw);

		LogController.LogGraveyardHability(player, 1, NumberOfCardsToDraw);
	}
	bool IsUsingHability(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
	{
		var player = getPlayer();
		var habilityManager = getHabilityManager();

		return !player.HasCondition(MandatoryConditionType.DiscartCard) && habilityManager.HasUniqueHability;
	}
}
