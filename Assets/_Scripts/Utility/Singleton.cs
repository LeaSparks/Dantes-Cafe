using UnityEngine;
public abstract class SingletonBase : MonoBehaviour
{
    public bool ShouldDieOnReload = true;

}
public class Singleton<T> : SingletonBase where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            Debug.Log($"[Singleton] Creating Instance of {typeof(T).Name}");

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Debug.Log($"[Singleton] Destroying Instance of {typeof(T).Name}");
            Instance = null;
        }
    }
}
