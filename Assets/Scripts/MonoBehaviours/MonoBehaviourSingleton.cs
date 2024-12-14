using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Attempted to make more than one instance of {nameof(MonoBehaviourSingleton<T>)} with type: {typeof(T)}");
            Destroy(this);
        }
        else
        {
            Instance = (T)this;
        }
    }
}
