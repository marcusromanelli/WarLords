using UnityEngine;

public class ElementFactory : Singleton<ElementFactory>
{
    public static T CreateGameObject<T>() where T : Component, IPoolable
    {
        return (new GameObject(typeof(T).Name, typeof(T))).GetComponent<T>();
    }
    public static Object CreateObject(Object gameObject, Transform transform)
    {
        return Instantiate(gameObject, transform);
    }
    public static T CreateObject<T>(T gameObject) where T : Object
    {
        return Instantiate(gameObject);
    }
    public static T CreateObject<T>(GameObject gameObject) where T : Component
    {
        return Instantiate(gameObject).GetComponent<T>();
    }
    public static T CreateObject<T>(GameObject gameObject, Transform transform) where T : Component
    {
        return Instantiate(gameObject, transform).GetComponent<T>();
    }
    public static T CreateObject<T>(Component gameObject, Transform transform) where T : Component
    {
        return Instantiate(gameObject, transform).GetComponent<T>();
    }
}
