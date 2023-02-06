using UnityEngine;

[CreateAssetMenu(fileName = "CardToMana", menuName = "ScriptableObjects/Habilities/CardToMana", order = 1)]
public class CardToManaHability : HabilityBase
{
    public override bool CanUse(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
    {
		return CanGenerateMana(getHabilityManager, getPlayer);
	}
	bool CanGenerateMana(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
	{
		var player = getPlayer();
		var habilityManager = getHabilityManager();

		var hasHabilityOrCondition = player.HasCondition(MandatoryConditionType.SendCardToManaPool) || habilityManager.HasUniqueHability;
		var canHeUse = player.CanInteract() && player.HasManaSpace() && hasHabilityOrCondition;

		return habilityManager.HasInfinityHabilitiesPerTurn || canHeUse;
	}

	public override void Use(GetHabilityManager getHabilityManager, GetPlayer getPlayer)
	{
		var player = getPlayer();

		player.TurnHoldinCardIntoMana();
	}
}
