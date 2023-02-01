using UnityEngine;

public class ElementFactory : Singleton<ElementFactory>
{
    public static T CreateGameObject<T>() where T : Component, IPoolable
    {
        return (new GameObject(typeof(T).Name, typeof(T))).GetComponent<T>();
    }
    public static GameObject CreateGameObject(GameObject gameObject, Transform transform)
    {
        return Instantiate(gameObject, transform);
    }
    public static T CreateGameObject<T>(GameObject gameObject, Transform transform) where T : Component
    {
        return Instantiate(gameObject, transform).GetComponent<T>();
    }
    public static T CreateGameObject<T>(Component gameObject, Transform transform) where T : Component
    {
        return Instantiate(gameObject, transform).GetComponent<T>();
    }
}
