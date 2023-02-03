using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class ManaPool
{
	[SerializeField] UIManaPool uiManaPool;
	[SerializeField, ReadOnly] int maxAllowedMana = 12;
	[SerializeField, ReadOnly] int currentMana;
	[SerializeField, ReadOnly] int maxMana;

	public int MaxMana => maxMana;
	public int CurrentMana => currentMana;

	public void Setup(HandleCanSummonHero CanSummonHero)
	{
		GameController.LocalPlayer.OnHoldCard += OnLocalPlayerHoldingCard;

		uiManaPool.Setup(GetMaxAllowedMana, GetMaxMana, GetCurrentMana, CanSummonHero);
    }
	public void IncreaseMaxMana(int number = 1)
    {
		SetMaxManaValue(maxMana + number);
		SetCurrentManaValue(currentMana + number);

		uiManaPool.UpdateUI();
	}

	public void RestoreSpentMana(int number = -1)
    {
		var valueToRestore = number == -1 ? maxMana - currentMana : number;

		if (valueToRestore <= 0)
			return;

		GameConfiguration.PlaySFX(GameConfiguration.energyToCard);

		SetCurrentManaValue(currentMana + valueToRestore);

		uiManaPool.UpdateUI();
	}
	public void SpendMana(int number)
	{
		if (currentMana < number)
		{
			return;
		}

		if (number <= 0)
			return;

		GameConfiguration.PlaySFX(GameConfiguration.useEnergy);

		SetCurrentManaValue(currentMana - number);

		uiManaPool.UpdateUI();
	}
	public bool HasManaSpace()
	{
		return maxMana < maxAllowedMana;
	}
	public bool HasAvailableMana(int value)
	{
		return value <= currentMana;
	}
	public int GetCurrentMana()
	{
		return CurrentMana;
	}
	public int GetMaxMana()
	{
		return MaxMana;
	}
	public int GetMaxAllowedMana()
	{
		return maxAllowedMana;
	}
	void SetMaxManaValue(int newValue)
	{
		maxMana = Mathf.Clamp(newValue, 0, maxAllowedMana);
	}
	void SetCurrentManaValue(int newValue)
	{
		currentMana = Mathf.Clamp(newValue, 0, maxMana);
	}

	#region UI_MANAPOOL_INTERFACE
	void OnLocalPlayerHoldingCard(Player player, CardObject cardObject)
	{
		uiManaPool.OnLocalPlayerHoldCard(player, cardObject);
	}
	#endregion UI_MANAPOOL_INTERFACE
}