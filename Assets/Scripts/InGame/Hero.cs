using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour {

	public bool doMoveForward;
	public bool activateSkill1;
	public bool activateSkill2;

	public static Hero selectedHero;


	Card originalCard;
	Grid Grid;


	public Transform pivot;
	public Card card;
	public CardObject cardObject;
	public Player player;
	public Vector2 nextPoint;
	public bool attack;
	Vector2 targetPoint;
	public bool isWalking;
	public bool isAttacking;
	public bool isDying;
	public int diretion;
	public int lastGivenDamage;
	public int walkSpeed = 1;
	public int numberOfAttacks = 1;
	public Vector2 gridPosition;

	TextMesh Life, _Attack;

	RaycastHit[] results;
	int layerMask;
	bool isMouseOver;


	void Awake(){
	}
	void Start () {
		Grid = Grid.Singleton;
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

	void Update () {
		gridPosition = Grid.UnityToGrid (transform.position);

		//Debug
		if (doMoveForward) {
			moveForward ();
			doMoveForward = false;
		}
		if (activateSkill1) {
			GameController.AddMacro (card.Skills [0], cardObject);
			activateSkill1 = false;
		}
		if (activateSkill2) {
			GameController.AddMacro (card.Skills [1], cardObject);
			activateSkill2 = false;
		}
		//Debug

		if (attack) {
			attack = false;
			moveForward ();
		}

		if (!isDying) {
			if (isWalking) {
				if (transform.position != Grid.GridToUnity (targetPoint)) {
					GameConfiguration.PlaySFX(GameConfiguration.denyAction);
					transform.position = Vector3.MoveTowards (transform.position, Grid.GridToUnity (targetPoint), Time.deltaTime * 3f);
				} else {
					GameController.SetTriggerType(TriggerType.OnAfterWalk, cardObject);
					isWalking = false;
				}
			}
				
			if (card.life <= 0) {
				LogController.Log (Action.AttackChar, this);
				Die ();
			}

			layerMask = 1 << gameObject.layer;
			results = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition), 1000, layerMask);
			if (results.ToList().FindAll(a => a.collider.gameObject == this.gameObject).Count>0) {
				isMouseOver = true;
			}else{
				isMouseOver = false;
			}

			Life.text = card.life.ToString();
			_Attack.text = calculateAttackPower().ToString();
		}

		if (isMouseOver) {
			selectedHero = this;
		} else {
			if (selectedHero == this) {
				selectedHero = null;
			}
		}
	}

	public void Initialize(CardObject card){
		this.cardObject = card;

		this.card = cardObject.card;
		player = cardObject.player;


		Instantiate(Resources.Load<GameObject>("Prefabs/Particles/Smoke"), transform.position, Quaternion.identity);
	}


	void WalkTo(Vector2 pos){
		targetPoint = Grid.Normalize(pos);
		isWalking = true;

		GameController.SetTriggerType(TriggerType.OnBeforeWalk, cardObject);
	}

	public void setCard(Card card, Player player){
		this.card = card;
		this.originalCard = card;
		this.player = player;
	}
	
	public void moveForward(){
		//if (!isWalking) {
			Vector2 newPos = calculateEndPosition ();

			Hero aux = checkForEnemiesInFront ();


			if (aux == null) {

				WalkTo (newPos);
		}else{
				Debug.Log(card.name+" - achou "+aux.name);
			}
		//}
	}

	public void Attack(){
		isAttacking = true;
		StartCoroutine ("DoAttack");
	}
	IEnumerator DoAttack(){
		isAttacking = true;

		for (int i = 0; i < numberOfAttacks; i++) {

			GameController.SetTriggerType(TriggerType.OnBeginAttack, cardObject);
			Hero hero = checkForEnemiesInFront ();
			if (hero == null) {
				if ((player.isRemotePlayer && gridPosition.y == 0) || (!player.isRemotePlayer && gridPosition.y == Grid.numberOfSquares - 1)) {
					Debug.LogWarning ("Attacked player with " + calculateAttackPower () + " damage");
					LogController.Log (Action.AttackPlayer, calculateAttackPower(), player, GameController.getOpponent(player));
					GameController.Singleton.AttackPlayer (calculateAttackPower ());
					lastGivenDamage = calculateAttackPower ();
				}
			} else {
				if(hero.player != player){
					LogController.Log (Action.AttackChar,  calculateAttackPower(), this, hero);
					Debug.LogWarning ("Attacked "+hero.name+" with " + calculateAttackPower() + " damage");
					hero.doDamage (calculateAttackPower ());
					lastGivenDamage = calculateAttackPower ();
				}
			}

			GameController.SetTriggerType(TriggerType.OnAfterAttack, cardObject);
			yield return new WaitForSeconds (0.25f);
		}



		numberOfAttacks = 1;
		isAttacking = false;
	}
	public void doDamage(int value){
		DamageCounter.New (transform.position, -value);
		Instantiate(Resources.Load<GameObject>("Prefabs/Particles/Hit"), pivot.transform.position, Quaternion.identity);
		GameConfiguration.PlaySFX(GameConfiguration.Hit);
		card.life -= value;
	}

	public void Die(){
		cardObject.Die ();
	}
	
	public void AddAttack(int number){
		if(number<0) number=0;
		card.attack += number;
	}
	public void RemoveAttack(int number){
		if(number<0) number=0;
		card.attack -= number;
	}
	public void AddLife(int number){
		if(number<0) number=0;
		DamageCounter.New (transform.position, number);
		card.life += number;
	}
	public void ResetAttack(){
		card.attack = originalCard.attack;
	}
	public void ResetLife(){
		if(card.life>originalCard.life){
			card.life = originalCard.life;
		}
	}
	public void DisableSkills(){
		Skill aux;
		for (int i=0;i<card.Skills.Count;i++){
			aux = card.Skills [i];
			aux.isActive = false;
			card.Skills [i] = aux;
		}
	}
	
	public void AddWalkSpeed(int number){
		if (number < 0)
			number = 0;
		
		if (walkSpeed == 1) {
			GameConfiguration.PlaySFX(GameConfiguration.movementBuff);
			walkSpeed += number;
		}
	}
	public void RemoveWalkSpeed(int number){
		if (number < 0)
			number = 0;

		walkSpeed-=number;
		if (walkSpeed <= 0)
			walkSpeed = 1;
	}
	
	public void AddNumberOfAttacks(int number){
		if (number < 0)
			number = 0;
		numberOfAttacks+=number;
	}
	public void RemoveNumberOfAttacks(int number){
		if (number < 0)
			number = 0;

		numberOfAttacks-=number;
		if (numberOfAttacks <= 0)
			numberOfAttacks = 1;
	}

	Vector2 calculateEndPosition(){
		Vector2 gridPos = Grid.UnityToGrid(transform.position);
		gridPos.y += movementDirection() * calculateWalkSpeed();

		return gridPos;
	}

	Vector2 calculateNextForward(){
		Vector2 gridPos = Grid.UnityToGrid(transform.position);
		gridPos.y += movementDirection ();

		return gridPos;
	}

	public Vector3 getTopPosition(){
		return pivot.transform.position;
	}

	public int calculateAttackPower(){
		return card.attack;
	}

	public int calculateWalkSpeed(){
		return walkSpeed;
	}

	int movementDirection(){
		if(player.isRemotePlayer){
			return -1;
		}else{
			return 1;
		}
	}

	void OnMouseDown(){
		cardObject.isBeingHeroVisualized = true;
	}

	Hero checkForEnemiesInFront(){
		Vector2 aux = calculateNextForward ();
		int layerMask = 1 << gameObject.layer;

		List<Collider> aux2 = Physics.OverlapSphere (Grid.GridToUnity (aux), 0.3f, layerMask).ToList ();
		aux2.RemoveAll (a => a.GetComponent<Hero>() == this.GetComponent<Hero>());
		if (aux2.Count>0) {
			return aux2 [0].GetComponent<Hero> ();
		}

		return null;
	}
}