using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpawnArea : PlaceableCard
{
	[SerializeField] Battlefield battlefield;
	[SerializeField] new Renderer renderer;
	[SerializeField] Color defaultColor = Color.grey;
	[SerializeField] Color selectedColor = Color.black;
	public PlayerType playerType { get; private set; }
	public bool IsSummonable { get; private set; }
	public bool TemporarilySummonable;
	public Hero Hero = null;


	

    private void Awake()
    {
		if (renderer == null)
			renderer = gameObject.GetComponent<Renderer>();
	}
    void Start()
	{
		if (Application.isEditor)
			return;

		SetColor(defaultColor);
	}
	public void Setup(Battlefield battlefield, bool IsSummonableTile, PlayerType playerType)
    {
		this.battlefield = battlefield;
		IsSummonable = IsSummonableTile;

		this.playerType = playerType;
	}
	void SetColor(Color color)
	{
		renderer.material.color = color;
	}
	protected override void Update()
	{
		if (!Application.isPlaying)
			return;

		player = GameController.Singleton.currentPlayer;

		SetColor(defaultColor);

		if (player == null || !player.hasCondition(ConditionType.PickSpawnArea))
        {
			CheckMouse();
			return;
		}


		var isLocal = player.GetPlayerType() == PlayerType.Local;

		isMouseOver = base.CheckMouseOver(false);

		if ((isLocal || TemporarilySummonable) && isMouseOver && Hero == null)
			SetColor(selectedColor);
	}
	protected void CheckMouse()
	{
		isMouseOver = base.CheckMouseOver(true);


		if (isMouseOver && Hero == null)
		{
			battlefield.SetSelectedTile(this);
		}
		else
		{
			isMouseOver = false;

			if (battlefield.GetSelectedTile() == this)
				battlefield.SetUnselectedTile(this);
		}
	}
	public override Vector3 GetTopPosition()
	{
		Vector3 aux = transform.position;
		aux.z -= renderer.bounds.size.z / 2.5f;
		aux.y += 0.1f;
		return aux;
	}
    public override Quaternion GetTopRotation()
    {
        return Quaternion.Euler(270, 180, 0);
    }
    public void SetHero(Hero hero)
	{
		Hero = hero;
	}
	public bool HasHero()
    {
		return Hero != null;
    }
}