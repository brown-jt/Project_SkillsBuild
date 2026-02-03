using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Tooltip("Spawn ID to use for the next teleport")]
    public string NextSpawnID = "Default";

    [Tooltip("Scene the player is teleporting into")]
    public string TargetSceneName;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetNextSpawn(string spawnID, string sceneName)
    {
        NextSpawnID = spawnID;
        TargetSceneName = sceneName;
    }

    public void ClearNextSpawn()
    {
        NextSpawnID = "Default";
        TargetSceneName = "";
    }
}
