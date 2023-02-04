using UnityEngine;
using System.Collections;

public class GameConfiguration : MonoBehaviour {
	private static GameConfiguration _singleton;
	public static GameConfiguration Singleton{
		get{
			if (_singleton == null) {
				GameConfiguration aux = GameObject.FindObjectOfType<GameConfiguration> ();
				if (aux == null) {
					_singleton = (new GameObject ("-----Game Configs-----", typeof(GameConfiguration))).GetComponent<GameConfiguration> ();
				} else {
					_singleton = aux;
				}
			}
			DontDestroyOnLoad (_singleton.gameObject);
			return _singleton;
		}
	}

	//Components
	AudioSource audioSource;

	public static int numberOfInitialDrawnCards = 8;
	public static int numberOfInitialMana = 3;
	public static int maxNumberOfCardsInHand = 8;
	public static int maxNumberOfCardsInManaPool = 12;
	public static int startLife = 15;

	//OST
	public static AudioClip backgroundMusic;

	//Audios
	public static AudioClip drawCard;
	public static AudioClip buffCard;
	public static AudioClip energyToCard;
	public static AudioClip cardToEnergy;
	public static AudioClip confirmAction;
	public static AudioClip debuff;
	public static AudioClip denyAction;
	public static AudioClip explosion;
	public static AudioClip fireMagic;
	public static AudioClip heal;
	public static AudioClip movementBuff;
	public static AudioClip useEnergy;
	public static AudioClip Summon;
	public static AudioClip Walk;
	public static AudioClip Death;
	public static AudioClip Hit;

	public AudioClip _drawCard;

	static Texture2D _closeHandCursor;
	public static Texture2D closeHandCursor {
		get{
			if (_closeHandCursor == null) {
				_closeHandCursor = Resources.Load<Texture2D> ("Textures/Cursor/closeHand");
			}
			return _closeHandCursor;
		}
		private set{
			_closeHandCursor = value;
		}
	}


	static Texture2D _openHandCursor;
	public static Texture2D openHandCursor {
		get{
			if (_openHandCursor == null) {
				_openHandCursor = Resources.Load<Texture2D> ("Textures/Cursor/openHand");
			}
			return _openHandCursor;
		}
		private set{
			_openHandCursor = value;
		}
	}

	public void Awake(){
		//SetCursor (CursorType.HandOpen);

		drawCard = Resources.Load<AudioClip>("SFX/drawCard");
		_drawCard = drawCard;

		cardToEnergy = Resources.Load<AudioClip>("SFX/cardToEnergy");
		buffCard = Resources.Load<AudioClip>("SFX/buffCard");
		confirmAction = Resources.Load<AudioClip>("SFX/confirmAction");
		debuff = Resources.Load<AudioClip>("SFX/debuff");
		denyAction = Resources.Load<AudioClip>("SFX/denyAction");
		explosion = Resources.Load<AudioClip>("SFX/explosion");
		fireMagic = Resources.Load<AudioClip>("SFX/fireMagic");
		heal = Resources.Load<AudioClip>("SFX/heal");
		movementBuff = Resources.Load<AudioClip>("SFX/movementBuff");
		useEnergy = Resources.Load<AudioClip>("SFX/useEnergy");
		Summon = Resources.Load<AudioClip>("SFX/Summon");
		Walk = Resources.Load<AudioClip>("SFX/Walk");
		Death = Resources.Load<AudioClip>("SFX/Death");
		Hit = Resources.Load<AudioClip>("SFX/Hit");
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	public static void Void(){ Singleton._void (); }

	protected void _void(){}


	public static void PlaySFX(AudioClip audio){
	 	GameConfiguration.Singleton.audioSource.PlayOneShot(audio);
	}

	void Update(){
		/*if (Input.GetMouseButton(0)) {
			SetCursor (CursorType.HandClosed);
		} else {
			SetCursor (CursorType.HandOpen);
		}*/
	}

	static CursorType CursorType;
	public static CursorType GetCursor(){
		return CursorType;
	}

	public static void SetCursor(CursorType cursor){
		switch (cursor) {

		case CursorType.HandClosed:
			Cursor.SetCursor (GameConfiguration.closeHandCursor, new Vector2 (20, 20), CursorMode.Auto);
			break;
		default:
		case CursorType.HandOpen:
			Cursor.SetCursor (GameConfiguration.openHandCursor, new Vector2 (20, 20), CursorMode.Auto);
			break;
		}
		CursorType = cursor;
	}
}
