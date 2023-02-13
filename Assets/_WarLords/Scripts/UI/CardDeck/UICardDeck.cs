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
    [SerializeField] Transform cardReferencePosition;
    [SerializeField] float distanceBetweenCards = 0.02f;
    [SerializeField, ReadOnly] bool isBusy;
    public bool IsBusy => isBusy;

    private List<DeckActionType> remainingActions = new List<DeckActionType>();
    private List<CardObject> deckObjects = new List<CardObject>();
    private List<Card> cardsToAdd = new List<Card>();
    private InputController inputController;

    public void Setup(InputController InputController)
    {
        inputController = InputController;
    }
    public void Add(Card card)
    {
        //AddCardAction(cards);
        remainingActions.Add(DeckActionType.AddCard);
        cardsToAdd.Add(card);

        StartSolvingActions();
    }
    public IEnumerator IsResolving()
    {
        while (IsBusy && remainingActions.Count <= 0 && cardsToAdd.Count <= 0)
            yield return null;
    }
    void UpdateCardCount()
    {
        if (cardCounter == null)
            return;

        cardCounter.text = deckObjects.Count.ToString();
    }

    public void RemoveCards(int count)
    {
        for (int i = 0; i < count; i++)
            remainingActions.Add(DeckActionType.DrawCard);

        StartSolvingActions();
    }
    public void RemoveAll()
    {
        RemoveCards(deckObjects.Count + cardsToAdd.Count);
    }
    public void Shuffle(Card[] cards)
    {
        var cardList = cards.ToList();
        deckObjects.OrderBy(cardObject => cardList.FindIndex(card => card == cardObject.Data));
    }
    public void SendCardToDeckFromPosition(CardObject cardObject, CardPositionData fromPosition, Action onFinish)
    {
        cardObject.transform.position = fromPosition.Position;
        cardObject.transform.rotation = fromPosition.Rotation;

        cardObject.SetPosition(CardPositionData.Create(GetTopCardPosition(), GetRotationReference()), () => {
            CardFactory.AddToPool(cardObject);

            onFinish?.Invoke();
        });
    }
    void StartSolvingActions()
    {
        if(!isBusy)
            StartCoroutine(SolveActions());
    }
    IEnumerator SolveActions()
    {
        isBusy = true;

        while (true)
        {
            try
            {
                DeckActionType nextAction = remainingActions[0];

                switch (nextAction)
                {
                    case DeckActionType.AddCard:
                        AddCardAction();
                        break;
                    case DeckActionType.DrawCard:
                        RemoveCardAction();
                        break;
                }

                remainingActions.RemoveAt(0);
            }
            catch (Exception)
            {
            }

            yield return null;
        }
    }
    CardObject CreateCard(Card cardData)
    {
        return CardFactory.CreateCard(inputController, cardData, transform, true);
    }
    void AddCardAction()
    {
        var cardData = cardsToAdd[0];
        CardObject cardObject = CreateCard(cardData);
        cardsToAdd.RemoveAt(0);

        cardObject.transform.rotation = GetRotationReference();
        cardObject.transform.position = GetTopCardPosition();

        deckObjects.Add(cardObject);

        UpdateCardCount();
    }
    void RemoveCardAction()
    {
        if (deckObjects.Count <= 0)
            return;

        var lastPosition = deckObjects.Count - 1;
        var card = deckObjects[lastPosition];

        CardFactory.AddToPool(card);

        deckObjects.RemoveAt(lastPosition);

        UpdateCardCount();
    }
    public Vector3 GetTopCardPosition()
    {
        var verticalPosition = deckObjects.Count * distanceBetweenCards;

        return cardReferencePosition.position + Vector3.up * verticalPosition;
    }
    public Quaternion GetRotationReference()
    {
        return cardReferencePosition.transform.rotation;
    }
}
