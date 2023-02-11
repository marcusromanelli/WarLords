using NaughtyAttributes;
using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public delegate void OnClickCloseButton();
public delegate CardPositionData OnGetPositionAndRotation();
public class CardObject : MonoBehaviour, IPoolable, IAttackable
{
	[BoxGroup("Components"), SerializeField] UICardObject uiCardObject;
	[BoxGroup("Components"), SerializeField] UITokenObject uiToken;


	[BoxGroup("Game"), Expandable, SerializeField] private Card cardData;

	public Vector2 GridPosition => position;
	private Vector2 position;
	public bool IsPositioned => uiCardObject.IsInPosition;
	public bool IsVisualizing => isVisualizing;
	public bool IsInvoked => isInvoked;
	public Card Data => cardData;
	public RuntimeCardData RuntimeCardData => runtimeCardData;

	private bool interactable = true;
	private Player player;
	private RuntimeCardData runtimeCardData;
	private bool isVisualizing;
	private bool isInvoked;
	private InputController inputController;

	public void Setup(InputController inputController, Player player, Card card, bool isLocalPlayer)
	{
		this.name = card.Name;
		this.inputController = inputController;
		this.player = player;
		cardData = card;

		runtimeCardData = new RuntimeCardData(card);

		uiCardObject.Setup(this, isLocalPlayer);
	}
	public void Setup(InputController inputController, Card card, bool hideInfo)
	{
		Setup(inputController, null, card, hideInfo);
	}
	public void Invoke()
    {
		isInvoked = true;

		uiCardObject.RefreshCardUI();
		uiToken.Setup(this, inputController);

		GameConfiguration.PlaySFX(GameConfiguration.Summon);
	}
	public void SetVisualizing(bool isVisualizing, OnClickCloseButton closeCallback = null)
	{
		this.isVisualizing = isVisualizing;

		uiCardObject.RegisterCloseCallback(closeCallback);

		uiCardObject.RefreshCardUI();
	}
	public void SetPosition(CardPositionData cardData, Action onFinish = null)
	{
		uiCardObject.SetPositionAndRotation(cardData, onFinish);
	}
	public void SetLocalPosition(CardPositionData cardData, Action onFinish = null)
	{
		uiCardObject.SetLocalPositionAndRotation(cardData, onFinish);
	}
	public void SetPosition(OnGetPositionAndRotation getPositionAndRotation)
	{
		uiCardObject.SetPositionCallback(getPositionAndRotation);
	}
	public void Pool()
	{
		uiToken.ResetObj();
		uiCardObject.ResetUI();
		ResetCard();
	}	
	public void Lock()
	{
		interactable = false;
	}
	public void Unlock()
	{
		interactable = true;
	}
	public void BecameMana(Action onFinishesAnimation)
	{
		uiCardObject.BecameMana(onFinishesAnimation);
	}
	public SkillData[] GetSkills()
	{
		return runtimeCardData.Skills;
	}
	public string GetCardName()
	{
		return runtimeCardData.Name;
	}
	public uint CalculateAttack()
	{
		return runtimeCardData.CalculateAttack();
	}
	public uint CalculateLife()
	{
		return runtimeCardData.CalculateDefense();
	}
	public uint CalculateSummonCost()
	{
		return runtimeCardData.CalculateSummonCost();
	}
	public uint CalculateWalkSpeed()
	{
		return runtimeCardData.CalculateWalkSpeed();
	}
	public void UnselectAllSkills()
    {
		runtimeCardData.UnselectAllSkills();
    }
	public void ToggleSkill(SkillData skill, bool value)
    {
		runtimeCardData.ToggleSkill(skill, value);

		player?.SummonCostChanged(CalculateSummonCost());
	}
	public GameObject GetPhysicalCardObject()
	{
		return uiCardObject.GetPhysicalCardObject();
	}

	void ResetCard()
	{
		runtimeCardData = null;
		isVisualizing = false;
		isInvoked = false;
		player = null;
		interactable = true;
		cardData = default;
		runtimeCardData = default;
	}

	#region TOKEN_INTERFACE

	public void Move(Vector3 position)
	{
		uiToken.Move(position);
	}
	public void TakeDamage(uint damage)
    {
        uiToken.TakeDamage(damage);
    }
    public void Heal(uint health)
	{
		uiToken.Heal(health);
	}
    public uint GetLife()
	{
		return uiToken.GetLife();
	}
	public IEnumerator IsWalking()
	{
		yield return uiToken.IsWalking();
	}
	public void SetPosition(Vector2 position)
	{
		uiToken.SetPosition(position);
	}
	public void SetTarget(IAttackable target)
    {
		uiToken.SetTarget(target);
    }
	public void ResetTarget()
    {
		uiToken.ResetTarget();
	}
	public bool HasTarget()
	{
		return uiToken.HasTarget();
	}
	public IAttackable GetTarget()
	{
		return uiToken.GetTarget();
	}
	public void Attack()
	{
		uiToken.Attack();
	}
	#endregion TOKEN_INTERFACE
}