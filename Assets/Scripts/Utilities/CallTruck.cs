using UnityEngine;

public class CallTruck : InteractableItem
{
    [Header("References")]
    [SerializeField] private TruckController truckController;
    [SerializeField] private WarehouseQuizManager quizManager;

    private void ReverseTruck()
    {
        truckController.StartReversing();
    }

    override public void Interact()
    {
        if (!IsInteractable) return;

        if (quizManager.IsQuizActive)
        {
            FeedbackNotificationsUI.Instance.AddNotification("Quiz is already active! Finish the current quiz before calling the truck again.", 4f);
            return;
        }

        Debug.Log("Checking active quests " + QuestManager.Instance.activeQuests.Count);

        foreach (QuestInstance questInstance in QuestManager.Instance.activeQuests)
        {
            if (questInstance.questData.questionSet != null && questInstance.questData.zoneId == ZoneId.Warehouse)
            {
                Debug.Log($"Found question set for quest: {questInstance.questData.title}");
                quizManager.questionSet = questInstance.questData.questionSet;
                break;
            }
        }

        if (quizManager.questionSet == null)
        {
            FeedbackNotificationsUI.Instance.AddNotification("No active quests with questions found! Start a quest to be able to call the truck.", 4f);
            return;
        }

        quizManager.SetQuizActive();
        ReverseTruck();
        FeedbackNotificationsUI.Instance.AddNotification("Delivery truck has been called! Get ready for the quiz!", 4f);
        AudioManager.Instance.PlaySFX("Click");
    }
}
