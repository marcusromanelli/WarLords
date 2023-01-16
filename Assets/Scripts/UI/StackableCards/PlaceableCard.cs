using UnityEngine;
using System.Linq;
using System;

public class PlaceableCard : MonoBehaviour
{
	[SerializeField] Transform CardReferencePosition;
	public virtual bool IsMouseOver { get { return isMouseOver; } }

	protected Func<int> GetCardCount;
	protected Player player;
	protected bool isMouseOver;
	protected RaycastHit[] results;

	public virtual void Setup(Player player)
	{
		this.player = player;
	}
	public virtual void Setup(Player player, Func<int> getCardCount)
	{
		this.player = player;
		this.GetCardCount = getCardCount;
	}

	protected virtual void Update()
    {
		isMouseOver = CheckMouseOver(true);
    }

	protected virtual bool CheckMouseOver(bool needsMouseClick)
	{
		if (needsMouseClick && !Input.GetMouseButton(0))
			return false;

		string layerMask = LayerMask.LayerToName(this.gameObject.layer);
		int layerMaskFinal = LayerMask.GetMask(layerMask);


		results = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000, layerMaskFinal);

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

	public virtual Vector3 GetTopPosition()
	{
		return CardReferencePosition.transform.position;
	}

	public virtual Quaternion GetTopRotation()
	{
		return CardReferencePosition.transform.rotation;
	}


}
