using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ManaPoolController : PlaceableCard
{
	[SerializeField] Transform BaseCenter;


	private Vector3 baseCenterPosition;

	public float distanceBetweenColumns = 1f;
	public float distanceBetweenLines = 1f;
	GameObject Mana;
	List<CardObject> ManaPoolCards;
	GameObject aux;


	void Start()
	{
		ManaPoolCards = new List<CardObject>();
		Mana = Resources.Load<GameObject>("Prefabs/Mana");

		baseCenterPosition = BaseCenter.transform.position;
	}

	void Update()
	{
		var playerManaPoolUnused = player.GetCurrentManaPoolCount();

		if (ManaPoolCards == null) ManaPoolCards = new List<CardObject>();

		if (playerManaPoolUnused < ManaPoolCards.Count)
		{
			Destroy(ManaPoolCards[ManaPoolCards.Count - 1].gameObject);
			ManaPoolCards.RemoveAt(ManaPoolCards.Count - 1);
		}
		else if (playerManaPoolUnused > ManaPoolCards.Count)
		{
			addCard();
		}

		base.Update();
	}

	public void spendMana(int number)
	{
		int c = 0;
		for (int i = 0; i < ManaPoolCards.Count; i++)
		{
			if (ManaPoolCards[i].cardData.manaStatus == ManaStatus.Active && c != number)
			{
				ManaPoolCards[i].cardData.manaStatus = ManaStatus.Used;
				c++;
			}
		}
	}

	public void previewMana(int number)
	{
		for (int i = 0; i < ManaPoolCards.Count; i++)
		{
			switch (ManaPoolCards[i].cardData.manaStatus)
			{
				case ManaStatus.Active:
				case ManaStatus.Preview:
					if (i < number)
					{
						ManaPoolCards[i].cardData.manaStatus = ManaStatus.Preview;
					}
					else
					{
						ManaPoolCards[i].cardData.manaStatus = ManaStatus.Active;
					}
					break;
			}
		}
	}

	public void recoverPreviewMana()
	{
		for (int i = 0; i < ManaPoolCards.Count; i++)
		{
			if (ManaPoolCards[i].cardData.manaStatus == ManaStatus.Preview)
			{
				ManaPoolCards[i].cardData.manaStatus = ManaStatus.Active;
			}
		}
	}

	public void recoverMana(int number)
	{
		int c = 0;
		foreach (CardObject card in ManaPoolCards)
		{
			if (card.cardData.manaStatus == ManaStatus.Used && c != number)
			{
				card.cardData.manaStatus = ManaStatus.Active;
				c++;
			}
		}
	}

	float calculateZ(int number)
	{
		return (number % 6);
	}
	Vector3 calculateNextPosition()
	{
		int value = ManaPoolCards.Count + 1;
		if (value >= 12)
			value = 12;

		Vector3 pos = transform.position;

		if (value <= 6)
		{
			pos.x -= distanceBetweenColumns;
		}
		else
		{
			pos.x += distanceBetweenColumns;
		}

		pos.z += transform.forward.z * calculateZ(value - 1) * distanceBetweenLines;
		return pos;
	}
	void addCard()
	{
		Vector3 next = calculateNextPosition();
		aux = (GameObject)Instantiate(Mana, Vector3.zero, Quaternion.Euler(270, 0, 180));
		//aux.transform.localScale /= 1.2f;
		aux.transform.position = next;
		aux.AddComponent<CardObject>().SetMana();
		aux.GetComponent<CardObject>().SetPlayer(player);
		ManaPoolCards.Add(aux.GetComponent<CardObject>());
		aux.transform.SetParent(transform, true);
	}

	public Vector3 GetBasePosition()
	{
		return baseCenterPosition;
	}
}