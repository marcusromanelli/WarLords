using System.Collections;
using UnityEngine;

public class UITokenObject : MonoBehaviour
{
	[SerializeField] UICardObject uiCardObject;
	[SerializeField] TokenStatus Status;
	[SerializeField] ParticleSystem damageParticleSystem;
	[SerializeField] ParticleSystem summonParticleSystem;
	[SerializeField] DamageCounter damageCounter;
	[SerializeField] Animation slideCardPivot;
	[SerializeField] float walkSpeed = 0.5f;

	public Vector2 GridPosition => gridPosition;
	public Transform SlideCardPivot => slideCardPivot.transform;


	private Vector2 gridPosition;
	private IAttackable target;
	private bool isWalking;
	private Vector3 targetPosition;
	private CardObject cardObject;
	private Transform tokenObject;
	private InputController inputController;
	private bool isBeingVisualized;
	private bool isInvoked;

    #region CARD_OBJECT_INTERFACE
    public void Setup(CardObject cardObject, Vector2 gridPosition, InputController inputController)
    {
		isInvoked = true;
		this.gridPosition = gridPosition;
		this.gameObject.SetActive(true);
		this.inputController = inputController;
		this.cardObject = cardObject;

		CreateToken(cardObject, inputController);

		summonParticleSystem.Play();

		UpdateVisuals();
	}
	public void ResetObj()
	{
		if (!isInvoked)
			return;

		if (tokenObject != null)
			Destroy(tokenObject.gameObject);

		target = null;
		tokenObject = null;
		isInvoked = false;
	}
	public void SetTarget(IAttackable target)
	{
		LogController.LogChangeTarget(cardObject, target);
		this.target = target;
	}
	public bool HasTarget()
	{
		return target != null;
	}
	public IAttackable GetTarget()
	{
		return target;
	}
	public void Attack()
	{
		if (target == null)
			return;

		var damage = cardObject.CalculateAttack();

		LogController.LogAttack(damage, cardObject, target);

		target.TakeDamage(damage);
	}
	public void ResetTarget()
	{
		LogController.LogChangeTarget(cardObject, null);
		target = null;
	}
	public void TakeDamage(uint damage)
	{
		LogController.LogElementTookDamage(cardObject, damage);

		damageParticleSystem.Play();

		Status.TakeDamage(damage);

		damageCounter.Show(damage);

		if (Status.Life <= 0)
			Die();
	}
	public void Die()
    {
		gameObject.SetActive(false);
    }
	public void Heal(uint health)
	{
		Status.Heal(health);
	}
	public IEnumerator IsWalking()
	{
		while (isWalking)
			yield return null;
	}
	public void SetPosition(Vector2 position)
	{
		gridPosition = position;
	}
	public void Move(Vector3 position)
	{
		isWalking = true;
		targetPosition = position;
	}
	#endregion CARD_OBJECT_INTERFACE



	public void CardSlideIn()
	{
		slideCardPivot.Play();
	}
	void CreateToken(CardObject cardObject, InputController inputController)
    {
		var token = ((GameObject)ElementFactory.CreateObject(cardObject.Data.Graphics.GetToken(), transform)).transform;

		token.localPosition = Vector3.zero;
		token.localRotation = Quaternion.identity;

		tokenObject = token;

		uiCardObject.AttachPhsyicalCard(SlideCardPivot);

		uiCardObject.SetLocalPositionAndRotation(CardPositionData.Create(Vector3.zero, Quaternion.identity));

		CardSlideIn();
	}

	void Update() {
		DoMovement();
	}
	void DoMovement()
    {
		if (!isWalking)
			return;

		var targetTransform = cardObject.transform;


		if (targetTransform.position != targetPosition)
        {
            GameRules.PlaySFX(GameRules.denyAction);
			targetTransform.position = Vector3.MoveTowards(targetTransform.position, targetPosition, Time.deltaTime * walkSpeed);

			return;
        }

        isWalking = false;
    }
	void UpdateVisuals()
    {
		Status.Setup(cardObject.CalculateLife(), cardObject.CalculateAttack());
	}
    public uint GetLife()
    {
		return Status.Life;
    }
}