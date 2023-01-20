using UnityEngine;
using System.Collections.Generic;

public class UIManaPool : MonoBehaviour
{
	[SerializeField] Transform BaseCenter;
	[SerializeField] public float distanceBetweenColumns = 1f;
	[SerializeField] float distanceBetweenLines = 1f;
	[SerializeField] GameObject manaOrbAsset;


	Vector3 baseCenterPosition;
	List<ManaOrb> manaPoolElements = new List<ManaOrb>();

	[SerializeField] protected Player localPlayerController;
	public void Setup(Player player)
    {
		localPlayerController = player;
    }
	void Awake()
	{
		manaPoolElements = new List<ManaOrb>();

		baseCenterPosition = BaseCenter.transform.position;
	}
	float CalculateRow(int number)
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

		pos.z += transform.forward.z * CalculateRow(value - 1) * distanceBetweenLines;
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

		GameConfiguration.PlaySFX(GameConfiguration.cardToEnergy);
	}
	public Vector3 GetBasePosition()
	{
		return baseCenterPosition;
	}

	void UpdateManaOrbs()
	{
		/*while (manaPoolElements.Count < maxMana)
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
		}*/
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
	public void RestorePreviewedMana()
	{
		for (int i = 0; i < manaPoolElements.Count; i++)
		{
			var manaOrb = manaPoolElements[i];

			if (manaOrb.ManaStatus == ManaStatus.Preview)
				manaPoolElements[i].SetStatus(ManaStatus.Active);
		}
	}
}