using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnArea : MonoBehaviour
{
	[SerializeField] Battlefield battlefield;
	[SerializeField] GameController gameController;
	[SerializeField] new Renderer renderer;
	[SerializeField] Color defaultColor = Color.grey;
	[SerializeField] Color selectedColor = Color.black;
	[ReadOnly] public PlayerType playerType = PlayerType.None;
	public bool IsTemporarilySummonable;
	public Hero Hero = null;

	Color lastUsedColor;
	/// <summary>
	/// Returns if a tile is a player summon area
	/// </summary>
	public bool IsSummonArea
	{
		get
		{
			return playerType == PlayerType.Local;
		}
	}
	/// <summary>
	/// Returns if the tile is allowed to receive a summon
	/// </summary>
	public bool IsSummonable
	{
		get
		{
			return (IsSummonArea || IsTemporarilySummonable) && Hero == null;
		}
	}

	private void Awake()
    {
		if (renderer == null)
			renderer = gameObject.GetComponent<Renderer>();
	}
    void Start()
	{
		if (!Application.isPlaying)
			return;

		if(IsSummonArea)
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
		if (color == lastUsedColor)
			return;

		renderer.material.color = color;
		lastUsedColor = color;
	}
	protected void Update()
	{
		if (!Application.isPlaying)
			return;

		//CheckMouse();

		//UpdateColor();
	}

    /*void UpdateColor()
	{
		var player = gameController.GetCurrentPlayer();

		if (player == null)
			return;

		if (!IsTemporarilySummonable && !IsSummonArea)
			return;

		var isLocal = player.GetPlayerType() == PlayerType.Local;
		var isPickingLocation = player.hasCondition(ConditionType.PickSpawnArea);

		isMouseOver = base.CheckMouseOver(false);

		if ((isLocal || IsTemporarilySummonable) && isPickingLocation && isMouseOver && Hero == null)
			SetColor(selectedColor);
		else
			SetColor(defaultColor);
	}*/
	/*void CheckMouse()
	{
		if (player != null && player.hasCondition(ConditionType.PickSpawnArea))
			return;

		isMouseOver = base.CheckMouseOver(true);

		if(IsMouseOver && Input.GetMouseButtonUp(0))
		{
			battlefield.ClickedTile(this);
		}

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
    }*/
	public void SetHero(Hero hero)
	{
		Hero = hero;
	}
	public bool HasHero()
    {
		return Hero != null;
    }
}