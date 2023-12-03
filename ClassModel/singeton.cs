using Unity.Netcode;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    protected virtual void OnDestroy()
    {
        if (Instance != null)
        {
            Instance = null;
        }
    }
}
public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{

    protected static T Singleton = null;
    public static T singleton
    {
        get
        {
            if (!Singleton)
                Singleton = Resources.FindObjectsOfTypeAll<T>()[0];
            return Singleton;
        }
    }
}

public class SingletonPersistent<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class SingletonNetwork<T> : NetworkBehaviour where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}

public class SingletonNetworkPersistent<T> : NetworkBehaviour where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
