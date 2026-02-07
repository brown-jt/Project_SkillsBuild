using UnityEngine;
using System.Collections.Generic;

public class QuestGiver : InteractableItem
{
    public List<QuestData> questChain;
    public string npcId;
    public DialogData idleDialog; // Optional dialog when no quests are available

    [SerializeField] private GameObject hasQuestAvailableSign;
    [SerializeField] private GameObject hasQuestCompleteSign;

    private void Start()
    {
        UpdateQuestSigns();
    }

    private void OnEnable()
    {
        QuestManager.Instance.onQuestUpdated += OnQuestUpdated;
    }

    private void OnDisable()
    {
        QuestManager.Instance.onQuestUpdated -= OnQuestUpdated;
    }

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
            DialogManager.Instance.StartDialog(idleDialog, InteractableName); // TODO: Idle NPC dialog if no quests
            return;
        }

        // Quest not yet accepted
        if (!QuestManager.Instance.HasQuest(quest) &&
            !QuestManager.Instance.IsQuestCompleted(quest))
        {
            // Offer quest
            DialogManager.Instance.StartDialog(quest.startDialog, InteractableName, () =>
            {
                // TODO: Add player choice to accept/decline quest but for now auto-accept
                QuestManager.Instance.AcceptQuest(quest);
                UpdateQuestSigns();
            });
        }
        else if (QuestManager.Instance.HasQuest(quest))
        {
            // Player has quest
            var instance = QuestManager.Instance.GetQuestInstance(quest);

            if (instance.IsObjectivesComplete)
            {
                DialogManager.Instance.StartDialog(quest.completedDialog, InteractableName, () =>
                {
                    QuestManager.Instance.TurnInQuest(quest, npcId);
                    UpdateQuestSigns();
                });
            }
            else
            {
                DialogManager.Instance.StartDialog(quest.inProgressDialog, InteractableName);
            }
        }
        else if (QuestManager.Instance.IsQuestCompleted(quest))
        {
            // Move to next quest in chain
            DialogManager.Instance.StartDialog(quest.completedDialog, InteractableName);
        }
    }

    public void UpdateQuestSigns()
    {
        QuestData quest = GetCurrentQuest();

        if (quest == null)
        {
            hasQuestAvailableSign.SetActive(false);
            hasQuestCompleteSign.SetActive(false);
            return;
        }

        if (!QuestManager.Instance.HasQuest(quest) &&
            !QuestManager.Instance.IsQuestCompleted(quest))
        {
            // Quest available to accept
            hasQuestAvailableSign.SetActive(true);
            hasQuestCompleteSign.SetActive(false);
        }
        else if (QuestManager.Instance.HasQuest(quest))
        {
            var instance = QuestManager.Instance.GetQuestInstance(quest);
            if (instance.IsObjectivesComplete)
            {
                // Quest completed, ready to turn in
                hasQuestAvailableSign.SetActive(false);
                hasQuestCompleteSign.SetActive(true);
            }
            else
            {
                // Quest in progress, no sign needed
                hasQuestAvailableSign.SetActive(false);
                hasQuestCompleteSign.SetActive(false);
            }
        }
        else if (QuestManager.Instance.IsQuestCompleted(quest))
        {
            // Quest already done, no signs
            hasQuestAvailableSign.SetActive(false);
            hasQuestCompleteSign.SetActive(false);
        }
    }

    private void OnQuestUpdated(QuestInstance quest)
    {
        if (quest.questData != null && questChain.Contains(quest.questData))
        {
            UpdateQuestSigns();
        }
    }
}
