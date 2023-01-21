using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIDeckType
{
    MainDeck, ManaPool, Graveyard
}
public class UICardDeck : MonoBehaviour
{    enum DeckActionType { AddCard, DrawCard }

    [SerializeField] UIDeckType type;
    [SerializeField] Transform cardReferencePosition;
    [SerializeField] float distanceBetweenCards = 0.02f;
    [SerializeField, ReadOnly] bool isBusy;

    List<DeckActionType> remainingActions = new List<DeckActionType>();
    List<CardObject> deckObjects = new List<CardObject>();
    Civilization civilization;
    public bool IsBusy => isBusy;
    private int targetDeckSize;
    public void Setup(Civilization civilization)
    {
        this.civilization = civilization;
    }
    public void UpdateDeckSize(int newSize)
    {
        if (newSize == targetDeckSize)
            return;

        var deltaSize = targetDeckSize - newSize;

        if (deltaSize < 0)
            AddCards(Mathf.Abs(deltaSize));
        else
            RemoveCards(deltaSize);

        StartSolvingActions();
    }
    public IEnumerator IsResolving()
    {
        while (IsBusy)
        {
            yield return null;
        }
    }

    void AddCards(int count)
    {
        targetDeckSize += count;

        for (int i = 0; i < count; i++)
            remainingActions.Add(DeckActionType.AddCard);
    }
    void RemoveCards(int count)
    {
        targetDeckSize -= count; 

        for (int i = 0; i < count; i++)
            remainingActions.Add(DeckActionType.DrawCard);
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
        CardObject cardObject = CardFactory.CreateEmptyCard(civilization, transform);
        cardObject.transform.rotation = GetReferenceRotation();
        cardObject.transform.position = GetTopCardPosition();

        deckObjects.Add(cardObject);
    }
    void RemoveCardAction()
    {
        if (deckObjects.Count <= 0)
            return;

        var lastPosition = deckObjects.Count - 1;
        var card = deckObjects[lastPosition];

        CardFactory.AddCardToPool(card);

        deckObjects.RemoveAt(lastPosition);
    }
    public Vector3 GetTopCardPosition()
    {
        var verticalPosition = deckObjects.Count * distanceBetweenCards;

        return cardReferencePosition.position + Vector3.up * verticalPosition;
    }
    public Quaternion GetReferenceRotation()
    {
        return cardReferencePosition.transform.rotation;
    }

    [Button("Add 1 Card", EButtonEnableMode.Playmode)]
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
    }
}
