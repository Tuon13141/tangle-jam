using UnityEngine;

/// <summary>
/// A thread-safe, generic singleton base class for MonoBehaviours.
/// Automatically creates an instance if none exists and persists it between scenes.
/// </summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // The singleton instance.
    private static T instance;

    // Lock object for thread safety.
    private static readonly object lockObject = new object();

    // Flag to indicate if the application is shutting down.
    private static bool isShuttingDown = false;

    /// <summary>
    /// Gets the singleton instance of this MonoBehaviour.
    /// If no instance exists, it will search for one or create a new GameObject with the required component.
    /// </summary>
    public static T Instance
    {
        get
        {
            // Prevent creating a new instance if the application is quitting.
            if (isShuttingDown)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} is already destroyed. Returning null.");
                return null;
            }

            lock (lockObject)
            {
                if (instance == null)
                {
                    // Try to find an existing instance in the scene.
                    instance = FindObjectOfType<T>();

                    // If no instance is found, create a new GameObject.
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"{typeof(T)} (Singleton)";
                        // Make sure the singleton persists between scene loads.
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return instance;
            }
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// This ensures that there is only one instance of the singleton.
    /// </summary>
    protected virtual void Awake()
    {
        // If no instance exists, assign this instance and persist it.
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        // If a duplicate instance exists, destroy this game object.
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the application is quitting.
    /// Prevents the singleton from creating new instances during shutdown.
    /// </summary>
    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    /// <summary>
    /// Called when this object is destroyed.
    /// Also marks the singleton as shutting down.
    /// </summary>
    private void OnDestroy()
    {
        isShuttingDown = true;
    }

    /// <summary>
    /// Checks if the singleton instance exists and is not shutting down.
    /// </summary>
    public static bool Exists()
    {
        return instance != null && !isShuttingDown;
    }
}
