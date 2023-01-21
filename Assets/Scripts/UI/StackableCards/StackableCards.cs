using System;
using UnityEngine;

public class StackableCards : MonoBehaviour, IStackableCards
{
    [SerializeField] protected TextMesh DeckCounter;
    [SerializeField] protected Transform CardReferencePosition;
    protected int CurrentDeckSize;
    protected GameObject DeckObject;
    protected Func<int> GetCardCount;
    protected Civilization Civilization;

    public void Setup(Civilization civilization, Func<int> getCardCount)
    {
        GetCardCount = getCardCount;
        Civilization = civilization;

        InstantiateDeck();
    }
    protected void InstantiateDeck()
    {
        var coverTemplate = Resources.Load<GameObject>("Prefabs/CardBackCover" + ((int)Civilization));

        DeckObject = Instantiate(coverTemplate);
        DeckObject.transform.SetParent(transform, true);
        DeckObject.transform.localPosition = Vector3.zero;
        DeckObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
        CurrentDeckSize = 0;
    }
    public int GetNumberOfCards()
    {
        return CurrentDeckSize;
    }
    public void SetDeckSize(int newSize)
    {
        if (newSize == 0)
            return;

        var scale = DeckObject.transform.localScale;

        scale.z += newSize;

        if (scale.z < 0) scale.z = 0;

        DeckObject.transform.localScale = scale;

        CurrentDeckSize += newSize;
    }
    protected void SetCounterText()
    {
        if (DeckCounter == null)
            return;

        DeckCounter.text = CurrentDeckSize + "\n" + ((CurrentDeckSize > 1) ? "Cards" : "Card");
    }
    public Vector3 GetTopPosition()
    {
        return CardReferencePosition.transform.position;
    }
    public Quaternion GetTopRotation()
    {
        return CardReferencePosition.transform.rotation;
    }

}
