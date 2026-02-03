using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    public string NextSpawnID;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialising default spawn ID
        if (string.IsNullOrEmpty(NextSpawnID))
        {
            NextSpawnID = "Default";
        }
    }
}
