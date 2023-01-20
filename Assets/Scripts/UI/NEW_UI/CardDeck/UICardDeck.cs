using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIDeckType
{
    MainDeck, ManaPool, Graveyard
}

public abstract class UICardDeck : MonoBehaviour
{
    [SerializeField] UIDeckType Type;

    public abstract void AddCard();
    public abstract void RemoveCard();

    public abstract Vector3 GetTopCardPosition();
    public abstract Vector3 GetTopCardRotation();
}
