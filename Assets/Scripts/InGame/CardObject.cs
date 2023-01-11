using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;

public enum ManaStatus{
	Active, Used, Preview
}

public class CardObject : MonoBehaviour {

	public Card card;
	public bool isBeingHeld;
	public bool isBeingVisualized;
	public bool isBeingHeroVisualized;
	public SummonType SummonType;
	public float cardMovementSpeed = 20;
	bool isMouseDown;

	public Player player;

	TextWrapper skillDesc1, skillDesc2;
	LocalHand handController;
	GameObject summonButton, skillButton1, skillButton2, SkillsTree, Status, CloseButton;
	public Hero Character;
	public Vector3 originalPosition;
	Vector3 offset;
	Vector3 originalScale;
	Vector3 mousePosition;
	Vector3 destinyPosition, destinyScale;
	Quaternion destinyRotation;
	ActionType destiny;


	void Awake(){
		if (SummonType != SummonType.Mana) {
			if(transform.Find ("Summon")){
				summonButton = transform.Find ("Summon").gameObject;
				summonButton.SetActive(false);
			}
			if(transform.Find ("Skill1Button")){
				skillButton1 = transform.Find ("Skill1Button").gameObject;
				skillButton1.SetActive(false);
			}
			if(transform.Find ("Skill2Button")){
				skillButton2 = transform.Find ("Skill2Button").gameObject;
				skillButton2.SetActive(false);
			}
			if(transform.Find ("Skills")){
				SkillsTree = transform.Find ("Skills").gameObject;
				SkillsTree.SetActive(false);
			}
			if(transform.Find ("Status")){
				Status = transform.Find ("Status").gameObject;
				Status.SetActive(false);
			}
			if(transform.Find ("CloseButton")){
				CloseButton = transform.Find ("CloseButton").gameObject;
				CloseButton.SetActive(false);
			}
		}
	}

	void Start () {
		handController = GameObject.FindObjectOfType<LocalHand> ();
		originalScale = transform.localScale;
		
	}

	void showSkillTree(){
		if(SkillsTree!=null){
			SkillsTree.SetActive(true);
			Status.SetActive (true);
			CloseButton.SetActive(true);
		}
	}

	void hideSkillTree(){
		if(SkillsTree!=null){
			SkillsTree.SetActive(false);
			Status.SetActive (true);
			CloseButton.SetActive(true);
		}
	}

	void showSummonButtons(){
		if(summonButton!=null){
			if(Character==null){
				summonButton.SetActive(true);
			}
			skillButton1.SetActive(true);
			skillButton2.SetActive(true);
			CloseButton.SetActive(true);
		}
	}

	void hideSummonButtons(){
		if(summonButton!=null){
			summonButton.SetActive(false);
			skillButton1.SetActive(false);
			skillButton2.SetActive(false);
			CloseButton.SetActive(false);
		}
	}

	Color aux, aux2;
	float spd=1;
	void Update () {
		switch(SummonType){
		case SummonType.Mana:
			switch (card.manaStatus) {
			case ManaStatus.Used:
				GetComponentsInChildren<ParticleSystem> ().ToList ().ForEach (delegate(ParticleSystem obj) {
					if (obj.isPlaying || obj.IsAlive ()) {
						obj.Stop (true);
						spd = obj.playbackSpeed;
						obj.playbackSpeed = spd*5f;
					}
				});
				GetComponent<Renderer> ().material.color = Color.Lerp(GetComponent<Renderer> ().material.color, Color.black, Time.deltaTime);
				GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.black);
				break;
			case ManaStatus.Active:
				GetComponentsInChildren<ParticleSystem> ().ToList ().ForEach (delegate(ParticleSystem obj) {
					if (!obj.isPlaying) {
						obj.Play ();
						obj.playbackSpeed = spd;
					}
				});
				GetComponent<Renderer> ().material.color = Color.Lerp(GetComponent<Renderer> ().material.color, aux, Time.deltaTime);
				GetComponent<Renderer> ().material.SetColor ("_EmissionColor", aux2);
				break;
			case ManaStatus.Preview:
				GetComponentsInChildren<ParticleSystem> ().ToList ().ForEach (delegate(ParticleSystem obj) {
					if (!obj.isPlaying) {
						obj.Play ();
						obj.playbackSpeed = spd;
					}
				});
				GetComponent<Renderer> ().material.color = Color.Lerp(GetComponent<Renderer> ().material.color, Color.red, Time.deltaTime);
				GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.red);
				break;
			}
			hideSummonButtons ();
			break;
		case SummonType.Monster:
			if (isMouseDown && Vector3.Distance (mousePosition, Input.mousePosition) > 0.2) {
				isBeingHeroVisualized = false;
			}

