using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerHand : MonoBehaviour
{
    public CardObject CurrentHoldingCard => currentTargetCard;

    [SerializeField] bool IsInteractable = true;
    [SerializeField] float awaitTimeBetweenDraws = 0.05f;
    [SerializeField, ShowIf("IsInteractable")] UICardDeck uiCardDeck;
    [SerializeField, ShowIf("IsInteractable")] UICardDeck uiGraveyardDeck;
    [SerializeField, ShowIf("IsInteractable")] UIManaPool uiManaPool;
    [SerializeField, ShowIf("IsInteractable")] BezierCurve bezierCurve;
	[SerializeField] AnimationCurve curveRange;

	[BoxGroup("Presets"), SerializeField, ShowIf("IsInteractable")] CardPositionData draggingCardRotationOffset;
    [BoxGroup("Presets"), SerializeField] Vector3 HandRotation;

    private Player player;
    private Battlefield battlefield;
    private List<CardObject> cardList = new List<CardObject>();
    private InputController inputController;
    private bool targetingCard;
    private CardObject _currentTargetCard;
    private CardObject currentTargetCard
    {
        get { return _currentTargetCard; }
        set {
            if (value == _currentTargetCard)
                return;

            _currentTargetCard = value;
        }
    }
    private bool IsHoldingCard => currentTargetCard != null;
    private bool IsCardAwaitingRelease;
    private bool IsDraggingCard;
    private bool IsRearragingCards;
    private ICardPlaceable hoverArea;
    HandleOnCardReleased onCardReleasedOnGraveyard;
    HandleOnCardReleased onCardReleasedOnManaPool;
    HandleOnCardReleased onCardReleasedOnSpawnArea;
    HandleCanSummonToken canSummonToken;

    public void PreSetup(Player player, Battlefield battlefield, InputController inputController, HandleOnCardReleased onCardReleasedOnGraveyard,
        HandleOnCardReleased onCardReleasedOnManaPool, HandleOnCardReleased onCardReleasedOnSpawnArea,
        HandleCanSummonToken canSummonToken
        )
    {
        this.player = player;
        this.battlefield = battlefield;
        this.inputController = inputController;
        this.onCardReleasedOnGraveyard = onCardReleasedOnGraveyard;
        this.onCardReleasedOnManaPool = onCardReleasedOnManaPool;
        this.onCardReleasedOnSpawnArea = onCardReleasedOnSpawnArea;
        this.canSummonToken = canSummonToken;

        RegisterDefaultCallbacks();
    }
    public void Remove(CardObject cardObject, bool destroyObject = true)
    {
        RemoveCard(cardObject);

        if(destroyObject)
            SendCardToPool(cardObject);
    }
    public void TurnCardIntoMana(CardObject cardObject, Action onFinishAnimation)
    {
        RemoveCard(cardObject);
        cardObject.BecameMana(() => { SendCardToPool(cardObject); onFinishAnimation(); });
    }
    public List<CardObject> GetCards()
    {
        return cardList;
    }
    void RemoveCard(CardObject cardObject)
    {
        CancelHandToCardInteraction();

        UnregisterCardCallback(cardObject.gameObject);

        var cardIndex = GetCardIndexByObject(cardObject);

        cardList.RemoveAt(cardIndex);

        RefreshCardPositions();
    }
    public void SendCardToPool(CardObject cardObject)
    {
        CardFactory.AddToPool(cardObject);
    }
    public void AddCard(Card card)
    {
        var cardObj = CardFactory.CreateCard(inputController, player, card, transform, uiCardDeck.GetTopCardPosition(), uiCardDeck.GetRotationReference(), IsInteractable);

        cardList.Add(cardObj);

        RegisterCardCallback(cardObj.gameObject);

        GameRules.PlaySFX(GameRules.drawCard);

        RefreshCardPositions();
    }
    public void AddCards(Card[] cards)
    {
        foreach(Card card in cards)
            AddCard(card);
    }
    public void CancelHandToCardInteraction()
    {
        targetingCard = false;
        currentTargetCard = null;
        IsCardAwaitingRelease = false;
        IsDraggingCard = false;
    }
    public void HoldCard(CardObject cardObject)
    {
        currentTargetCard = cardObject;
    }
    public IEnumerator IsResolving()
    {
        while (IsRearragingCards)
        {
            yield return null;
        }
    }
    CardObject GetCardObjectByData(Card card)
    {
        return cardList.Find(cardObj => cardObj.Data == card);
    }
    void RegisterDefaultCallbacks()
    {
        if (!IsInteractable)
            return;

        inputController.RegisterTargetCallback(MouseEventType.Hover, uiGraveyardDeck.gameObject, OnStartHoverGraveyard);
        inputController.RegisterTargetCallback(MouseEventType.EndHover, uiGraveyardDeck.gameObject, OnEndHoverGraveyard);
        inputController.RegisterTargetCallback(MouseEventType.LeftMouseDragEnd, uiGraveyardDeck.gameObject, OnReleaseCardOnGraveyard);

        inputController.RegisterTargetCallback(MouseEventType.Hover, uiManaPool.gameObject, OnStartHoverManaPool);
        inputController.RegisterTargetCallback(MouseEventType.EndHover, uiManaPool.gameObject, OnEndHoverManaPool);
        inputController.RegisterTargetCallback(MouseEventType.LeftMouseDragEnd, uiManaPool.gameObject, OnReleaseCardOnManaPool);


        foreach(var field in battlefield.GetFields())
        {
            inputController.RegisterTargetCallback(MouseEventType.Hover, field.gameObject, OnStartHoverSpawnArea);
            inputController.RegisterTargetCallback(MouseEventType.EndHover, field.gameObject, OnEndHoverSpawnArea);
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseButtonUp, field.gameObject, OnReleaseHoverSpawnArea);
        }
    }
    void RegisterCardCallback(GameObject gameObject)
    {
        if (IsInteractable)
        {
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseButtonDown, gameObject, OnDownCard);
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseButtonUp, gameObject, OnUpCard);
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseDragStart, gameObject, OnDragCardStart);
            inputController.RegisterTargetCallback(MouseEventType.LeftMouseDragEnd, gameObject, OnDragCardEnd);
        }
    }
    void UnregisterCardCallback(GameObject gameObject)
    {
        if (IsInteractable)
        {
            inputController.UnregisterTargetCallback(MouseEventType.LeftMouseButtonDown, gameObject, OnUpCard);
            inputController.UnregisterTargetCallback(MouseEventType.LeftMouseButtonUp, gameObject, OnUpCard);
            inputController.UnregisterTargetCallback(MouseEventType.LeftMouseDragStart, gameObject, OnDragCardStart);
            inputController.UnregisterTargetCallback(MouseEventType.LeftMouseDragEnd, gameObject, OnDragCardEnd);
        }
    }
    [Button("Force Card Positions Refresh")]
    void RefreshCardPositions()
    {
        StopAllCoroutines();
        StartCoroutine(DoRefreshHandCardsPositions());
    }

    IEnumerator DoRefreshHandCardsPositions()
    {
        IsRearragingCards = true;

        var numberOfCards = cardList.Count;

        if (numberOfCards <= 0)
            yield break;

        for (int i = 0; i < numberOfCards; i++)
        {
            var card = cardList[i];

            var cardHandIndex = GetCardIndexByObject(card);

            var cardPosition = GetCardHandPosition(cardHandIndex);

            card.SetPosition(cardPosition);

            yield return new WaitForSeconds(awaitTimeBetweenDraws);
        }

        IsRearragingCards = false;
    }
    void OnDownCard(GameObject gameObject)
    {
        if (currentTargetCard != null && targetingCard)
            return;

        targetingCard = true;
    }
    void OnUpCard(GameObject cardObject)
    {
        if (currentTargetCard != null)
            return;

        targetingCard = false;
    }
    void OnDragCardStart(GameObject cardGameObject)
    {
        if (IsDraggingCard || currentTargetCard != null && cardGameObject != cardGameObject.gameObject)
            return;

        CardObject cardObject;
        if (currentTargetCard == null)
        {
            cardObject = cardGameObject.GetComponent<CardObject>();

            if (!cardObject.IsPositioned)
                return;

            currentTargetCard = cardObject;
        }
        else
            cardObject = currentTargetCard;

        IsDraggingCard = true;
        StartCardDynamicDrag(cardObject);
    }
    void CancelDrag()
    {
        IsDraggingCard = false;
    }

    void OnDragCardEnd(GameObject cardObject)
    {
        if (!IsDraggingCard || currentTargetCard == null || currentTargetCard.gameObject != cardObject)
            return;

        CancelDrag();
        StopCardDynamicDrag();
        ReturnCurrentCardToHand();
    }
    void ReturnCurrentCardToHand()
    {
        var cardIndex = GetCardIndexByObject(currentTargetCard);
        currentTargetCard.SetVisualizing(false);
        currentTargetCard.SetPosition(GetCardHandPosition(cardIndex));
        CancelHandToCardInteraction();
    }
    CardPositionData CalculateheldCardPosition()
    {
        var mousePosition = inputController.MousePosition;

        mousePosition += draggingCardRotationOffset.Position;

        return CardPositionData.Create(Camera.main.ScreenToWorldPoint(mousePosition), draggingCardRotationOffset.Rotation);;
    }
    void StartCardDynamicDrag(CardObject cardObject)
    {
        currentTargetCard.SetPosition(CalculateheldCardPosition);
    }
    void StopCardDynamicDrag()
    {
        currentTargetCard.SetPosition(null);
    }


    #region GRAVEYARD_LOGIC
    void OnReleaseCardOnGraveyard(GameObject releasedArea)
    {
        if (IsRearragingCards || !IsCardAwaitingRelease)
            return;

        onCardReleasedOnGraveyard?.Invoke(currentTargetCard, releasedArea);
    }
    void OnStartHoverGraveyard(GameObject gameObject)
    {
        if (!IsHoldingCard || !IsDraggingCard)
            return;

        GenericHoverPlace(gameObject);
    }
    void OnEndHoverGraveyard(GameObject gameObject)
    {
        if (!IsHoldingCard || !IsDraggingCard)
            return;

        IsCardAwaitingRelease = false;
        StartCardDynamicDrag(currentTargetCard);
    }
    #endregion GRAVEYARD_LOGIC

    #region MANA_POOL_LOGIC

    void OnReleaseCardOnManaPool(GameObject releasedArea)
    {
        if (IsRearragingCards || !IsCardAwaitingRelease)
            return;

        onCardReleasedOnManaPool?.Invoke(currentTargetCard, releasedArea);
    }
    void OnStartHoverManaPool(GameObject gameObject)
    {
        if (!IsHoldingCard || !IsDraggingCard)
            return;

        GenericHoverPlace(gameObject);
    }
    void OnEndHoverManaPool(GameObject gameObject)
    {
        if (!IsHoldingCard || !IsDraggingCard)
            return;

        IsCardAwaitingRelease = false;
        StartCardDynamicDrag(currentTargetCard);
    }

    #endregion MANA_POOL_LOGIC

    #region SPAWN_AREA_LOGIC
    void OnReleaseHoverSpawnArea(GameObject releasedArea)
    {
        if (IsRearragingCards || !IsCardAwaitingRelease)
            return;

        onCardReleasedOnSpawnArea?.Invoke(currentTargetCard, releasedArea);
    }
    void OnStartHoverSpawnArea(GameObject gameObject)
    {
        if (!IsHoldingCard || !IsDraggingCard)
            return;

        HandleStartHoverSpawnArea(gameObject);
    }
    void OnEndHoverSpawnArea(GameObject gameObject)
    {
        var spawnArea = gameObject.GetComponent<SpawnArea>();

        if ((spawnArea != null && hoverArea != spawnArea) || !IsHoldingCard || !IsDraggingCard)
            return;

        hoverArea = null;
        IsCardAwaitingRelease = false;
        StartCardDynamicDrag(currentTargetCard);
    }
    void HandleStartHoverSpawnArea(GameObject spawnAreaObject)
    {
        var spawnArea = spawnAreaObject.GetComponent<SpawnArea>();

        if (spawnArea == null)
            return;

        var isSkillOnly = spawnArea.Token != null;

        if (hoverArea == spawnArea || !canSummonToken(currentTargetCard, spawnArea, isSkillOnly))
            return;

        GenericHoverPlace(spawnArea);
    }

    #endregion SPAWN_AREA_LOGIC


    void GenericHoverPlace(GameObject gameObject)
    {
        ICardPlaceable cardPlaceable = gameObject.GetComponent<ICardPlaceable>();

        GenericHoverPlace(cardPlaceable);
    }
    void GenericHoverPlace(ICardPlaceable cardPlaceable)
    {
        hoverArea = cardPlaceable;
        IsCardAwaitingRelease = true;
        StopCardDynamicDrag();
        currentTargetCard.SetPosition(CardPositionData.Create(cardPlaceable.GetTopCardPosition(), cardPlaceable.GetRotationReference()));
    }
    int GetCardIndexByObject(CardObject card)
    {
        return cardList.IndexOf(card);
    }
    CardPositionData GetCardHandPosition(int cardIndex)
    {
        var verticalBuildUp = 0.02f;

        var numberOfCards = cardList.Count; //5
        var sectionSize = (1f / numberOfCards); //0.2
        var sectionSizeHalf = sectionSize / 2;
        var currentCardSection = sectionSize * (cardIndex + 1);// Card 0 = 0.2, Card 1 = 0.4
        var currentCardSectionMiddle = currentCardSection - sectionSizeHalf;        

        var position = bezierCurve.GetPointAt(currentCardSectionMiddle);
        position.y += verticalBuildUp * (cardIndex + 1); //Rise every card by a little to avoid Z-fight

        var rotationValue = curveRange.Evaluate(currentCardSectionMiddle);
        rotationValue *= 10;//Curve is stored in X/10 value

        var rotation = Quaternion.Euler(HandRotation.x, HandRotation.y, rotationValue);

        return CardPositionData.Create(position, rotation);
    }
}

