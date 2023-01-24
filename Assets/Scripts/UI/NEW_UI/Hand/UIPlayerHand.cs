using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerHand : MonoBehaviour
{
    public bool IsBusy => isBusy;


    [SerializeField] bool IsInteractable = true;
    [SerializeField] float awaitTimeBetweenDraws = 0f;
    [SerializeField, ShowIf("IsInteractable")] UICardDeck uiCardDeck;
    [SerializeField, ShowIf("IsInteractable")] BezierCurve bezierCurve;
	[SerializeField, ShowIf("IsInteractable")] AnimationCurve curveRange;

    [BoxGroup("Presets"), SerializeField, ShowIf("IsInteractable")] CardObjectData visualizeCardPositionOffset;
	[BoxGroup("Presets"), SerializeField, ShowIf("IsInteractable")] CardObjectData draggingCardRotationOffset;
    [BoxGroup("Presets"), SerializeField, ShowIf("IsInteractable")] Vector3 DeckRotation;

    List<CardObject> cardList = new List<CardObject>();
    private bool isBusy;
    protected Civilization civilization;

    private InputController inputController;
    private CardObject currentTargetCard;
    private CardObjectData storedOriginalCardObjectData;
    private bool isDraggingObject;


    public void PreSetup(InputController inputController)
    {
        this.inputController = inputController;
    }
    public void Setup(Civilization civilization)
    {
        this.civilization = civilization;
    }
    public void AddCard(Card card)
    {
        var cardObj = CardFactory.CreateCard(card, transform, uiCardDeck.GetTopCardPosition());
        cardObj.SetPositionAndRotation(CardObjectData.Create(uiCardDeck.GetTopCardPosition(), Quaternion.Euler(DeckRotation)));

        cardList.Add(cardObj);

        if (IsInteractable)
        {
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseButtonUp, cardObj.gameObject, OnUpCard);
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseDragStart, cardObj.gameObject, OnDragCardStart);
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseDragEnd, cardObj.gameObject, OnDragCardEnd);
        }

        GameConfiguration.PlaySFX(GameConfiguration.drawCard);

        StartCoroutine(UpdatePositions());
    }
    public void AddCards(Card[] cards)
    {
        foreach(Card card in cards)
            AddCard(card);
    }

    IEnumerator UpdatePositions()
    {
        var numberOfCards = cardList.Count;

        if (numberOfCards <= 0)
            yield break;

        var spaceBetween = 0.5f / numberOfCards;
        var positionCount = spaceBetween;
        var verticalBuildUp = 0.01f;

        for (int i = 0; i < numberOfCards; i++)
        {
            var card = cardList[i];
            var normalizedPosition = positionCount * 2;

            var position = bezierCurve.GetPointAt(normalizedPosition);
            position.y += verticalBuildUp * i; //Rise every card by a little to avoid Z-fight

            var rotationValue = curveRange.Evaluate(normalizedPosition);
            rotationValue *= 10;//Curve is stored in X/10 value

            var rotation = Quaternion.Euler(-90, 0, rotationValue);

            card.SetPositionAndRotation(CardObjectData.Create(position, rotation));

            positionCount += spaceBetween;

            yield return AwaitCardInPlace(card);

            yield return new WaitForSeconds(awaitTimeBetweenDraws);
        }
    }
    IEnumerator AwaitCardInPlace(CardObject card)
    {
        while (true)
        {
            if(card.IsInPosition)
                yield break;

            yield return null;
        }
    }
    public IEnumerator IsResolving()
    {
        isBusy = true;
        var cardsInPosition = false;

        while (!cardsInPosition)
        {
            foreach(var card in cardList)
            {
                cardsInPosition = card.IsInPosition;
            }
            yield return null;
        }

        isBusy = false;
    }
    void OnUpCard(GameObject cardObject)
    {
        if (currentTargetCard != null)
            return;

        ShowVisualizingCard(cardObject);
    }
    void ShowVisualizingCard(GameObject cardObject)
    {
        var card = cardObject.GetComponent<CardObject>();

        if (!card.IsInPosition)
            return;

        storedOriginalCardObjectData = CardObjectData.Create(card.transform.position, card.transform.localRotation);

        var mainCameraPosition = Camera.main.transform.position;
        mainCameraPosition += (Camera.main.transform.forward * visualizeCardPositionOffset.Position.z); //Adjust Z
        mainCameraPosition += (-Camera.main.transform.up * visualizeCardPositionOffset.Position.y); //Adjust Y

        var data = CardObjectData.Create(mainCameraPosition, visualizeCardPositionOffset.Rotation);

        currentTargetCard = card;
        currentTargetCard.SetPositionAndRotation(data);
        currentTargetCard.RegisterCloseCallback(CloseCardVisualization);
    }
    void CloseCardVisualization()
    {
        currentTargetCard.SetPositionAndRotation(storedOriginalCardObjectData);
        currentTargetCard.RegisterCloseCallback(null);
        currentTargetCard = null;
    }



    void OnDragCardStart(GameObject cardObject)
    {
        if (currentTargetCard != null)
            return;

        var card = cardObject.GetComponent<CardObject>();

        if (!card.IsInPosition)
            return;

        Debug.Log("Dragging started");

        currentTargetCard = card;
        storedOriginalCardObjectData = CardObjectData.Create(card.transform.position, card.transform.localRotation);
        currentTargetCard.SetPositionAndCallback(CalculateheldCardPosition);
    }
    void OnDragCardEnd(GameObject cardObject)
    {
        if (currentTargetCard == null || currentTargetCard.gameObject != cardObject)
            return;

        Debug.Log("Dragging ended");

        currentTargetCard.SetPositionAndCallback(null);
        currentTargetCard.SetPositionAndRotation(storedOriginalCardObjectData);
        currentTargetCard = null;
    }
    CardObjectData CalculateheldCardPosition()
    {
        var mousePosition = inputController.MousePosition;

        mousePosition += draggingCardRotationOffset.Position;

        return CardObjectData.Create(Camera.main.ScreenToWorldPoint(mousePosition), draggingCardRotationOffset.Rotation);;
    }
}

