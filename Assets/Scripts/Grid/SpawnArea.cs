using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SpawnArea : PlaceableCard {

	public static SpawnArea selected;

	Player player;
	public bool LocalPlayer;
	public bool canBeUsedToSpawn;
	public bool doesHaveHero;

	Color defaul = Color.grey;
	Color max = Color.black;
	new Renderer renderer;
	bool OnMouseOver;
	int layerMask;
	RaycastHit[] results;
	bool isMouseOver;

	void Start () {
		renderer = GetComponent<Renderer>();
		defaul = new Color (0.66f, 0.66f, 0.66f, 1);
		renderer = GetComponent<Renderer> ();
		renderer.material.color = defaul;
	}


	protected void Update () {

		if (GameController.Singleton.currentPhase == Phase.Action) {
			doesHaveHero = Physics.CheckSphere (transform.position, 0.3f, 1 << LayerMask.NameToLayer ("Hero"));
		}

		if (!GameController.Singleton.MatchHasStarted)
        {
			renderer.material.color = defaul;
			return;
		}


		player = GameController.Singleton.GetCurrentPlayer ();

		if (!player.hasCondition(ConditionType.PickSpawnArea))
			return;

		base.CheckMouseOver(false);



		if (isMouseOver)
		{
			Debug.Log("Over spawn area");
			selected = this;
		}
		else if (selected == this)
				selected = null;

		if (player.GetPlayerType() == PlayerType.Remote || (player.GetPlayerType() == PlayerType.Local && canBeUsedToSpawn))
		{
			layerMask = 1 << gameObject.layer;
			results = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000, layerMask);
			if (results.ToList().FindAll(a => a.collider.gameObject == this.gameObject).Count > 0)
			{
				if (!Physics.CheckSphere(transform.position, 0.3f, 1 << LayerMask.NameToLayer("Hero")))
				{
					renderer.material.color = max;
				}
			}
			else
			{
				renderer.material.color = defaul;
			}
		}
		else
		{
			renderer.material.color = defaul;
		}
	}


	public override Vector3 GetTopPosition(){
		Vector3 aux = transform.position;
		aux.z-=renderer.bounds.size.z/2.5f;
		aux.y+=0.1f;
		return aux;
	}

	public override Quaternion GetTopRotation(){
		return Quaternion.Euler (270, 180, 0);
	}
}