			if (isBeingHeroVisualized) {
				transform.position = Vector3.MoveTowards (transform.position, Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / ((card.Skills.Count>2)?2.25f:2), 70, 5f)), Time.deltaTime * cardMovementSpeed);
				transform.localRotation = Quaternion.Euler (-50, 180, 0);
				transform.localScale = Vector3.MoveTowards (transform.localScale, originalScale*1.5f, Time.deltaTime * 15f);
				showSkillTree ();
				showSummonButtons ();
			} else {
				hideSkillTree ();
				hideSummonButtons ();
				transform.position = Vector3.MoveTowards (transform.position, player.BattlefieldController.getTopPosition (), Time.deltaTime * cardMovementSpeed);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, player.BattlefieldController.getTopRotation (), Time.deltaTime * 600f);
				transform.localScale = Vector3.MoveTowards (transform.localScale, player.BattlefieldController.getTopScale (), Time.deltaTime * 15f);			
				hideSummonButtons ();
			}

			break;
		case SummonType.DoNotApply: 
			if (card.CardID != 0) {
				if (card.CardID <= 0) {
					Destroy (gameObject);
					Debug.LogWarning ("Card created empty! Deleting...");
				} else {
					if (destiny == ActionType.NoAction) {
						if (isMouseDown && Vector3.Distance (mousePosition, Input.mousePosition) > 0.2) {
							isBeingHeld = true;
							isBeingVisualized = false;
						}
						mousePosition = Input.mousePosition;
						if (isBeingHeld) {
							hideSummonButtons ();
							offset.z = offset.x = 0;						
							if (GameController.Singleton.currentPlayer == ((int)player.civilization) || player.hasCondition (ConditionType.DiscartCard) || player.hasCondition (ConditionType.DrawCard) || player.hasCondition (ConditionType.SendCardToManaPool)) {
								if (player.DeckController.isMouseOver) {
									transform.position = Vector3.MoveTowards (transform.position, player.DeckController.getTopPosition (), Time.deltaTime * cardMovementSpeed);
									transform.rotation = Quaternion.RotateTowards (transform.rotation, player.DeckController.getTopRotation (), Time.deltaTime * 600f);
									transform.localScale = Vector3.MoveTowards (transform.localScale, player.DeckController.getTopScale (), Time.deltaTime * 15f);
								} else if (player.ManaPoolController.isMouseOver) {
									transform.position = Vector3.MoveTowards (transform.position, player.ManaPoolController.getBasePosition (), Time.deltaTime * cardMovementSpeed);
									transform.rotation = Quaternion.RotateTowards (transform.rotation, player.ManaPoolController.getTopRotation (), Time.deltaTime * 600f);
									transform.localScale = Vector3.MoveTowards (transform.localScale, player.ManaPoolController.getTopScale (), Time.deltaTime * 15f);
								} else if (player.GraveyardController.isMouseOver) {
									transform.position = Vector3.MoveTowards (transform.position, player.GraveyardController.getTopPosition (), Time.deltaTime * cardMovementSpeed);
									transform.rotation = Quaternion.RotateTowards (transform.rotation, player.GraveyardController.getTopRotation (), Time.deltaTime * 600f);
									transform.localScale = Vector3.MoveTowards (transform.localScale, player.GraveyardController.getTopScale (), Time.deltaTime * 15f);
								} else if (Hero.selectedHero!=null) {
									transform.position = Vector3.MoveTowards (transform.position, Hero.selectedHero.getTopPosition (), Time.deltaTime * cardMovementSpeed);
									transform.rotation = Quaternion.RotateTowards (transform.rotation, player.DeckController.getTopRotation (), Time.deltaTime * 600f);
									transform.localScale = Vector3.MoveTowards (transform.localScale, player.DeckController.getTopScale (), Time.deltaTime * 15f);
								} else if (SpawnArea.selected!=null && SpawnArea.selected.LocalPlayer) {
									transform.position = Vector3.MoveTowards (transform.position, SpawnArea.selected.getTopPosition (), Time.deltaTime * cardMovementSpeed);
									transform.rotation = Quaternion.RotateTowards (transform.rotation, SpawnArea.selected.getTopRotation (), Time.deltaTime * 600f);
									transform.localScale = Vector3.MoveTowards (transform.localScale, SpawnArea.selected.getTopScale (), Time.deltaTime * 15f);
								} else {
									transform.position = Vector3.MoveTowards (transform.position, Camera.main.ScreenToWorldPoint (new Vector3 (mousePosition.x, mousePosition.y, 10f)), Time.deltaTime * cardMovementSpeed);
									transform.localRotation = Quaternion.RotateTowards (transform.localRotation, Quaternion.Euler (Vector3.right*270), Time.deltaTime * handController.cardFoldSpeed);
									transform.localScale = Vector3.MoveTowards (transform.localScale, originalScale, Time.deltaTime * 15f);
								}
							} else {
								transform.position = Vector3.MoveTowards (transform.position, Camera.main.ScreenToWorldPoint (new Vector3 (mousePosition.x, mousePosition.y, 10f)), Time.deltaTime * cardMovementSpeed);
								transform.localRotation = Quaternion.RotateTowards (transform.localRotation, Quaternion.Euler (Vector3.right*270), Time.deltaTime * handController.cardFoldSpeed);
								transform.localScale = Vector3.MoveTowards (transform.localScale, originalScale, Time.deltaTime * 15f);
							}
						} else {
							if (isBeingVisualized) {
								transform.position = Vector3.MoveTowards (transform.position, Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2, 70, 5f)), Time.deltaTime * cardMovementSpeed);
								transform.localRotation = Quaternion.Euler (310, 0, 0);
								transform.localScale = Vector3.MoveTowards (transform.localScale, originalScale*1.5f, Time.deltaTime * 15f);
								player.previewSpendMana (card.calculateCost ());

								showSummonButtons ();
							} else {
								hideSummonButtons();
								//if (transform.localPosition != Vector3.zero) {
									//transform.localPosition = Vector3.MoveTowards (transform.localPosition, originalPosition, Time.deltaTime * cardMovementSpeed);
								//}
								transform.localScale = Vector3.MoveTowards (transform.localScale, originalScale, Time.deltaTime * 15f);
							}
						}
					} else {
						if(Vector3.Distance(transform.position, destinyPosition)>0.1f){
							transform.position = Vector3.MoveTowards (transform.position, destinyPosition, Time.deltaTime * cardMovementSpeed);
							transform.rotation = Quaternion.RotateTowards (transform.rotation, destinyRotation, Time.deltaTime * 600f);
							transform.localScale = Vector3.MoveTowards (transform.localScale, destinyScale, Time.deltaTime * 15f);
						}else{
							switch (destiny) {
							case ActionType.CardToDeck:
								player.DiscartCardToDrawTwo (card);
								break;
							case ActionType.CardToManaPool:
								player.SendCardToManaPool (card);
								break;
							case ActionType.DiscartCard:
								player.DiscartCard (card);
								break;
							case ActionType.BuffHero:
								GameConfiguration.PlaySFX (GameConfiguration.buffCard);
								target.cardObject.AddSkill (card.Skills [1]);
								GameController.SetTriggerType (TriggerType.OnBeforeSpawn, target.cardObject);
								GameController.SetTriggerType (TriggerType.OnAfterSpawn, target.cardObject);
								target = null;
								Destroy (this.gameObject);
								break;
							case ActionType.SummonHero:
								if (!player.Summon (this, destinyPosition)) {
									destiny = ActionType.NoAction;
									isBeingHeld = false;
								}
								break;
							}
						}
					}
				}
			}
			break;
		}
	}

	public void OnMouseDown(){
		if (player.hasCondition (ConditionType.PickSpawnArea)==false && player.isRemotePlayer==false) {
			mousePosition = Input.mousePosition;
			offset = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 11)) - transform.localPosition;
			isMouseDown = true;
		}
	}
	Hero target;
	public void OnMouseUp(){
		isMouseDown = false;
		if(SummonType== SummonType.DoNotApply){
			if (player.DeckController.isMouseOver) {
				if (GameController.Singleton.currentPhase != Phase.Attack && GameController.Singleton.currentPhase != Phase.End && GameController.Singleton.currentPhase != Phase.Movement && GameController.Singleton.MatchHasStarted) {
					if (!player.hasUsedHability) {
						destiny = ActionType.CardToDeck;
						destinyPosition = player.DeckController.getTopPosition ();
						destinyRotation = player.DeckController.getTopRotation ();
						destinyScale = player.DeckController.getTopScale ();
						return;
					} else {
						GameConfiguration.PlaySFX(GameConfiguration.denyAction);
						Debug.LogWarning ("You already used your hability this turn.");
					}
				} else {
					GameConfiguration.PlaySFX(GameConfiguration.denyAction);
					Debug.LogWarning ("This movement is not allowed now.");
				}
			} else if (player.ManaPoolController.isMouseOver) {
				if (GameController.Singleton.MatchHasStarted) {
					if (GameController.Singleton.currentPhase != Phase.Attack && GameController.Singleton.currentPhase != Phase.End && GameController.Singleton.currentPhase != Phase.Movement) {
						if (!player.hasUsedHability) {
							destiny = ActionType.CardToManaPool;
							destinyPosition = player.ManaPoolController.getBasePosition ();
							destinyRotation = player.ManaPoolController.getTopRotation ();
							destinyScale = player.ManaPoolController.getTopScale ();
							return;
						} else {
							GameConfiguration.PlaySFX(GameConfiguration.denyAction);
							Debug.LogWarning ("You already used your hability this turn.");
						}
					} else {
						GameConfiguration.PlaySFX(GameConfiguration.denyAction);
						Debug.LogWarning ("This movement is not allowed in this phase.");
					}
				} else {
					if (!player.hasCondition (ConditionType.SendCardToManaPool)) {
						GameConfiguration.PlaySFX(GameConfiguration.denyAction);
						Debug.LogWarning ("This movement is not allowed now.");
					} else {
						destiny = ActionType.CardToManaPool;
						destinyPosition = player.ManaPoolController.getBasePosition ();
						destinyRotation = player.ManaPoolController.getTopRotation ();
						destinyScale = player.ManaPoolController.getTopScale ();
						return;
					}
				}
			} else if (player.GraveyardController.isMouseOver) {
				if (!player.hasCondition (ConditionType.DiscartCard) && !Application.isEditor) {
					GameConfiguration.PlaySFX(GameConfiguration.denyAction);
					Debug.LogWarning ("You cannot discart a cart without a reason.");
				} else {
					destiny = ActionType.DiscartCard;
					destinyPosition = player.GraveyardController.getTopPosition ();
					destinyRotation = player.GraveyardController.getTopRotation ();
					destinyScale = player.GraveyardController.getTopScale ();
					return;
				}
			} else if (Hero.selectedHero!=null && !isBeingVisualized && isBeingHeld) {
				if (GameController.Singleton.MatchHasStarted) {
					if (player.canSpendMana (card.Skills [1].manaCost)) {
						target = Hero.selectedHero;
						player.SpendMana (card.Skills [1].manaCost);
						destiny = ActionType.BuffHero;

						destinyPosition = target.pivot.transform.position;
						destinyRotation = player.DeckController.getTopRotation ();
						destinyScale = player.DeckController.getTopScale ();
						return;
					}
				} else {
					GameConfiguration.PlaySFX(GameConfiguration.denyAction);
					Debug.LogWarning ("This movement is not allowed now.");
				}
			} else if (SpawnArea.selected!=null && !isBeingVisualized && isBeingHeld) {
				if (GameController.Singleton.MatchHasStarted) {
					if (player.canSpendMana (card.calculateCost ())) {
						destiny = ActionType.SummonHero;

						destinyPosition = SpawnArea.selected.getTopPosition ();
						destinyRotation = SpawnArea.selected.getTopRotation ();
						destinyScale = SpawnArea.selected.getTopScale ();
						return;
					} else {
						GameConfiguration.PlaySFX (GameConfiguration.denyAction);
						Debug.LogWarning ("Not enought mana.");
					}
					/*if (player.canSpendMana (card.Skills [1].manaCost)) {
						target = Hero.selectedHero;
						player.SpendMana (card.Skills [1].manaCost);
					}*/
				} else {
					GameConfiguration.PlaySFX(GameConfiguration.denyAction);
					Debug.LogWarning ("This movement is not allowed now.");
				}
			}else{
				if(player.isRemotePlayer==false && !isBeingHeld && Vector3.Distance(Input.mousePosition, mousePosition)<0.1f){
					isBeingVisualized = true;
					if (!isBeingVisualized) {
						Skill aux;
						for (int i = 0; i < card.Skills.Count; i++) {
							aux = card.Skills [i];
							aux.isActive = false;
							card.Skills [i] = aux;
						}
					}
				}
			}
		}else if(SummonType== SummonType.DoNotApply){
			isBeingHeroVisualized = !isBeingHeroVisualized;
		}
		isBeingHeld = false;
	}

	public void addSKill(int number){
		if(number>1) number = 1;
		Skill aux = card.Skills [number];
		aux.isActive = true;
		card.Skills [number] = aux;
	}

	public void removeSkill(int number){
		if(number>1) number = 1;
		Skill aux = card.Skills [number];
		aux.isActive = false;
		card.Skills [number] = aux;
	}

	public void setMana(){
		aux = GetComponent<Renderer> ().material.color;
		aux2 = GetComponent<Renderer> ().material.GetColor ("_EmissionColor");
		SummonType = SummonType.Mana;
		aux = GetComponent<Renderer> ().material.color;
		aux2 = GetComponent<Renderer> ().material.GetColor ("_EmissionColor");
	}

	ParticleSystem system;
	public void becameMana(){
		system = GetComponentInChildren<ParticleSystem> ();
		system.Play();
		system.transform.SetParent (null);
		Destroy (gameObject);
	}	


	public void setPlayer(Player player){
		this.player = player;
	}

	public void Die(){
		GameController.SetTriggerType(TriggerType.OnBeforeDeath, this);
		player.killCard (this);
	}

	public void AddSkill(Skill skills){
		Debug.LogWarning ("Adicionando skill " + skills.name + " para a carda " + name);
		Debug.LogWarning ("-----Comecou----");
		if(Character!=null){
			Debug.LogWarning ("1");
			card.AddSkill (skills);
		}
		Debug.LogWarning ("-----Terminou----");
	}

	public void Close(){
		if(isBeingHeroVisualized){
			isBeingHeroVisualized = false;
		}
		if(isBeingVisualized){
			player.ResetPreviewMana ();
			isBeingVisualized = false;
		}
	}

	public void setCharacterSpawnArea(Vector3 area){
		GameObject res = Resources.Load<GameObject> ("Prefabs/Characters/" + player.civilization.ToString () + "/" + card.name.Replace (" ", ""));
		if(res==null){
			Debug.LogError("Could not instantiate: "+player.civilization.ToString () + "/" + card.name.Replace (" ", ""));
		}
		area = Grid.GridToUnity(Grid.UnityToGrid(area));
		Character = ((GameObject)Instantiate (res, area, Quaternion.identity)).AddComponent<Hero> ();

		Character.setCard(card, player);
		Character.Initialize(this);
		GameConfiguration.PlaySFX(GameConfiguration.Summon);
		GameController.SetTriggerType (TriggerType.OnAfterSpawn, this);
	}
	public void setCard (int cardID, int playID, Player player){
		if (card.PlayID <= 0) {
			card = new Card (cardID);
			card.PlayID = playID;
			this.player = player;
			GameObject cardObj = Resources.Load<GameObject> ("Prefabs/Cards/"+player.civilization.ToString()+"/"+card.name.Replace(" ", ""));
			GameObject aux;
			if (cardObj == null) {
				Debug.Log(card.name.Replace(" ", "")+" não invocado. Civ:"+player.civilization.ToString());
				cardObj = Resources.Load<GameObject> ("Prefabs/Cards/Aeterna/AnittaAshervind");
			}
			aux = (GameObject)Instantiate (cardObj, Vector3.zero, Quaternion.identity);
			aux.transform.SetParent (this.transform);
			aux.transform.localPosition = Vector3.zero;
			aux.transform.localRotation = Quaternion.Euler (Vector3.zero);
		}
	}
}