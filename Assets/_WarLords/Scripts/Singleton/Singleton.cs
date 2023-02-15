using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if(instance == null) 
                    instance = CreateGameObject();

                DontDestroyOnLoad(instance);
            }

            return instance;
        }
    }
    static T CreateGameObject()
    {
        return (new GameObject(typeof(T).Name, typeof(T))).GetComponent<T>();
    }
}
