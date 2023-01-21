using UnityEngine;

public class ElementFactory : Singleton<ElementFactory>
{
    public static T CreateGameObject<T>() where T : Component, IPoolable
    {
        return (new GameObject(typeof(T).Name, typeof(T))).GetComponent<T>();
    }
    public static T CreatePrefab<T>(Component gameObject, Transform transform) where T : Component
    {
        return Instantiate(gameObject, transform).GetComponent<T>();
    }
    public static T LoadResource<T>(string resourcePath, Transform transform) where T : Object
    {
        T cardObj = Resources.Load<T>(resourcePath);

        return Instantiate(cardObj, transform);
    }
}
