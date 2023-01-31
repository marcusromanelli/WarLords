using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Hero : MonoBehaviour
{

	public bool doMoveForward;
	public bool activateSkill1;
	public bool activateSkill2;

	public CardObject CardObject { get; private set; }

	public static Hero selectedHero;

	//Battlefield battlefield;
	GameController gameController;
	Player Player;

	Transform pivot;
	Vector2 nextPoint;
	bool attack;
	Vector2 targetPoint;
	bool isWalking;
	bool isAttacking;
	bool isDying;
	int diretion;
	int lastGivenDamage;
	int walkSpeed = 1;
	int numberOfAttacks = 1;

	/*public Vector2 GridPosition
	{
		get
		{
			return battlefield.UnityToGrid(transform.position);
		}
	}
	*/
	TextMesh Life, _Attack;

	int layerMask;

	void Start()
	{
		GameObject aux = new GameObject();
		aux.transform.SetParent(this.transform);
		Vector3 aux2 = Vector3.zero;
		aux2.y = GetComponent<Renderer>().bounds.size.y;
		aux.transform.position = aux2;

		transform.Find("Status").transform.localPosition = Vector3.zero;
		Life = transform.Find("Status/Life/Value").GetComponent<TextMesh>();
		_Attack = transform.Find("Status/Attack/Value").GetComponent<TextMesh>();

		pivot = transform.GetChild(0);
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
		}*/
	}

	public void Setup(GameController gameController, /*Battlefield battlefield, */CardObject card)
	{
		//this.CardObject = card;

		//Player = CardObject.player;
		//this.battlefield = battlefield;
		//this.gameController = gameController;

		//Instantiate(Resources.Load<GameObject>("Prefabs/Particles/Smoke"), transform.position, Quaternion.identity);
	}


	/*void WalkTo(Vector2 pos)
	{
		targetPoint = battlefield.Normalize(pos);
		isWalking = true;

		//gameController.SetTriggerType(TriggerType.OnBeforeWalk, CardObject);
	}*/

	public void moveForward()
	{
		////if (!isWalking) {
		//Vector2 newPos = calculateEndPosition();

		//Hero aux = checkForEnemiesInFront();


		//if (aux == null)
		//{

		//	WalkTo(newPos);
		//}
		//else
		//{
		//	Debug.Log(CardObject.GetCardData().name + " - achou " + aux.name);
		//}
		//}
	}

	
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
	public void AddWalkSpeed(int number)
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
	}

	/*Vector2 calculateEndPosition()
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

	public Vector3 GetTopPosition()
	{
		return pivot.transform.position;
	}


	public int calculateWalkSpeed()
	{
		return walkSpeed;
	}

	int movementDirection()
	{
		/*if (Player.GetPlayerType() == PlayerType.Remote)
		{
			return -1;
		}
		else
		{
			return 1;
		}*/
		return 1;
	}

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
	}*/
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
	}*/
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
}