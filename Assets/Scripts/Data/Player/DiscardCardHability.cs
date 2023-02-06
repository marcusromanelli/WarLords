
using UnityEngine;

[CreateAssetMenu(fileName = "DiscardCard", menuName = "ScriptableObjects/Habilities/DiscardCard", order = 1)]
public class DiscardCardHability : HabilityBase
{
    public override bool CanUse(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
	{
		return IsFulfullingCondition(getHabilityManager, getPlayer);
	}
    public override void Use(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
	{
		var player = getPlayer();

		player.DiscardCurrentHoldingCard();
	}
	bool IsFulfullingCondition(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
	{
		var player = getPlayer();
		var habilityManager = getHabilityManager();

		return player.HasCondition(MandatoryConditionType.DiscartCard);
	}
}
