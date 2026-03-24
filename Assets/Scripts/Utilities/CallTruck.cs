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
            Debug.LogError("No Quests with QuestionSet data to show!");
            return;
        }

        ReverseTruck();
    }
}
