using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

public class ManaPoolController : PlaceableCard
{
	[SerializeField] Transform BaseCenter;
	[SerializeField] public float distanceBetweenColumns = 1f;
	[SerializeField] float distanceBetweenLines = 1f;
	[SerializeField] GameObject manaOrbAsset;
	[SerializeField] int maxManaAllowed = 12;


	Vector3 baseCenterPosition;
	List<ManaOrb> manaPoolElements;

	[SerializeField, ReadOnly] int currentMana;
	[SerializeField, ReadOnly] int maxMana;

	void Start()
	{
		manaPoolElements = new List<ManaOrb>();

		baseCenterPosition = BaseCenter.transform.position;
	}

	void Update()
	{
		base.Update();
	}
	float calculateZ(int number)
	{
		return (number % 6);
	}
	Vector3 calculateNextPosition()
	{
		int value = manaPoolElements.Count + 1;
		if (value >= 12)
			value = 12;

		Vector3 pos = transform.position;

		if (value <= 6)
		{
			pos.x -= distanceBetweenColumns;
		}
		else
		{
			pos.x += distanceBetweenColumns;
		}

		pos.z += transform.forward.z * calculateZ(value - 1) * distanceBetweenLines;
		return pos;
	}
	void AddOrb()
	{
		Vector3 next = calculateNextPosition();
		var manaObj = Instantiate(manaOrbAsset, Vector3.zero, Quaternion.Euler(270, 0, 180));
		
		manaObj.transform.position = next;
		var mana = manaObj.GetComponent<ManaOrb>();

		manaPoolElements.Add(mana);
		manaObj.transform.SetParent(transform, true);
	}
	[Button("Add 2 Mana")]
	void Add2Mana()
	{
		IncreaseMaxMana(2);
	}
	[Button("Spend 2 Mana")]
	void Spend2Mana()
	{
		SpendMana(2);
	}
	[Button("Preview 3 Mana")]
	void Preview2Mana()
	{
		PreviewMana(2);
	}
	[Button("Restore Previewed Mana")]
	void RestorePreviewedMana()
	{
		RestorePreviewedMana();
	}
	public Vector3 GetBasePosition()
	{
		return baseCenterPosition;
	}

	public void IncreaseMaxMana(int number)
    {
		SetMaxManaValue(maxMana + number);
		SetCurrentManaValue(currentMana + number);

		UpdateManaOrbs();
	}
	void SetMaxManaValue(int newValue)
	{
		maxMana = Mathf.Clamp(newValue, 0, maxManaAllowed);
	}	
	void SetCurrentManaValue(int newValue)
	{
		currentMana = Mathf.Clamp(newValue, 0, maxMana);
	}
	public void SpendMana(int number)
	{
		if (currentMana < number)
		{
			return;
		}

		SetCurrentManaValue(currentMana - number);

		UpdateManaOrbs();
	}
	public void RecoverSpentMana(int number)
	{
		SetCurrentManaValue(currentMana + number);

		UpdateManaOrbs();
	}
	void UpdateManaOrbs()
	{
		while (manaPoolElements.Count < maxMana)
		{
			AddOrb();
		}

		int c = 0;
		for (int i = manaPoolElements.Count; i > 0; i--)
		{
			if (currentMana >= i)
				manaPoolElements[i - 1].SetStatus(ManaStatus.Active);
			else
				manaPoolElements[i - 1].SetStatus(ManaStatus.Used);
		}
	}

	public void PreviewMana(int number)
	{
		for (int i = 0; i < manaPoolElements.Count; i++)
		{
			var manaOrb = manaPoolElements[i];

			switch (manaOrb.ManaStatus)
			{
				case ManaStatus.Active:
				case ManaStatus.Preview:
					if (i < number)
					{
						manaOrb.SetStatus(ManaStatus.Preview);
					}
					else
					{
						manaOrb.SetStatus(ManaStatus.Active);
					}
					break;
			}
		}
	}
	public void RecoverPreviewMana()
	{
		for (int i = 0; i < manaPoolElements.Count; i++)
		{
			var manaOrb = manaPoolElements[i];

			if (manaOrb.ManaStatus == ManaStatus.Preview)
				manaPoolElements[i].SetStatus(ManaStatus.Active);
		}
	}
}