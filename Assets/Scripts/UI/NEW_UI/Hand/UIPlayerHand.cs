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
    [SerializeField, ShowIf("IsInteractable")] UICardDeck uiGraveyardDeck;
    [SerializeField, ShowIf("IsInteractable")] UIManaPool uiManaPool;
    [SerializeField, ShowIf("IsInteractable")] BezierCurve bezierCurve;
	[SerializeField, ShowIf("IsInteractable")] AnimationCurve curveRange;

    [BoxGroup("Presets"), SerializeField, ShowIf("IsInteractable")] CardPositionData visualizeCardPositionOffset;
	[BoxGroup("Presets"), SerializeField, ShowIf("IsInteractable")] CardPositionData draggingCardRotationOffset;
    [BoxGroup("Presets"), SerializeField, ShowIf("IsInteractable")] Vector3 DeckRotation;

    List<CardObject> cardList = new List<CardObject>();
    private bool isBusy;
    protected Civilization civilization;

    private InputController inputController;
    private CardObject currentTargetCard;
    private bool IsDraggingCard;

    public void PreSetup(InputController inputController)
    {
        this.inputController = inputController;

        RegisterDefaultCallbacks();
    }
    public void Setup(Civilization civilization)
    {
        this.civilization = civilization;
    }
    public void AddCard(Card card)
    {
        var cardObj = CardFactory.CreateCard(card, transform, uiCardDeck.GetTopCardPosition());
        cardObj.SetPositionAndRotation(CardPositionData.Create(uiCardDeck.GetTopCardPosition(), Quaternion.Euler(DeckRotation)));

        cardList.Add(cardObj);

        RegisterCardCallback(cardObj.gameObject);

        GameConfiguration.PlaySFX(GameConfiguration.drawCard);

        StartCoroutine(RefreshHandCardsPositions());
    }
    public void AddCards(Card[] cards)
    {
        foreach(Card card in cards)
            AddCard(card);
    }
    void RegisterDefaultCallbacks()
    {
        inputController.RegisterTargetCallback(MouseEventType.Hover, uiCardDeck.gameObject, OnStartHoverMainDeck);
        inputController.RegisterTargetCallback(MouseEventType.EndHover, uiCardDeck.gameObject, OnEndHoverMainDeck);

        inputController.RegisterTargetCallback(MouseEventType.Hover, uiGraveyardDeck.gameObject, OnStartHoverGraveyard);
        inputController.RegisterTargetCallback(MouseEventType.EndHover, uiGraveyardDeck.gameObject, OnEndHoverGraveyard);

        inputController.RegisterTargetCallback(MouseEventType.Hover, uiManaPool.gameObject, OnStartHoverManaPool);
        inputController.RegisterTargetCallback(MouseEventType.EndHover, uiManaPool.gameObject, OnEndHoverManaPool);
    }
    void RegisterCardCallback(GameObject gameObject)
    {
        if (IsInteractable)
        {
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseButtonUp, gameObject, OnUpCard);
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseDragStart, gameObject, OnDragCardStart);
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseDragEnd, gameObject, OnDragCardEnd);
        }
    }
    IEnumerator RefreshHandCardsPositions()
    {
        var numberOfCards = cardList.Count;

        if (numberOfCards <= 0)
            yield break;

        for (int i = 0; i < numberOfCards; i++)
        {
            var card = cardList[i];

            var cardHandIndex = GetCardIndexByObject(card);

            var cardPosition = GetCardHandPosition(cardHandIndex);
                
            card.SetPositionAndRotation(cardPosition);

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

        var forwardCameraPosition = CalculateForwardCameraPosition();        

        var data = CardPositionData.Create(forwardCameraPosition, visualizeCardPositionOffset.Rotation);

        currentTargetCard = card;
        currentTargetCard.SetPositionAndRotation(data);
        currentTargetCard.RegisterCloseCallback(CloseCardVisualization);
    }
    Vector3 CalculateForwardCameraPosition()
    {
        var mainCameraPosition = Camera.main.transform.position;
        mainCameraPosition += (Camera.main.transform.forward * visualizeCardPositionOffset.Position.z); //Adjust Z
        mainCameraPosition += (-Camera.main.transform.up * visualizeCardPositionOffset.Position.y); //Adjust Y

        return mainCameraPosition;
    }
    void CloseCardVisualization()
    {
        ReturnCurrentCardToHand();
    }
    void OnDragCardStart(GameObject cardGameObject)
    {
        if (IsDraggingCard || currentTargetCard != null && cardGameObject != cardGameObject.gameObject)
            return;

        CardObject cardObject;
        if (currentTargetCard == null)
        {
            cardObject = cardGameObject.GetComponent<CardObject>();

            if (!cardObject.IsInPosition)
                return;
            currentTargetCard = cardObject;
        }
        else
            cardObject = currentTargetCard;

        IsDraggingCard = true;
        StartCardDynamicDrag(cardObject);
    }
    void OnDragCardEnd(GameObject cardObject)
    {
        if (!IsDraggingCard || currentTargetCard == null || currentTargetCard.gameObject != cardObject)
            return;

        IsDraggingCard = false;
        StopCardDynamicDrag();
        ReturnCurrentCardToHand();
    }
    void ReturnCurrentCardToHand()
    {
        var cardIndex = GetCardIndexByObject(currentTargetCard);
        currentTargetCard.SetPositionAndRotation(GetCardHandPosition(cardIndex));
        currentTargetCard.RegisterCloseCallback(null);
        currentTargetCard = null;
    }
    CardPositionData CalculateheldCardPosition()
    {
        var mousePosition = inputController.MousePosition;

        mousePosition += draggingCardRotationOffset.Position;

        return CardPositionData.Create(Camera.main.ScreenToWorldPoint(mousePosition), draggingCardRotationOffset.Rotation);;
    }
    void StartCardDynamicDrag(CardObject cardObject)
    {
        currentTargetCard.SetPositionAndCallback(CalculateheldCardPosition);
    }
    void StopCardDynamicDrag()
    {
        currentTargetCard.SetPositionAndCallback(null);
    }
    void OnStartHoverMainDeck(GameObject gameObject)
    {
        if (!IsDraggingCard)
            return;

        GenericHoverPlace(gameObject);
    }
    void OnEndHoverMainDeck(GameObject gameObject)
    {
        if (!IsDraggingCard)
            return;

        StartCardDynamicDrag(currentTargetCard);
    }
    void OnStartHoverGraveyard(GameObject gameObject)
    {
        if (!IsDraggingCard)
            return;

        GenericHoverPlace(gameObject);
    }
    void OnEndHoverGraveyard(GameObject gameObject)
    {
        if (!IsDraggingCard)
            return;

        StartCardDynamicDrag(currentTargetCard);
    }
    void OnStartHoverManaPool(GameObject gameObject)
    {
        if (!IsDraggingCard)
            return;

        GenericHoverPlace(gameObject);
    }
    void OnEndHoverManaPool(GameObject gameObject)
    {
        if (!IsDraggingCard)
            return;

        StartCardDynamicDrag(currentTargetCard);
    }
    void GenericHoverPlace(GameObject gameObject)
    {
        ICardPlaceable cardPlaceable = gameObject.transform.GetComponent<ICardPlaceable>();

        StopCardDynamicDrag();
        currentTargetCard.SetPositionAndRotation(CardPositionData.Create(cardPlaceable.GetTopCardPosition(), cardPlaceable.GetRotationReference()));
    }
    int GetCardIndexByObject(CardObject card)
    {
        return cardList.IndexOf(card);
    }
    CardPositionData GetCardHandPosition(int cardIndex)
    {
        var numberOfCards = cardList.Count;
        var spaceBetween = 0.5f / numberOfCards;
        var positionCount = spaceBetween * cardIndex;
        var verticalBuildUp = 0.01f;
        var normalizedPosition = positionCount * 2;
        

        var position = bezierCurve.GetPointAt(normalizedPosition);
        position.y += verticalBuildUp * cardIndex; //Rise every card by a little to avoid Z-fight

        var rotationValue = curveRange.Evaluate(normalizedPosition);
        rotationValue *= 10;//Curve is stored in X/10 value

        var rotation = Quaternion.Euler(-90, 180, rotationValue);

        return CardPositionData.Create(position, rotation);
    }
}

