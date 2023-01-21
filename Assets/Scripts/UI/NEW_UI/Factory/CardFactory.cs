using System.Collections.Generic;
using UnityEngine;

public class CardFactory : Singleton<CardFactory>
{
    [SerializeField] CardObject CardTemplate;

    Stack<CardObject> cardPool = new Stack<CardObject>();

    public static CardObject CreateCard(Card card, Transform transform)
    {
        //var cardObj = CreateEmptyCard(transform);

        //cardObj.Setup(card);

        return null;
    }
    public static CardObject CreateEmptyCard(Civilization civilization, Transform transform)
    {
        if (PoolHasElements())
        {
            return GetPoolElement();
        }

        var cardTemplate = Instance.CardTemplate;

        var prefab = ElementFactory.CreatePrefab<CardObject>(cardTemplate, transform);
        prefab.transform.SetParent(transform, false);
        prefab.SetupEmpty(civilization);

        return prefab;
    }
    public static void AddCardToPool(CardObject cardObject)
    {
        var cardPool = GetPool();

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