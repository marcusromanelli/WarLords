using System.Collections.Generic;
using UnityEngine;

/*public class HeroFactory : PoolableFactory<HeroObject> 
{
   /* public static HeroObject Create(CardObject cardObject, Transform transform, Vector3 position, Quaternion rotation)
    {
        var prefab = CreateDefault(transform, position, rotation);

        prefab.Setup(cardObject);

        prefab.name = cardObject.Data.Name;

        return prefab;
    }*/

    /*public static HeroObject CreateCard(Card card, Transform transform, bool hideCardInfo)
    {
        return CreateCard(card, transform, Vector3.zero, Quaternion.identity, hideCardInfo);
    }
    public static HeroObject CreateCard(Card card, Transform transform, Vector3 position, Quaternion rotation, bool hideCardInfo)
    {
        var cardObj = CreateEmptyCard(card.civilization, transform, position, rotation);

        cardObj.Setup(card, hideCardInfo);

        return cardObj;
    }
    public static HeroObject CreateEmptyCard(Civilization civilization, Transform transform)
    {
        return CreateEmptyCard(civilization, transform, Vector3.zero, Quaternion.identity);
    }
    public static HeroObject CreateEmptyCard(Civilization civilization, Transform transform, Vector3 position, Quaternion rotation)
    {
        var obj = CreateDefault(transform, position, rotation);

        obj.transform.SetParent(transform, false);
        obj.transform.position = position;
        obj.transform.localRotation = rotation;
        obj.SetupCover(civilization);

        return obj;
    }*
}*/