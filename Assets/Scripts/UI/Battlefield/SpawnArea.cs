using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpawnArea : PlaceableCard
{
	[SerializeField] Battlefield battlefield;
	[SerializeField] GameController gameController;
	[SerializeField] new Renderer renderer;
	[SerializeField] Color defaultColor = Color.grey;
	[SerializeField] Color selectedColor = Color.black;
	public PlayerType playerType/* { get; private set; }*/ = PlayerType.None;
	public bool IsSummonable {
		get {
			return playerType != PlayerType.None;
		}
	}
	public bool TemporarilySummonable;
	public Hero Hero = null;	

    private void Awake()
    {
		if (renderer == null)
			renderer = gameObject.GetComponent<Renderer>();
	}
    void Start()
	{
		if (!Application.isPlaying)
			return;

		if(IsSummonable)
			SetColor(defaultColor);
	}
	public void Setup(Battlefield battlefield, GameController gameController, PlayerType playerType)
	{
		Setup(battlefield, gameController);

		this.playerType = playerType;

	}
	public void Setup(Battlefield battlefield, GameController gameController)
	{
		this.battlefield = battlefield;
		this.gameController = gameController;
	}

	void SetColor(Color color)
	{
		renderer.material.color = color;
	}
	protected override void Update()
	{
		if (!Application.isPlaying)
			return;

		CheckMouse();

		UpdateColor();
	}

	void UpdateColor()
	{
		var player = gameController.currentPlayer;

		if (player == null)
			return;

		var isLocal = player.GetPlayerType() == PlayerType.Local;
		var isPickingLocation = player.hasCondition(ConditionType.PickSpawnArea);

		isMouseOver = base.CheckMouseOver(false);

		if ((isLocal || TemporarilySummonable) && isPickingLocation && isMouseOver && Hero == null)
			SetColor(selectedColor);

	}
	void CheckMouse()
	{
		if (player != null && player.hasCondition(ConditionType.PickSpawnArea))
			return;

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