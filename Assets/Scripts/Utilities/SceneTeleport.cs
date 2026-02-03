using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : InteractableArea
{
    [SerializeField] private string sceneName;
    [SerializeField] private string returnSpawnID;
    public override void Interact()
    {
        SpawnManager.Instance.SetNextSpawn(returnSpawnID, sceneName);
        ZoneManager.Instance.TeleportTo(sceneName, returnSpawnID);
    }
}
