using UnityEngine;
using UnityEngine.EventSystems;

public class PersistentEventSystem : MonoBehaviour
{
    private static EventSystem instance;

    private void Awake()
    {
        if (instance != null && instance != EventSystem.current)
        {
            Destroy(gameObject);
            return;
        }

        instance = GetComponent<EventSystem>();
        DontDestroyOnLoad(gameObject);
    }
}
