using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public delegate int GetMaxAllowedMana();
public delegate int GetMaxMana();
public delegate int GetCurrentMana();

public class UIManaPool : MonoBehaviour, ICardPlaceable
{
	[SerializeField] Transform cardReferencePosition;
	[SerializeField] public float distanceBetweenColumns = 1f;
	[SerializeField] float distanceBetweenLines = 1f;
	[SerializeField] GameObject manaOrbAsset;

	private List<ManaOrb> manaOrbs = new List<ManaOrb>();
	private GetMaxMana getMaxManaCallback;
	private GetCurrentMana getCurrentManaCallback;
	private GetMaxAllowedMana getMaxAllowedManaCallback;
	private HandleCanSummonHero canSummonHero;


	void Awake()
	{
		manaOrbs = new List<ManaOrb>();
	}
	public void Setup(GetMaxAllowedMana getGetMaxAllowedManaCallback, GetMaxMana getMaxManaCallback, GetCurrentMana getCurrentManaCallback, HandleCanSummonHero CanSummonHero)
    {
		this.getMaxAllowedManaCallback = getGetMaxAllowedManaCallback;
		this.getMaxManaCallback = getMaxManaCallback;
		this.getCurrentManaCallback = getCurrentManaCallback;
		this.canSummonHero = CanSummonHero;
	}
	[Button("Force Refresh UI")]
	public void UpdateUI()
	{
		while (manaOrbs.Count < getMaxManaCallback())
		{
			AddOrb();
		}

		for (int i = manaOrbs.Count; i > 0; i--)
		{
			var manaOrb = manaOrbs[i - 1];

			manaOrb.transform.position = CalculateOrbPosition(i - 1);

			if (getCurrentManaCallback() >= i)
				manaOrbs[i - 1].SetStatus(ManaStatus.Active);
			else
				manaOrbs[i - 1].SetStatus(ManaStatus.Used);
		}
	}
	public Vector3 GetTopCardPosition()
	{
		return cardReferencePosition.position;
	}
	public Quaternion GetRotationReference()
	{
		return cardReferencePosition.transform.rotation;
	}
	float calculateRow(int number)
	{
		var maxMana = getMaxAllowedManaCallback();

		return (number % (maxMana/2));
	}
	Vector3 CalculateOrbPosition(int index)
	{
		var maxMana = getMaxAllowedManaCallback();
		int value = Mathf.Clamp(index, 0, maxMana);

		Vector3 pos = transform.position;
		var isLeftPosition = value < maxMana / 2;

		if (isLeftPosition)
			pos.x -= distanceBetweenColumns;
		else
			pos.x += distanceBetweenColumns;

		pos.z += transform.forward.z * calculateRow(value) * distanceBetweenLines;

		return pos;
	}
	void AddOrb()
	{
		var manaObj = Instantiate(manaOrbAsset, Vector3.zero, Quaternion.Euler(270, 0, 180));
		Vector3 next = CalculateOrbPosition(manaOrbs.Count + 1);

		manaObj.transform.position = next;
		var mana = manaObj.GetComponent<ManaOrb>();

		manaOrbs.Add(mana);
		manaObj.transform.SetParent(transform, true);

		GameConfiguration.PlaySFX(GameConfiguration.cardToEnergy);
	}
#region FIELD_INTERACTION
	public void OnLocalPlayerHoldCard(Player player, CardObject cardObject)
	{
		if (cardObject == null || !canSummonHero(cardObject.Data))
		{
			RestorePreviewedMana();
			return;
		}

		var manaCost = cardObject.Data.CalculateSummonCost();

		PreviewMana(manaCost);
	}
	void PreviewMana(int number)
	{
		for (int i = 0; i < manaOrbs.Count; i++)
		{
			var manaOrb = manaOrbs[i];

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
	void RestorePreviewedMana()
	{
		for (int i = 0; i < manaOrbs.Count; i++)
		{
			var manaOrb = manaOrbs[i];

			if (manaOrb.ManaStatus == ManaStatus.Preview)
				manaOrbs[i].SetStatus(ManaStatus.Active);
		}
	}

	#endregion FIELD_INTERACTION
}