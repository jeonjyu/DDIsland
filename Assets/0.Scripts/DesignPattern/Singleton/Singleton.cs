using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static bool isQuit = false;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (isQuit) return null;

            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.AddComponent<T>();
                }
            }
            
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this as T;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnApplicationQuit() => isQuit = true;
    protected virtual void OnDestroy() => isQuit = true;
}
