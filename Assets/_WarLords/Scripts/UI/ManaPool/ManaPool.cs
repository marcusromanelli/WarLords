﻿using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class ManaPool
{
	[SerializeField] UIManaPool uiManaPool;
	[SerializeField, ReadOnly] uint maxAllowedMana = 12;
	[SerializeField, ReadOnly] uint currentMana;
	[SerializeField, ReadOnly] uint maxMana;

	public uint MaxMana => maxMana;
	public uint CurrentMana => currentMana;

	public void Setup(Player LocalPlayer, HandleCanPlayerSummonToken CanSummonHero)
	{
		LocalPlayer.OnHoldCard += OnLocalPlayerHoldingCard;

		uiManaPool.Setup(GetMaxAllowedMana, GetMaxMana, GetCurrentMana, CanSummonHero);
    }
	public void IncreaseMaxMana(uint number = 1)
    {
		SetMaxManaValue(maxMana + number);
		SetCurrentManaValue(currentMana + number);

		uiManaPool.UpdateUI();
	}
	public void RestoreSpentMana( )
	{
		RestoreSpentMana(maxMana);
	}
	public void RestoreSpentMana(uint number)
    {
		var valueToRestore = number;

		GameConfiguration.PlaySFX(GameConfiguration.energyToCard);

		SetCurrentManaValue(currentMana + valueToRestore);

		uiManaPool.UpdateUI();
	}
	public void SpendMana(uint number)
	{
		if (currentMana < number)
			return;

		GameConfiguration.PlaySFX(GameConfiguration.useEnergy);

		SetCurrentManaValue(currentMana - number);

		uiManaPool.UpdateUI();
	}
	public bool HasManaSpace()
	{
		return maxMana < maxAllowedMana;
	}
	public bool HasAvailableMana(uint value)
	{
		return value <= currentMana;
	}
	public uint GetCurrentMana()
	{
		return CurrentMana;
	}
	public uint GetMaxMana()
	{
		return MaxMana;
	}
	public uint GetMaxAllowedMana()
	{
		return maxAllowedMana;
	}
	public void RefreshPreviewedMana(uint newManaCost)
    {
		uiManaPool.RefreshPreviewedMana(newManaCost);
	}
	void SetMaxManaValue(uint newValue)
	{
		maxMana = (uint)Mathf.Clamp(newValue, 0, maxAllowedMana);
	}
	void SetCurrentManaValue(uint newValue)
	{
		currentMana = (uint)Mathf.Clamp(newValue, 0, maxMana);
	}

	#region UI_MANAPOOL_INTERFACE
	void OnLocalPlayerHoldingCard(Player player, CardObject cardObject)
	{
		uiManaPool.OnLocalPlayerHoldCard(player, cardObject);
	}
	#endregion UI_MANAPOOL_INTERFACE
}