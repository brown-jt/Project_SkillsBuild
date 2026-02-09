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

        if (sceneName == "HubScene")
        {
            FeedbackBannerUI.Instance.ShowBanner("Saved Progress", "Returned to Main Hub");
        }
        else
        {
            FeedbackBannerUI.Instance.ShowBanner("Entered Course", InteractName);
        }
    }
}
