using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SpawnArea : PlaceableCard {

	public static SpawnArea selected;

	public bool LocalPlayer;
	public bool canBeUsedToSpawn;
	public bool doesHaveHero;

	Color @default = Color.grey;
	Color max = Color.black;
	new Renderer renderer;

	void Start () {
		renderer = GetComponent<Renderer>();
		@default = new Color (0.66f, 0.66f, 0.66f, 1);

		renderer.material.color = @default;
	}


	protected void Update ()
	{

		if (GameController.Singleton.currentPhase == Phase.Action)
		{
			doesHaveHero = Physics.CheckSphere(transform.position, 0.3f, 1 << LayerMask.NameToLayer("Hero"));
		}

		if (!GameController.Singleton.MatchHasStarted)
		{
			renderer.material.color = @default;
			return;
		}

		player = GameController.Singleton.currentPlayer;

		if (player.hasCondition(ConditionType.PickSpawnArea))
		{
			var isLocal = player.GetPlayerType() == PlayerType.Local;

			if (isLocal || canBeUsedToSpawn)
			{
				isMouseOver = base.CheckMouseOver(false);

				if (isMouseOver && !Physics.CheckSphere(transform.position, 0.3f, 1 << LayerMask.NameToLayer("Hero"))) 
				{
					renderer.material.color = max;
				}
				else
				{
					renderer.material.color = @default;
				}
			}
			else
			{
				renderer.material.color = @default;
			}
		}
		else
		{

			CheckMouse();

			renderer.material.color = @default;
		}
	}

	protected void CheckMouse()
    {
		isMouseOver = base.CheckMouseOver(true);


		if (isMouseOver && !Physics.CheckSphere(transform.position, 0.3f, 1 << LayerMask.NameToLayer("Hero")))
		{ 
			isMouseOver = true;

			selected = this;
		}
		else
		{
			isMouseOver = false;

			if (selected == this)
			{
				selected = null;
			}
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