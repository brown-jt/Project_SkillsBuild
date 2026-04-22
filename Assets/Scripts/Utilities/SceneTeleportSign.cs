using UnityEngine;

public class SceneTeleportSign : InteractableItem
{
    [SerializeField] private string sceneName;
    [SerializeField] private string returnSpawnID;
    public override void Interact()
    {
        SpawnManager.Instance.SetNextSpawn(returnSpawnID, sceneName);
        ZoneManager.Instance.TeleportTo(sceneName, returnSpawnID);

        Debug.Log($"Teleporting to {sceneName} with spawn ID {returnSpawnID}");

        if (sceneName == "HubScene")
        {
            FeedbackBannerUI.Instance.ShowBanner("Saved Progress", "Returned to Main Hub");
        }
        else
        {
            FeedbackBannerUI.Instance.ShowBanner("Entered Course", InteractableName);
        }
    }
}
