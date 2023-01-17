using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Hero : PlaceableCard
{

	public bool doMoveForward;
	public bool activateSkill1;
	public bool activateSkill2;

	public static Hero selectedHero;


	Card originalCard;
	Battlefield battlefield;
	GameController gameController;


	Transform pivot;
	Card card;
	CardObject cardObject;
	Player player;
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

	public Vector2 GridPosition
	{
		get
		{
			return battlefield.UnityToGrid(transform.position);
		}
	}

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
		//Debug
		if (doMoveForward)
		{
			moveForward();
			doMoveForward = false;
		}
		if (activateSkill1)
		{
			gameController.AddMacro(card.Skills[0], cardObject);
			activateSkill1 = false;
		}
		if (activateSkill2)
		{
			gameController.AddMacro(card.Skills[1], cardObject);
			activateSkill2 = false;
		}
		//Debug

		if (attack)
		{
			attack = false;
			moveForward();
		}

		if (!isDying)
		{
			if (isWalking)
			{
				if (transform.position != battlefield.GridToUnity(targetPoint))
				{
					GameConfiguration.PlaySFX(GameConfiguration.denyAction);
					transform.position = Vector3.MoveTowards(transform.position, battlefield.GridToUnity(targetPoint), Time.deltaTime * 3f);
				}
				else
				{
					gameController.SetTriggerType(TriggerType.OnAfterWalk, cardObject);
					isWalking = false;
				}
			}

			if (card.life <= 0)
			{
				LogController.Log(Action.AttackChar, this);
				Die();
			}

			layerMask = 1 << gameObject.layer;
			results = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000, layerMask);
			if (results.ToList().FindAll(a => a.collider.gameObject == this.gameObject).Count > 0)
			{
				isMouseOver = true;
			}
			else
			{
				isMouseOver = false;
			}

			Life.text = card.life.ToString();
			_Attack.text = calculateAttackPower().ToString();
		}

		if (isMouseOver)
		{
			selectedHero = this;
		}
		else
		{
			if (selectedHero == this)
			{
				selectedHero = null;
			}
		}
	}

	public void Setup(GameController gameController, Battlefield battlefield, CardObject card)
	{
		this.cardObject = card;

		this.card = cardObject.cardData;
		player = cardObject.player;
		this.battlefield = battlefield;
		this.gameController = gameController;

		Instantiate(Resources.Load<GameObject>("Prefabs/Particles/Smoke"), transform.position, Quaternion.identity);
	}


	void WalkTo(Vector2 pos)
	{
		targetPoint = battlefield.Normalize(pos);
		isWalking = true;

		gameController.SetTriggerType(TriggerType.OnBeforeWalk, cardObject);
	}

	public void setCard(Card card, Player player)
	{
		this.card = card;
		this.originalCard = card;
		this.player = player;
	}

	public void moveForward()
	{
		//if (!isWalking) {
		Vector2 newPos = calculateEndPosition();

		Hero aux = checkForEnemiesInFront();


		if (aux == null)
		{

			WalkTo(newPos);
		}
		else
		{
			Debug.Log(card.name + " - achou " + aux.name);
		}
		//}
	}

	public void Attack()
	{
		isAttacking = true;
		StartCoroutine("DoAttack");
	}
	IEnumerator DoAttack()
	{
		isAttacking = true;

		for (int i = 0; i < numberOfAttacks; i++)
		{

			gameController.SetTriggerType(TriggerType.OnBeginAttack, cardObject);
			Hero targetHero = checkForEnemiesInFront();
			if (targetHero == null)
			{
				var isAtEdgeOfOpponent = battlefield.IsAtEnemyEdge(this);

				if (isAtEdgeOfOpponent)
				{
					Debug.LogWarning("Attacked player with " + calculateAttackPower() + " damage");
					LogController.Log(Action.AttackPlayer, calculateAttackPower(), player, gameController.GetOpponent(player));
					gameController.AttackPlayer(calculateAttackPower());
					lastGivenDamage = calculateAttackPower();
				}
			}
			else
			{
				if (targetHero.player != player)
				{
					LogController.Log(Action.AttackChar, calculateAttackPower(), this, targetHero);
					Debug.LogWarning("Attacked " + targetHero.name + " with " + calculateAttackPower() + " damage");
					targetHero.doDamage(calculateAttackPower());
					lastGivenDamage = calculateAttackPower();
				}
			}

			gameController.SetTriggerType(TriggerType.OnAfterAttack, cardObject);
			yield return new WaitForSeconds(0.25f);
		}



		numberOfAttacks = 1;
		isAttacking = false;
	}
	public void doDamage(int value)
	{
		DamageCounter.New(transform.position, -value);
		Instantiate(Resources.Load<GameObject>("Prefabs/Particles/Hit"), pivot.transform.position, Quaternion.identity);
		GameConfiguration.PlaySFX(GameConfiguration.Hit);
		card.life -= value;
	}

	public void Die()
	{
		cardObject.Die();
	}

	public void AddAttack(int number)
	{
		if (number < 0) number = 0;
		card.attack += number;
	}
	public void RemoveAttack(int number)
	{
		if (number < 0) number = 0;
		card.attack -= number;
	}
	public void AddLife(int number)
	{
		if (number < 0) number = 0;
		DamageCounter.New(transform.position, number);
		card.life += number;
	}
	public void ResetAttack()
	{
		card.attack = originalCard.attack;
	}
	public void ResetLife()
	{
		if (card.life > originalCard.life)
			card.life = originalCard.life;
	}
	public void DisableSkills()
	{
		Skill aux;
		for (int i = 0; i < card.Skills.Count; i++)
		{
			aux = card.Skills[i];
			aux.isActive = false;
			card.Skills[i] = aux;
		}
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

	Vector2 calculateEndPosition()
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
	}

	public override Vector3 GetTopPosition()
	{
		return pivot.transform.position;
	}

	public int calculateAttackPower()
	{
		return card.attack;
	}

	public int calculateWalkSpeed()
	{
		return walkSpeed;
	}

	int movementDirection()
	{
		if (player.GetPlayerType() == PlayerType.Remote)
		{
			return -1;
		}
		else
		{
			return 1;
		}
	}

	void OnMouseDown()
	{
		cardObject.isBeingHeroVisualized = true;
	}

	Hero checkForEnemiesInFront()
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
	}
	public List<Skill> GetSkills()
    {
		return card.Skills;
    }
	public Player GetPlayer()
    {
		return player;
    }
	public Card GetCard()
    {
		return card;
    }
	public CardObject GetCardObject()
    {
		return cardObject;
    }
	public Vector3 GetPivotPosition()
    {
		return pivot.transform.position;
    }
	public int GetLastGivenDamage()
    {
		return lastGivenDamage;
	}
	public bool IsAttacking()
	{
		return isAttacking;
	}
	public bool IsWalking()
	{
		return isWalking;
	}
}