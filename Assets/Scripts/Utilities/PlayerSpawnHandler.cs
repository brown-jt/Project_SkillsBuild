using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnHandler : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the potential multiple spawn points in the scene
        PlayerSpawnPoint[] spawnPoints = FindObjectsByType<PlayerSpawnPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        string desiredSpawnID = SpawnManager.Instance != null ? SpawnManager.Instance.NextSpawnID : "";

        PlayerSpawnPoint spawnPoint = null;

        // Looking for the spawn point that matches the desired spawn ID
        foreach (var spawn in spawnPoints)
        {
            if (spawn.spawnID == desiredSpawnID)
            {
                spawnPoint = spawn;
                break;
            }
        }

        // Fallback to the default spawn point if no matching ID is found
        if (spawnPoint == null)
        {
            foreach (var spawn in spawnPoints)
            {
                if (spawn.spawnID == "Default")
                {
                    spawnPoint = spawn;
                    break;
                }
            }
        }

        // Fallback to the first available spawn point if no default is found
        if (spawnPoint == null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[0];
        }

        // Move player to the chosen spawn point wherever it may be
        if (spawnPoint != null)
        {
            // Move the player to the spawn point position and rotation
            transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
        else
        {
            Debug.LogWarning("No PlayerSpawnPoint found in scene:" + scene.name);
        }
    }
}
