using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : InteractableArea
{
    [SerializeField] private string sceneName;
    [SerializeField] private string returnSpawnID;
    public override void Interact()
    {
        SpawnManager.Instance.NextSpawnID = returnSpawnID;
        SceneManager.LoadScene(sceneName);
    }
}
