using System.Collections;
using UnityEngine;

public class HeroObject : MonoBehaviour, IPoolable, IAttackable
{
	[SerializeField] TokenStatus Status;
	[SerializeField] ParticleSystem damageParticleSystem;
	[SerializeField] ParticleSystem summonParticleSystem;
	[SerializeField] DamageCounter damageCounter;
	[SerializeField] float walkSpeed = 0.5f;

	public Vector2 GridPosition => position;
	public CardObject CardObject => cardObject;

	private Vector2 position;
	private Card cardData;
	private Token tokenObject;
	private bool isWalking;
	private Vector3 targetPosition;
	private IAttackable target;
	private CardObject cardObject;


	public void Setup(CardObject cardObject)
    {
		cardData = cardObject.Data;
		this.cardObject = cardObject;
		this.cardObject.SetInvoked();

		CreateToken(cardObject);

		summonParticleSystem.Play();

		UpdateVisuals();
	}
	public void Pool()
	{
		cardData = null;
		tokenObject.Destroy();
		Destroy(tokenObject.gameObject);
		target = null;
		position = Vector3.zero;
		targetPosition = Vector3.zero;
		isWalking = false;
		tokenObject = null;
	}
	public void Move(Vector3 position)
	{
		isWalking = true;
		targetPosition = position;
	}
	public uint GetWalkSpeed()
	{
		return cardObject.CalculateWalkSpeed();
	}
	public IEnumerator IsWalking()
    {
		while (isWalking)
			yield return null;
	}
	public void SetPosition(Vector2 position)
    {
		this.position = position;
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

		Debug.Log("Hero attacking " + cardObject.name);

		target.TakeDamage(cardObject.CalculateAttack());
	}
	public void ResetTargets()
	{
		target = null;
	}
	public void TakeDamage(uint damage)
	{
		Debug.Log(this.name + " took " + damage + " damage");

		damageParticleSystem.Play();

		Status.TakeDamage(damage);

		damageCounter.Show(damage);
	}
	public void Heal(uint health)
	{
		Status.Heal(health);
	}
	void CreateToken(CardObject cardObject)
    {
		var token = ElementFactory.CreateObject<Token>(cardData.Civilization.Token, transform);

		tokenObject = token;
		tokenObject.transform.localPosition = Vector3.zero;
		tokenObject.transform.localRotation = Quaternion.identity;

		tokenObject.SetCardObject(cardObject);
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

        //gameController.SetTriggerType(TriggerType.OnAfterWalk, CardObject);

        isWalking = false;
    }
	void UpdateVisuals()
    {
		tokenObject.transform.localPosition = Vector3.zero;
		tokenObject.transform.rotation = Quaternion.identity;

		tokenObject.Setup(cardData.FrontCover);

		Status.Setup(cardObject.CalculateLife(), cardObject.CalculateAttack());
	}
    public uint GetLife()
    {
		return Status.Life;
    }	
    public string GetId()
    {
		return cardData.Id;
    }

