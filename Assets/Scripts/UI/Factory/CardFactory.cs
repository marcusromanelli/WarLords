using UnityEngine;

public class CardFactory : PoolableFactory<CardObject> 
{
    public static CardObject CreateCard(Player player, Card card, Transform transform, bool hideCardInfo)
    {
        return CreateCard(player, card, transform, Vector3.zero, Quaternion.identity, hideCardInfo);
    }
    public static CardObject CreateCard(Card card, Transform transform, bool hideCardInfo)
    {
        return CreateCard(null, card, transform, Vector3.zero, Quaternion.identity, hideCardInfo);
    }
    public static CardObject CreateCard(Player player, Card card, Transform transform, Vector3 position, Quaternion rotation, bool hideCardInfo)
    {
        var cardObj = CreateEmptyCard(transform, position, rotation);

        cardObj.Setup(player, card, hideCardInfo);

        return cardObj;
    }
    public static CardObject CreateEmptyCard(Transform transform, Vector3 position, Quaternion rotation)
    {
        var obj = CreateDefault(transform, position, rotation);

        obj.transform.SetParent(transform, false);
        obj.transform.position = position;
        obj.transform.localRotation = rotation;

        return obj;
    }
}