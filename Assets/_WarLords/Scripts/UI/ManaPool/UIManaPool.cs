using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public delegate uint GetMaxAllowedMana();
public delegate uint GetMaxMana();
public delegate uint GetCurrentMana();

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
	private HandleCanPlayerSummonToken canSummonHero;
	private bool isPreviewingManaCost;

	void Awake()
	{
		manaOrbs = new List<ManaOrb>();
	}
	public void Setup(GetMaxAllowedMana getGetMaxAllowedManaCallback, GetMaxMana getMaxManaCallback, GetCurrentMana getCurrentManaCallback, HandleCanPlayerSummonToken CanSummonHero)
    {
		this.getMaxAllowedManaCallback = getGetMaxAllowedManaCallback;
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
				manaOrb.SetStatus(ManaStatus.Active);
			else
				manaOrb.SetStatus(ManaStatus.Used);
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
		int value = Mathf.Clamp(index, 0, (int)maxMana);

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

		GameRules.PlaySFX(GameRules.cardToEnergy);
	}
#region FIELD_INTERACTION
	public void OnLocalPlayerVisualizeCard(Player player, RuntimeCardData cardObject)
	{
		if (cardObject == null || !canSummonHero(cardObject, false))
		{
			RestorePreviewedMana();
			return;
		}

		var manaCost = cardObject.CalculateSummonCost(false);

		PreviewMana(manaCost);
	}
	public void PreviewMana(uint number)
	{
		isPreviewingManaCost = true;
		int usedMana = (int)number;

		for (int i = manaOrbs.Count; i > 0; i--)
		{
			var manaOrb = manaOrbs[i - 1];

			if (manaOrb.ManaStatus == ManaStatus.Used)
				continue;

			if (usedMana > 0)
			{
				manaOrb.SetStatus(ManaStatus.Preview);
				usedMana--;
			}
			else if(manaOrb.ManaStatus == ManaStatus.Preview)
				manaOrb.SetStatus(ManaStatus.Active);
		}
	}
	void RestorePreviewedMana()
	{
		isPreviewingManaCost = false;

		for (int i = manaOrbs.Count; i > 0; i--)
		{
			var manaOrb = manaOrbs[i - 1];

			if (manaOrb.ManaStatus == ManaStatus.Preview)
				manaOrb.SetStatus(ManaStatus.Active);
		}
	}

	#endregion FIELD_INTERACTION
}