    /*
	void Start()
	{
		/*GameObject aux = new GameObject();
		aux.transform.SetParent(this.transform);
		Vector3 aux2 = Vector3.zero;
		aux2.y = GetComponent<Renderer>().bounds.size.y;
		aux.transform.position = aux2;

		transform.Find("Status").transform.localPosition = Vector3.zero;
		Life = transform.Find("Status/Life/Value").GetComponent<TextMesh>();
		_Attack = transform.Find("Status/Attack/Value").GetComponent<TextMesh>();

		pivot = transform.GetChild(0);*
	}

	void Update()
	{
		////Debug
		//if (doMoveForward)
		//{
		//	moveForward();
		//	doMoveForward = false;
		//}
		//if (activateSkill1)
		//{
		//	gameController.AddMacro(CardObject.GetCardData().Skills[0], CardObject);
		//	activateSkill1 = false;
		//}
		//if (activateSkill2)
		//{
		//	gameController.AddMacro(CardObject.GetCardData().Skills[1], CardObject);
		//	activateSkill2 = false;
		//}
		////Debug

		//if (attack)
		//{
		//	attack = false;
		//	moveForward();
		//}

		//if (!isDying)
		//{
		//	if (isWalking)
		//	{
		//		if (transform.position != battlefield.GridToUnity(targetPoint))
		//		{
		//			GameConfiguration.PlaySFX(GameConfiguration.denyAction);
		//			transform.position = Vector3.MoveTowards(transform.position, battlefield.GridToUnity(targetPoint), Time.deltaTime * 0.5f);
		//		}
		//		else
		//		{
		//			gameController.SetTriggerType(TriggerType.OnAfterWalk, CardObject);
		//			isWalking = false;
		//		}
		//	}

		//	var currentLife = CardObject.GetCardData().life;

		//	if (currentLife <= 0)
		//	{
		//		LogController.Log(Action.AttackChar, this);
		//		Die();
		//	}

		//	layerMask = 1 << gameObject.layer;
		//	var results = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000, layerMask);
		//	if (results.ToList().FindAll(a => a.collider.gameObject == this.gameObject).Count > 0)
		//	{
		//		//isMouseOver = true;
		//	}
		//	else
		//	{
		//		//isMouseOver = false;
		//	}

		//	Life.text = currentLife.ToString();
		//	//_Attack.text = CardObject.calculateAttackPower().ToString();
		//}

		/*if (isMouseOver)
		{
			selectedHero = this;
		}
		else
		{
			if (selectedHero == this)
			{
				selectedHero = null;
			}
		}*
	}

	/*void WalkTo(Vector2 pos)
	{
		targetPoint = battlefield.Normalize(pos);
		isWalking = true;

		//gameController.SetTriggerType(TriggerType.OnBeforeWalk, CardObject);
	}*



	
	public void doDamage(int value)
	{
		//DamageCounter.New(transform.position, -value);
		//Instantiate(Resources.Load<GameObject>("Prefabs/Particles/Hit"), pivot.transform.position, Quaternion.identity);
		//GameConfiguration.PlaySFX(GameConfiguration.Hit);
		//CardObject.RemoveLife(value);
	}
	public void Die()
	{
		//CardObject.Die();
	}

    /*public void AddWalkSpeed(int number)
{
    if (number < 0)
        number = 0;

    if (walkSpeed == 1)
    {
        GameConfiguration.PlaySFX(GameConfiguration.movementBuff);
        walkSpeed += number;
    }
}
public void RemoveWalkSpeed(int number)
{
    if (number < 0)
        number = 0;

    walkSpeed -= number;
    if (walkSpeed <= 0)
        walkSpeed = 1;
}

public void AddNumberOfAttacks(int number)
{
    if (number < 0)
        number = 0;
    numberOfAttacks += number;
}
public void RemoveNumberOfAttacks(int number)
{
    if (number < 0)
        number = 0;

    numberOfAttacks -= number;
    if (numberOfAttacks <= 0)
        numberOfAttacks = 1;
}*

    *Vector2 calculateEndPosition()
	{
		Vector2 gridPos = battlefield.UnityToGrid(transform.position);
		gridPos.y += movementDirection() * calculateWalkSpeed();

		return gridPos;
	}

	Vector2 calculateNextForward()
	{
		Vector2 gridPos = battlefield.UnityToGrid(transform.position);
		gridPos.y += movementDirection();

		return gridPos;
	}*/

    /*public Vector3 GetTopPosition()
	{
		return pivot.transform.position;
	}*



	void OnMouseDown()
	{
		//CardObject.isBeingHeroVisualized = true;
	}

	/*Hero checkForEnemiesInFront()
	{
		Vector2 aux = calculateNextForward();
		int layerMask = 1 << gameObject.layer;

		List<Collider> aux2 = Physics.OverlapSphere(battlefield.GridToUnity(aux), 0.3f, layerMask).ToList();
		aux2.RemoveAll(a => a.GetComponent<Hero>() == this.GetComponent<Hero>());
		if (aux2.Count > 0)
		{
			return aux2[0].GetComponent<Hero>();
		}

		return null;
	}*
	public Vector3 GetPivotPosition()
	{
		return pivot.transform.position;
	}
	/*public List<Skill> GetSkills()
    {
		return card.Skills;
    }
	public Player GetPlayer()
    {
		return player;
    }
	public CardObject GetCardObject()
    {
		return CardObject;
    }
	public int GetLastGivenDamage()
    {
		return lastGivenDamage;
	}*
	public bool IsAttacking()
	{
		return isAttacking;
	}
	public bool IsWalking()
	{
		return isWalking;
	}

    public void CheckMouseOver(bool requiresClick)
    {
        throw new System.NotImplementedException();
    }

    public Quaternion GetTopRotation()
    {
        throw new System.NotImplementedException();
    }

    public void Pool()
    {
        throw new System.NotImplementedException();
    }*/
}