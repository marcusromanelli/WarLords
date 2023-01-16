using UnityEngine;
using System.Collections;

public class CardsLibrary : MonoBehaviour {

	private static CardsLibrary _singleton;
	public static CardsLibrary Singleton{
		get{
			if (_singleton == null) {
				CardsLibrary aux = GameObject.FindObjectOfType<CardsLibrary> ();
				if (aux == null) {
					_singleton = (new GameObject ("-----Cards Library-----", typeof(CardsLibrary))).GetComponent<CardsLibrary> ();
				} else {
					_singleton = aux;
				}
			}
			DontDestroyOnLoad (_singleton.gameObject);
			return _singleton;
		}
	}


	public CardCollection Cards;
	public MacroCollection Macros;

	public bool Save;
	public bool Load;

	void Awake(){
		GameConfiguration.Void ();
	}

	void Start(){
		doLoad ();
	}

	void Update(){
		if (Save) {
			Save = false;
			doSave ();
		}
		if (Load) {
			Load = false;
			doLoad ();
		}
	}

	void doSave(){
		Macros.Save (false);	
		Cards.Save (false);
	}

	void doLoad(){
		Macros = MacroCollection.Load (true);
		Cards = CardCollection.Load (true);
	}
}