using UnityEngine;

public class PersistentManager : MonoBehaviour
{
    [Header("Prefabs to persist across scenes and not re-create going back to Hub Scene")]
    public GameObject[] persistentPrefabs;

    void Awake()
    {
        foreach (var prefab in persistentPrefabs)
        {
            string prefabName = prefab.name;

            // Check if a persistent object of this type already exists
            if (GameObject.Find(prefabName) == null)
            {
                GameObject instance = Instantiate(prefab);
                instance.name = prefabName; // Ensuring name matches
                DontDestroyOnLoad(instance);
            }
        }
    }
}
