using UnityEngine;

public class ExpertMedalGiver : InteractableItem
{
    [SerializeField] private ItemData[] requiredMedals;
    [SerializeField] private ItemData expertMedalReward;

    [SerializeField] private DialogData noMedalsDialog;
    [SerializeField] private DialogData hasMedalsDialog;
    [SerializeField] private DialogData claimedDialog;

    private Animator animator;
    private string achievementName;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed", 0); // Stay in idle animation
    }

    public override void Interact()
    {
        if (InventoryManager.Instance.HasItem(expertMedalReward))
        {
            DialogManager.Instance.StartDialog(claimedDialog, InteractableName);
            return;
        }

        foreach (var medal in requiredMedals)
        {
            if (!InventoryManager.Instance.HasItem(medal))
            {
                DialogManager.Instance.StartDialog(noMedalsDialog, InteractableName);
                return;
            }
        }

        // If all required medals are present, give the expert medal reward
        DialogManager.Instance.StartDialog(hasMedalsDialog, InteractableName, () => {
            foreach (var medal in requiredMedals)
            {
                InventoryManager.Instance.RemoveItem(medal);
            }
            InventoryManager.Instance.AddItem(expertMedalReward);
            FeedbackNotificationsUI.Instance.AddNotification($"Received: {expertMedalReward.itemName}!");

            switch (expertMedalReward.itemId)
            {
                case "GSWGA_Expert_Medal":
                    achievementName = "Master of Generative AI";
                    break;
                case "GSWAI_Expert_Medal":
                    achievementName = "Master of Artificial Intelligence";
                    break;
                case "GSWD_Expert_Medal":
                    achievementName = "Master of Data";
                    break;
                default:
                    break;
            }

            FeedbackBannerUI.Instance.ShowBanner($"Achievement Unlocked!", achievementName);
            AudioManager.Instance.PlaySFX("Quest_Accept_Complete");
        });
    }
}
