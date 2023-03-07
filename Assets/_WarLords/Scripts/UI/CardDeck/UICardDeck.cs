using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UICardDeck : MonoBehaviour, ICardPlaceable
{    enum DeckActionType { AddCard, DrawCard }

    [SerializeField] TMP_Text cardCounter;
    [SerializeField] Renderer deckObject;
    [SerializeField] float distanceBetweenCardsForScale = 1f;
    [SerializeField] float distanceBetweenCardsForPosition = 1f;
    [SerializeField, ReadOnly] bool isBusy;
    public bool IsBusy => isBusy;

    private List<Card> deckObjects = new List<Card>();

    public void Setup(CivilizationGraphicsData civilizationData)
    {
        deckObject.material.mainTexture = civilizationData.GetBackCoverTexture();
        UpdateCardCount();
    }
    public void Add(Card card)
    {
        AddCardAction(card);
    }
    void UpdateCardCount()
    {
        var newSize = CalculateDeckHeight();

        if (newSize.z == 0)
            deckObject.gameObject.SetActive(false);
        else
        {
            deckObject.gameObject.SetActive(true);
            deckObject.transform.localScale = newSize;
        }

        if(cardCounter != null)
            cardCounter.text = deckObjects.Count.ToString();
    }

    public void RemoveCards(int count)
    {
        for (int i = 0; i < count; i++)
            RemoveCardAction();
    }
    public void RemoveAll()
    {
        RemoveCards(deckObjects.Count);
    }
    public void Shuffle(Card[] cards)
    {
        var cardList = cards.ToList();
        deckObjects.OrderBy(cardObject => cardList.FindIndex(card => card == cardObject));
    }
    public void SendCardToDeckFromPosition(CardObject cardObject, CardPositionData fromPosition, Action onFinish)
    {
        cardObject.transform.SetParent(null, true);
        cardObject.gameObject.SetActive(true);
        cardObject.transform.position = fromPosition.Position;
        cardObject.transform.rotation = fromPosition.Rotation;

        cardObject.SetPosition(CardPositionData.Create(GetTopCardPosition(), GetRotationReference()), () => {
            CardFactory.AddToPool(cardObject);

            onFinish?.Invoke();
        });
    }
    void AddCardAction(Card card)
    {
        deckObjects.Add(card);

        UpdateCardCount();
    }
    void RemoveCardAction()
    {
        if (deckObjects.Count <= 0)
            return;

        var lastPosition = deckObjects.Count - 1;

        deckObjects.RemoveAt(lastPosition);

        UpdateCardCount();
    }
    public Vector3 GetTopCardPosition()
    {
        var verticalPosition = deckObjects.Count * distanceBetweenCardsForPosition;

        return deckObject.transform.position + Vector3.up * verticalPosition;
    }
    public Quaternion GetRotationReference()
    {
        return deckObject.transform.rotation;
    }
    Vector3 CalculateDeckHeight()
    {
        return new Vector3(1, 1, (distanceBetweenCardsForScale * deckObjects.Count));
    }
}
