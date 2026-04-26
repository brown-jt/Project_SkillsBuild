using UnityEngine;

public class SceneTeleport : InteractableArea
{
    [SerializeField] private string sceneName;
    [SerializeField] private string returnSpawnID;
    public override void Interact()
    {
        SpawnManager.Instance.SetNextSpawn(returnSpawnID, sceneName);
        ZoneManager.Instance.TeleportTo(sceneName, returnSpawnID);
        AudioManager.Instance.PlaySFX("Teleport");
        AudioManager.Instance.FadeToNewMusic("Hub_Music");

        if (sceneName == "HubScene")
        {
            FeedbackBannerUI.Instance.ShowBanner("Saved Progress", "Returned to Main Hub");
        }
        else
        {
            FeedbackBannerUI.Instance.ShowBanner("Entered Course", InteractName);
        }

        IsInteractable = false; // Prevent multiple interactions until player leaves and re-enters the area
    }
}
