using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GraveyardController : MonoBehaviour {
	public float distanceBetweenCards = 0.02f;
	public bool isMouseOver;
	GameObject coverTemplate;
	Player player;
	Stack<GameObject> GraveyardCards;
	int layerMask;
	RaycastHit[] results;

	void Start(){
		GraveyardCards = new Stack<GameObject> ();
		player = GetComponentInParent<Player> ();
		coverTemplate = Resources.Load<GameObject> ("Prefabs/CardBackCover"+((int)player.civilization));
	}
		
	GameObject aux;
	void Update () {
		if (player.Graveyard != null) {
			if (player.Graveyard.Count < GraveyardCards.Count) {
				Destroy (GraveyardCards.Pop ().gameObject);
			} else if (player.Graveyard.Count > GraveyardCards.Count) {
				(aux = (GameObject)Instantiate (coverTemplate, Vector3.zero, Quaternion.Euler (90, 270, 0))).transform.position = transform.position + Vector3.up * (distanceBetweenCards * GraveyardCards.Count);
				GraveyardCards.Push (aux);
				aux.transform.SetParent (transform, true);
			}
		}

		layerMask = 1 << gameObject.layer;
		results = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition), 1000, layerMask);
		if (results.ToList().FindAll(a => a.collider.gameObject == this.gameObject).Count>0) {
			isMouseOver = true;
		}else{
			isMouseOver = false;
		}
	}

	public Vector3 getTopPosition(){
		return transform.position + Vector3.up * (distanceBetweenCards * ((GraveyardCards.Count>0)?GraveyardCards.Count:1));
	}

	public Quaternion getTopRotation(){
		return Quaternion.Euler (90, 270, 0);
	}

	public Vector3 getTopScale(){
		return Vector3.one;// coverTemplate.transform.localScale;
	}
}
