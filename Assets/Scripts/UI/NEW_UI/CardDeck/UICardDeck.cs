using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardDeck : MonoBehaviour, ICardPlaceable
{    enum DeckActionType { AddCard, DrawCard }

    [SerializeField] TextMesh cardCounter;
    [SerializeField] Transform cardReferencePosition;
    [SerializeField] float distanceBetweenCards = 0.02f;
    [SerializeField, ReadOnly] bool isBusy;

    List<DeckActionType> remainingActions = new List<DeckActionType>();
    List<CardObject> deckObjects = new List<CardObject>();
    List<Card> cardsToAdd = new List<Card>();
    public bool IsBusy => isBusy;

    public void Add(Card card)
    {
        remainingActions.Add(DeckActionType.AddCard);
        cardsToAdd.Add(card);

        StartSolvingActions();
    }
    public IEnumerator IsResolving()
    {
        while (IsBusy)
        {
            yield return null;
        }
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
        RemoveCards(deckObjects.Count);
    }
    void StartSolvingActions()
    {
        if (IsBusy)
            return;

        StartCoroutine(SolveActions());
    }
    IEnumerator SolveActions()
    {
        isBusy = true;

        while (remainingActions.Count > 0)
        {
            var nextAction = remainingActions[0];
            switch (nextAction)
            {
                case DeckActionType.AddCard:
                    AddCardAction();
                    break;
                case DeckActionType.DrawCard:
                    RemoveCardAction();
                    break;
            }
            yield return null;
            remainingActions.RemoveAt(0);
        }

        isBusy = false;
    }
    void AddCardAction()
    {
        var card = cardsToAdd[0];
        CardObject cardObject = CardFactory.CreateCard(card, transform, true);
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

    /*[Button("Add 1 Card", EButtonEnableMode.Playmode)]
    public void Editor_Add_1_Card()
    {
        UpdateDeckSize(deckObjects.Count + 1);
    }
    [Button("Add 2 Cards", EButtonEnableMode.Playmode)]
    public void Editor_Add_2_Card()
    {
        UpdateDeckSize(deckObjects.Count + 2);
    }
    [Button("Draw 1 Card", EButtonEnableMode.Playmode)]
    public void Editor_Draw_1_Card()
    {
        UpdateDeckSize(deckObjects.Count - 1);
    }
    [Button("Add 2 Cards", EButtonEnableMode.Playmode)]
    public void Editor_Draw_2_Card()
    {
        UpdateDeckSize(deckObjects.Count - 2);
    }*/
}
