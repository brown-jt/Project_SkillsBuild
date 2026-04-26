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
        AudioManager.Instance.PlaySFX("Teleport");

        string musicKey = sceneName switch
        {
            "HubScene" => "Hub_Music",
            "FactoryScene" => "Factory_Music",
            "WarehouseScene" => "Warehouse_Music",
            "MuseumScene" => "Museum_Music",
            _ => "Hub_Music"
        };
        AudioManager.Instance.FadeToNewMusic(musicKey);

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
