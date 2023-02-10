using System.Collections;
using UnityEngine;

public class UIToken : MonoBehaviour
{
	[SerializeField] TokenStatus Status;
	[SerializeField] ParticleSystem damageParticleSystem;
	[SerializeField] ParticleSystem summonParticleSystem;
	[SerializeField] DamageCounter damageCounter;
	[SerializeField] float walkSpeed = 0.5f;

	public Vector2 GridPosition => gridPosition;


	private Vector2 gridPosition;
	private Token tokenObject;
	private IAttackable target;
	private bool isWalking;
	private Vector3 targetPosition;
	private CardObject cardObject;

    #region CARD_OBJECT_INTERFACE
    public void Setup(CardObject cardObject)
    {
		this.gameObject.SetActive(true);

		this.cardObject = cardObject;

		CreateToken(cardObject);

		summonParticleSystem.Play();

		UpdateVisuals();
	}
	public void ResetObj()
	{
		if (tokenObject != null)
			tokenObject.gameObject.SetActive(false);

		target = null;
		tokenObject = null;
	}
	public void SetTarget(IAttackable target)
	{
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

		target.TakeDamage(cardObject.CalculateAttack());
	}
	public void ResetTarget()
	{
		target = null;
	}
	public void TakeDamage(uint damage)
	{
		Debug.Log(this.name + " took " + damage + " damage");

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



	void CreateToken(CardObject cardObject)
    {
		var token = ElementFactory.CreateObject<Token>(cardObject.Data.Civilization.Token, transform);

		tokenObject = token;
		tokenObject.transform.localPosition = Vector3.zero;
		tokenObject.transform.localRotation = Quaternion.identity;

		tokenObject.SetCardObject(cardObject.GetPhysicalCardObject());
	}
	void Update() {
		DoMovement();
	}
	void DoMovement()
    {
		if (!isWalking)
			return;

        if (transform.position != targetPosition)
        {
            GameConfiguration.PlaySFX(GameConfiguration.denyAction);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * walkSpeed);

			return;
        }

        isWalking = false;
    }
	void UpdateVisuals()
    {
		tokenObject.transform.localPosition = Vector3.zero;
		tokenObject.transform.rotation = Quaternion.identity;

		tokenObject.Setup(cardObject.Data.FrontCover);

		Status.Setup(cardObject.CalculateLife(), cardObject.CalculateAttack());
	}
    public uint GetLife()
    {
		return Status.Life;
    }
}