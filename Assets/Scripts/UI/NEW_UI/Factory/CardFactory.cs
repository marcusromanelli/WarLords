using System.Collections.Generic;
using UnityEngine;

public class CardFactory : Singleton<CardFactory>
{
    [SerializeField] CardObject CardTemplate;

    Stack<CardObject> cardPool = new Stack<CardObject>();

    public static CardObject CreateCard(Card card, Transform transform)
    {
        return CreateCard(card, transform, Vector3.zero);
    }
    public static CardObject CreateCard(Card card, Transform transform, Vector3 position)
    {
        var cardObj = CreateEmptyCard(card.civilization, transform, position);

        cardObj.Setup(card);

        return cardObj;
    }
    public static CardObject CreateEmptyCard(Civilization civilization, Transform transform)
    {
        return CreateEmptyCard(civilization, transform, Vector3.zero);
    }
    public static CardObject CreateEmptyCard(Civilization civilization, Transform transform, Vector3 position)
    {
        CardObject obj;
        if (PoolHasElements())
        {
            obj = GetPoolElement();
        }
        else {
            var cardTemplate = Instance.CardTemplate;

            obj = ElementFactory.CreatePrefab<CardObject>(cardTemplate, transform);
        }

        obj.transform.SetParent(transform, false);
        obj.transform.position = position;
        obj.SetupCover(civilization);

        return obj;
    }
    public static void AddCardToPool(CardObject cardObject)
    {
        var cardPool = GetPool();

        cardObject.transform.SetParent(Instance.transform);
        cardObject.gameObject.SetActive(false);
        cardObject.Pool();
        cardPool.Push(cardObject);
    }

    static bool PoolHasElements()
    {
        return GetPool().Count > 0;
    }

    static CardObject GetPoolElement()
    {
        var cardObject = GetPool().Pop();

        cardObject.gameObject.SetActive(true);

        return cardObject;
    }

    static Stack<CardObject> GetPool()
    {
        return Instance.cardPool;
    }
}