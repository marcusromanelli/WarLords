using UnityEngine;
using NaughtyAttributes;
using System;

[Serializable]
public class ManaPool
{
	[SerializeField] UIManaPool uiManaPool;
	[SerializeField, ReadOnly] int maxManaAllowed = 12;
	[SerializeField, ReadOnly] int currentMana;
	[SerializeField, ReadOnly] int maxMana;

	public int MaxMana => maxMana;
	public int CurrentMana => currentMana;
	void Start()
    {
		uiManaPool.Setup(GetMaxMana, GetCurrentMana);
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
		return maxMana < maxManaAllowed;
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
	void SetMaxManaValue(int newValue)
	{
		maxMana = Mathf.Clamp(newValue, 0, maxManaAllowed);
	}
	void SetCurrentManaValue(int newValue)
	{
		currentMana = Mathf.Clamp(newValue, 0, maxMana);
	}
}