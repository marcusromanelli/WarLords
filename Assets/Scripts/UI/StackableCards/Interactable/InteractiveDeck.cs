
using System.Linq;
using UnityEngine;

public class InteractiveDeck : StackableCards, ICardPlaceable
{
	protected bool isMouseOver;
	[SerializeField] protected Player localPlayerController;
	public virtual bool IsMouseOver { get { return isMouseOver; } }

	public Player Player { get; private set; }

    protected void Awake()
	{
		//localPlayerController.OnReleaseCard += OnReleaseCard;
	}
	public void Setup(Player player)
    {
		Player = player;
    }
	protected virtual void OnReleaseCard(CardObject cardObject)
    {

    }

	bool IsMouseHover(bool needsMouseClick)
	{
		if (needsMouseClick && !Input.GetMouseButton(0))
			return false;

		string layerMask = LayerMask.LayerToName(this.gameObject.layer);
		int layerMaskFinal = LayerMask.GetMask(layerMask);


		var results = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000, layerMaskFinal);

		var finalResults = results.ToList();

		if (finalResults.FindAll(a => a.collider.gameObject == this.gameObject).Count > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public virtual void Update()
    {
		CheckMouseOver(true);
	}
	public void CheckMouseOver(bool needsMouseClick)
	{
		isMouseOver = IsMouseHover(needsMouseClick);

		if (isMouseOver && Input.GetMouseButtonDown(0))
			OnClick();
	}
	public void OnClick()
	{

	}
}
