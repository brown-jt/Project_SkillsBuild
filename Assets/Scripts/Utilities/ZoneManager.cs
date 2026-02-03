using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    [SerializeField] private List<string> allZoneScenes; // all additive zones including Hub
    [SerializeField] private string hubSceneName = "HubScene";

    private Scene currentScene;
    private Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Register the currently active scene (HubScene)
        Scene activeScene = SceneManager.GetActiveScene();
        loadedScenes[activeScene.name] = activeScene;
        currentScene = activeScene;

        // Load all other zones additively
        foreach (var sceneName in allZoneScenes)
        {
            if (!loadedScenes.ContainsKey(sceneName))
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }

        StartCoroutine(PostLoadSetup());
    }

    private System.Collections.IEnumerator PostLoadSetup()
    {
        // Wait a frame for additive loads to complete
        yield return null;

        foreach (var sceneName in allZoneScenes)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid()) continue;

            if (!loadedScenes.ContainsKey(sceneName))
                loadedScenes[sceneName] = scene;

            // Deactivate root objects if not current scene
            bool keepActive = sceneName == currentScene.name;
            foreach (var go in scene.GetRootGameObjects())
                go.SetActive(keepActive);
        }
    }

    public void TeleportTo(string targetSceneName, string spawnID)
    {
        if (!loadedScenes.ContainsKey(targetSceneName))
        {
            Debug.LogError($"Scene {targetSceneName} has not been loaded yet!");
            return;
        }

        Scene targetScene = loadedScenes[targetSceneName];

        // Activate target scene
        foreach (var go in targetScene.GetRootGameObjects())
            go.SetActive(true);

        // Move player to spawn point
        MovePlayerToSpawn(targetScene, spawnID);

        // Call OnSceneActivated on all components in the scene
        foreach (var comp in targetScene.GetRootGameObjects()
                                       .SelectMany(go => go.GetComponentsInChildren<IOnSceneActivated>(true)))
        {
            comp.OnSceneActivated();
        }

        // Set as active scene
        SceneManager.SetActiveScene(targetScene);

        // Deactivate previous scene (except player)
        if (currentScene.IsValid() && currentScene.name != targetScene.name)
        {
            foreach (var go in currentScene.GetRootGameObjects())
            {
                if (!go.CompareTag("Player"))
                    go.SetActive(false);
            }
        }

        currentScene = targetScene;
    }

    private void MovePlayerToSpawn(Scene scene, string spawnID)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var spawnPoints = scene.GetRootGameObjects()
            .SelectMany(go => go.GetComponentsInChildren<PlayerSpawnPoint>(true))
            .ToArray();

        PlayerSpawnPoint targetSpawn = spawnPoints.FirstOrDefault(sp => sp.spawnID == spawnID)
            ?? spawnPoints.FirstOrDefault(sp => sp.spawnID == "Default")
            ?? spawnPoints.FirstOrDefault();

        if (targetSpawn != null)
        {
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.SetPositionAndRotation(targetSpawn.transform.position, targetSpawn.transform.rotation);

            if (controller != null) controller.enabled = true;
        }
        else
        {
            Debug.LogWarning($"No spawn point found in scene {scene.name} for ID '{spawnID}'");
        }
    }
}
