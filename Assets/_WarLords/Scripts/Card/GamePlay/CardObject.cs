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
	[BoxGroup("Components"), SerializeField] UICardSkillSelection uiCardSkillSelection;


	[BoxGroup("Game"), Expandable, SerializeField] private Card cardData;

	public Player Player => player;
	public Vector2 GridPosition => uiToken.GridPosition;
	public bool IsPositioned => uiCardObject.IsInPosition;
	public bool IsVisualizing => isVisualizing;
	public bool IsInvoked => isSummoned;
	public Card Data => cardData;
	public RuntimeCardData RuntimeCardData => runtimeCardData;
	public Stack<CardObject> BuffedCards => buffedCards;

	private bool interactable = true;
	private Player player;
	private RuntimeCardData runtimeCardData;
	private bool isVisualizing;
	private bool isSummoned;
	private InputController inputController;
	private Stack<CardObject> buffedCards = new Stack<CardObject>();

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
		Setup(inputController, null, card, !hideInfo);
	}
	public void Summon(Vector2 gridPosition, Action<uint> OnFinishedSummon)
	{
		if(OnFinishedSummon == null)
		{
			FinishSummon(gridPosition);
			return;
		}

		uiCardSkillSelection.Show(player, runtimeCardData.OriginalCardSkills, (response) =>
		{
			var totalSkill = runtimeCardData.UpdateSkillStatus(response);

			FinishSummon(gridPosition);

			OnFinishedSummon?.Invoke(totalSkill);
		});
	}
	void FinishSummon(Vector2 gridPosition)
	{
		isSummoned = true;

		runtimeCardData.SetSummoned();
		uiCardObject.RefreshCardUI();
		uiToken.Setup(this, gridPosition, inputController);

		GameRules.PlaySFX(GameRules.Summon);
	}
	public void SummonSkillBuff(Vector3 position, Action<Dictionary<SkillData, bool>> OnFinishedSummon)
	{
		transform.position = position;
		uiCardSkillSelection.Show(player, runtimeCardData.OriginalCardSkills, (response) =>
		{
			runtimeCardData.UpdateSkillStatus(response);

			OnFinishedSummon?.Invoke(response);
		});
	}
	public void SkillBuff(CardObject originCardObject)
	{
		originCardObject.SummonSkillBuff(transform.position, (response) => {

			var summonCost = originCardObject.CalculateSummonCost(true);

			if(summonCost > 0)
				LogController.LogBuffToken(this, originCardObject, originCardObject.GetActiveSkills(), originCardObject.CalculateSummonCost(true));

			buffedCards.Push(originCardObject);

			runtimeCardData.BuffSkills(originCardObject.RuntimeCardData);

			originCardObject.Lock();
			originCardObject.gameObject.SetActive(false);
			originCardObject.transform.SetParent(transform);
		});
	}
	public void SetVisualizing(bool isVisualizing, OnClickCloseButton closeCallback = null)
	{
		this.isVisualizing = isVisualizing;

		uiCardObject.RefreshCardUI();

		uiCardObject.RegisterCloseCallback(closeCallback);
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
		uiToken?.ResetObj();
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
	public SkillData[] GetSkillList()
	{
		return runtimeCardData.OriginalCardSkills;
	}
	public List<SkillData> GetActiveSkills()
	{
		return runtimeCardData.ActiveSkills;
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
	public uint CalculateSummonCost(bool isSkillOnly)
	{
		return runtimeCardData.CalculateSummonCost(isSkillOnly);
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

		player?.SummonCostChanged(CalculateSummonCost(false));
	}
	public GameObject GetPhysicalCardObject()
	{
		return uiCardObject.GetPhysicalCardObject();
	}

	void ResetCard()
	{
		runtimeCardData = null;
		isVisualizing = false;
		isSummoned = false;
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
		runtimeCardData.TakeDamage(damage);
		uiToken.TakeDamage(damage);
    }
    public void Heal(uint health)
	{
		runtimeCardData.Heal(health);
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
    public string GetName()
    {
		return Data.Name;
    }
    #endregion TOKEN_INTERFACE
}