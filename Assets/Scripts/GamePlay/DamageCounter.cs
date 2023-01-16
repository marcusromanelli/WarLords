using UnityEngine;
using System.Collections;

public class DamageCounter : MonoBehaviour {

	public int value;
	public Vector3 scaleAux, positionAux;
	public TextMesh text;

	public static void New(Vector3 position, int value){
		GameObject aux = (GameObject)Instantiate (Resources.Load<GameObject> ("Misc/DamageCounter"), position, Quaternion.identity);
		aux.GetComponent<DamageCounter> ().value = value;
	}
	// Use this for initialization
	void Start () {
		scaleAux = transform.localScale;
		positionAux = transform.position;
		text = GetComponent<TextMesh> ();

		text.text = ((value>0)?"+":"")+value.ToString ();
	}

	// Update is called once per frame
	void Update () {
		if (transform.localScale.x > 0.01f) {
				positionAux.y--;
			transform.position = Vector3.MoveTowards (transform.position, positionAux, Time.deltaTime * 3);
	
			transform.localScale = Vector3.MoveTowards (transform.localScale, Vector3.zero, Time.deltaTime * 1.1f);
		} else {
			Destroy (gameObject);
		}
	}
}
