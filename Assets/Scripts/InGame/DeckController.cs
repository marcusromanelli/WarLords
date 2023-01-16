using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DeckController : MonoBehaviour {


	[SerializeField] TextMesh deckCounter;


	private Player player;
	private int currentDeckSize;
	private GameObject deckObject;





	public bool isMouseOver;
	
	Stack<GameObject> DeckCards;
	GameObject aux;
	float value = 0;
	RaycastHit[] results;
	int layerMask;


	void Start(){
		DeckCards = new Stack<GameObject> ();
	}

	public void Setup(Player player)
    {
		this.player = player;


		InstantiateDeck();
	}

	void InstantiateDeck()
    {
		var coverTemplate = Resources.Load<GameObject>("Prefabs/CardBackCover" + ((int)player.GetCivilization()));

		deckObject = Instantiate(coverTemplate);
		deckObject.transform.SetParent(transform, true);
		deckObject.transform.localPosition = Vector3.zero;
		deckObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
		currentDeckSize = 0;
	}

	void SetDeckSize(int newSize)
	{
		if (newSize == 0)
			return;

		var scale = deckObject.transform.localScale;

		scale.z += newSize;

		deckObject.transform.localScale = scale;

		currentDeckSize += newSize;
	}
	void SyncCardNumber()
	{
		var realDeckCount = player.GetCurrentPlayDeckCount();

		var cardDelta = realDeckCount - currentDeckSize;

		SetDeckSize(cardDelta);

		deckCounter.text = realDeckCount + "\n" + ((realDeckCount > 1) ? "Cards" : "Card");
	}

	void CheckMouseOver()
    {
		if (!isMouseOver || !Input.GetMouseButtonDown(0))
			return;

		if (GameController.Singleton.currentPhase == Phase.Draw && GameController.Singleton.GetCurrentPlayer() == player)
		{
			if (!player.HasDrawnCard())
			{
				player.SetDrawnCard(true);
				player.DrawCard();

				GameController.Singleton.goToPhase(Phase.Action, player.GetCivilization());
			}
			else
			{
				if (!player.hasCondition(ConditionType.DrawCard) && !Application.isEditor)
				{
					Debug.LogWarning("You already drawn your card this turn");
				}
				else
				{
					player.DrawCard();
				}
			}
		}
		else
		{
			if (!player.hasCondition(ConditionType.DrawCard) && !Application.isEditor)
			{
				GameConfiguration.PlaySFX(GameConfiguration.denyAction);
				Debug.LogWarning("You only draw a card in your Drawn Phase");
			}
			else
			{
				player.DrawCard();
			}
		}

		layerMask = 1 << gameObject.layer;
		results = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 1000, layerMask);
		if (results.ToList().FindAll(a => a.collider.gameObject == this.gameObject).Count > 0)
		{
			isMouseOver = true;
		}
		else
		{
			isMouseOver = false;
		}
	}

	void Update () {
		SyncCardNumber();

		CheckMouseOver();
	}


	public int getNumberOfCards(){
		if (DeckCards == null) {
			DeckCards = new Stack<GameObject> ();
		}
		return DeckCards.Count;
	}

	public Vector3 getTopPosition(){
		return transform.position + Vector3.up * deckObject.transform.localScale.y;
	}

	public Quaternion getTopRotation(){
		return Quaternion.Euler (90, 0, 0);
	}

	public Vector3 getTopScale(){
		return Vector3.one;
	}
}
