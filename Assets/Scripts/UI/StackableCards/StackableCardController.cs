using UnityEngine;
using System.Linq;
using System;

public class StackableCardController : PlaceableCard {


	[SerializeField] protected TextMesh deckCounter;

	
	protected int currentDeckSize;
	protected GameObject deckObject;
	protected Func<bool> CanClick;


	public void Setup(Player player, Func<int> getCardCount, Func<bool> canClick)
	{
		this.player = player;
		this.GetCardCount = getCardCount;
		this.CanClick = canClick;

		InstantiateDeck();
	}
	public override void Setup(Player player, Func<int> getCardCount)
	{
		this.player = player;
		this.GetCardCount = getCardCount;
		this.CanClick = null;

		InstantiateDeck();
	}

	protected virtual void InstantiateDeck()
    {
		var coverTemplate = Resources.Load<GameObject>("Prefabs/CardBackCover" + ((int)player.GetCivilization()));

		deckObject = Instantiate(coverTemplate);
		deckObject.transform.SetParent(transform, true);
		deckObject.transform.localPosition = Vector3.zero;
		deckObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
		currentDeckSize = 0;
	}

	protected virtual void SetDeckSize(int newSize)
	{
		if (newSize == 0)
			return;

		var scale = deckObject.transform.localScale;

		scale.z += newSize;

		if (scale.z < 0) scale.z = 0;

		deckObject.transform.localScale = scale;

		currentDeckSize += newSize;
	}
	protected virtual void SyncCardNumber()
	{
		var realDeckCount = GetCardCount();

		var cardDelta = realDeckCount - currentDeckSize;

		SetDeckSize(cardDelta);

		SetCounterText();
	}

	protected void SetCounterText()
	{
		if (deckCounter == null)
			return;

		deckCounter.text = currentDeckSize + "\n" + ((currentDeckSize > 1) ? "Cards" : "Card");
	}
	protected override void CheckMouseOver(bool needsMouseClick)
	{
		base.CheckMouseOver(needsMouseClick);

		if(isMouseOver && Input.GetMouseButtonDown(0) && CanClick())
			OnClick();
	}
	protected virtual void OnClick()
    {

    }

	protected override void Update ()
	{
		SyncCardNumber();

		base.Update();
	}


	public virtual int GetNumberOfCards(){
		return currentDeckSize;
	}

}
