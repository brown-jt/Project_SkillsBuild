using UnityEngine;
using System.Collections.Generic;

public class QuestGiver : InteractableItem
{
    public List<QuestData> questChain;
    public string npcId;

    public QuestData GetCurrentQuest()
    {
        foreach (var quest in questChain)
        {
            if (QuestManager.Instance.IsQuestCompleted(quest))
                continue;

            // If quest has a prerequisite, ensure it's done
            if (quest.nextQuest != null)
            {
                // Optional validation here
            }

            return quest;
        }

        return null; // All quests in potential chain are done
    }

    public override void Interact()
    {
        QuestData quest = GetCurrentQuest();

        if (quest == null)
        {
            DialogSystem.Instance.StartDialog(null); // fallback idle dialog
            return;
        }

        if (!QuestManager.Instance.HasQuest(quest) &&
            !QuestManager.Instance.IsQuestCompleted(quest))
        {
            // Offer quest
            DialogSystem.Instance.StartDialog(quest.startDialog);
        }
        else if (QuestManager.Instance.HasQuest(quest))
        {
            // Player has quest
            var instance = QuestManager.Instance.GetQuestInstance(quest);

            if (instance.IsObjectivesComplete)
            {
                DialogSystem.Instance.StartDialog(quest.completedDialog);
                QuestManager.Instance.TurnInQuest(quest, npcId);
            }
            else
            {
                DialogSystem.Instance.StartDialog(quest.inProgressDialog);
            }
        }
        else if (QuestManager.Instance.IsQuestCompleted(quest))
        {
            // Move to next quest in chain
            DialogSystem.Instance.StartDialog(quest.completedDialog);
        }
    }
}
