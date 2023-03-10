using System.Collections.Generic;
using UnityEngine;


public class PoolableFactory<T> : Singleton<PoolableFactory<T>> where T : MonoBehaviour, IPoolable
{
    [SerializeField] T Template;

    protected Stack<T> elementPool = new Stack<T>();

    public static T CreateDefault(Transform transform, Vector3 position, Quaternion rotation)
    {
        T obj;
        if (PoolHasElements())
        {
            obj = GetPoolElement();
            obj.transform.SetParent(transform, false);
        }
        else {
            var template = Instance.Template;

            obj = ElementFactory.CreateObject<T>(template, transform);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    public static void AddToPool(T T)
    {
        var cardPool = GetPool();

        T.transform.SetParent(Instance.transform);
        T.gameObject.SetActive(false);
        T.Pool();
        cardPool.Push(T);
    }

    static bool PoolHasElements()
    {
        return GetPool().Count > 0;
    }

    static T GetPoolElement()
    {
        var T = GetPool().Pop();

        T.gameObject.SetActive(true);

        return T;
    }

    static Stack<T> GetPool()
    {
        return Instance.elementPool;
    }
}