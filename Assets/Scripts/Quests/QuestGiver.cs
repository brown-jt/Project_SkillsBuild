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
            DialogManager.Instance.StartDialog(null); // TODO: Idle NPC dialog if no quests
            return;
        }

        // Quest not yet accepted
        if (!QuestManager.Instance.HasQuest(quest) &&
            !QuestManager.Instance.IsQuestCompleted(quest))
        {
            // Offer quest
            DialogManager.Instance.StartDialog(quest.startDialog, () =>
            {
                // TODO: Add player choice to accept/decline quest but for now auto-accept
                QuestManager.Instance.AcceptQuest(quest);
            });
        }
        else if (QuestManager.Instance.HasQuest(quest))
        {
            // Player has quest
            var instance = QuestManager.Instance.GetQuestInstance(quest);

            if (instance.IsObjectivesComplete)
            {
                DialogManager.Instance.StartDialog(quest.completedDialog);
                QuestManager.Instance.TurnInQuest(quest, npcId);
            }
            else
            {
                DialogManager.Instance.StartDialog(quest.inProgressDialog);
            }
        }
        else if (QuestManager.Instance.IsQuestCompleted(quest))
        {
            // Move to next quest in chain
            DialogManager.Instance.StartDialog(quest.completedDialog);
        }
    }
}
