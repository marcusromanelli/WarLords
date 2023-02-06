using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class HabilityManager : MonoBehaviour
{
	[SerializeField] MandatoryConditionManager mandatoryConditionManager;
	[SerializeField] bool hasInfinityHabilitiesPerTurn;
	[SerializeField, Expandable] List<HabilityBase> allowedHabilities = new List<HabilityBase> ();

	public bool HasInfinityHabilitiesPerTurn => hasInfinityHabilitiesPerTurn;
	public bool HasUniqueHability => hasUniqueHability;

	private bool hasUniqueHability = false;
	private Player player;

	public void Setup(Player player)
    {
		this.player = player;

		this.player.OnStartActionPhase += OnStartActionPhase;
		this.player.OnCardReleasedOnGraveyard += OnReleasedOnGraveyard;
		this.player.OnCardReleasedOnManaPool += OnReleasedOnManaPool;
	}
	protected void OnStartActionPhase()
    {
		RestoreUniqueHability();
	}
	protected void OnReleasedOnManaPool(CardObject cardObject)
    {
		UseManaPoolHability();
    }
	protected void OnReleasedOnGraveyard(CardObject cardObject)
    {
		UseGraveyardHability();
	}
	void RestoreUniqueHability()
	{
		hasUniqueHability = true;
	}
	bool CanUseAnyHability()
	{
		return HasCondition() || (player.IsOnActionPhase && hasUniqueHability);
	}
	void UseManaPoolHability()
	{
		UseHability(HabilityTrigger.OnManaPoolRelease);
	}
	void UseGraveyardHability()
	{
		UseHability(HabilityTrigger.OnGreaveyardRelease);
	}
	void UseHability(HabilityTrigger triggerType)
	{
		if (!HasAnyHability() || !CanUseAnyHability())
			return;

		HabilityManager getHabilityManager() => this;
		Player getPlayer() => player;

		foreach (var hability in allowedHabilities)
			if (hability.Trigger == triggerType && hability.CanUse(getHabilityManager, getPlayer))
				if (!hability.IsUnique || hasUniqueHability)
				{
					hability.Use(getHabilityManager, getPlayer);
					hasUniqueHability = false;
				}
	}
	bool HasCondition()
    {
		return mandatoryConditionManager.HasAny();
	}
	bool HasAnyHability()
    {
		return allowedHabilities.Count > 0;
    }
}
