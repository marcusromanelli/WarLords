using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerHand : MonoBehaviour
{
    public bool IsBusy => isBusy;
    [SerializeField] float awaitTimeBetweenDraws = 0f;
    [SerializeField] UICardDeck uiCardDeck;
    [SerializeField] Vector3 DeckRotation;
    [SerializeField] BezierCurve bezierCurve;
	[SerializeField] AnimationCurve curveRange;

    List<CardObject> cardList = new List<CardObject>();
    private bool isBusy;
    protected Civilization civilization;

    private InputController inputController;
    private HandleMouseAction onPressedCard;
    private HandleMouseAction onDownCard;
    private HandleMouseAction onUpCard;

    public void PreSetup(InputController inputController, HandleMouseAction onDownCard, HandleMouseAction onPressedCard, HandleMouseAction onUpCard)
    {
        this.inputController = inputController;
        this.onPressedCard = onPressedCard;
        this.onDownCard = onDownCard;
        this.onUpCard = onUpCard;
    }
    public void Setup(Civilization civilization)
    {
        this.civilization = civilization;
    }
    public void AddCard(Card card)
    {
        var cardObj = CardFactory.CreateCard(card, transform, uiCardDeck.GetTopCardPosition());
        cardObj.SetPositionAndRotation(uiCardDeck.GetTopCardPosition(), Quaternion.Euler(DeckRotation));

        cardList.Add(cardObj);

        inputController.RegisterTargetCallback(MouseEventType.LeftMouseButtonUp, cardObj.gameObject, onUpCard);
        inputController.RegisterTargetCallback(MouseEventType.LeftMouseButton, cardObj.gameObject, onPressedCard);
        inputController.RegisterTargetCallback(MouseEventType.LeftMouseButtonDown, cardObj.gameObject, onDownCard);

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

        var spaceBetween = 0.5f/numberOfCards;
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

            card.SetPositionAndRotation(position, rotation);

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
}

