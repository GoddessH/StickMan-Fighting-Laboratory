using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 
    protected static T _instance;
    protected static bool _isDontDestroyOnLoad = false;

    public static T Instance
    {
        get
        {
            if (_instance == null) _instance = FindAnyObjectByType<T>();
            if (_instance == null) _instance = SetupInstance();
            return _instance;
        }
    }

    protected static T SetupInstance()
    {
        GameObject go = new GameObject(typeof(T).Name);
        return go.AddComponent<T>();
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (_isDontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);   
    }

}
