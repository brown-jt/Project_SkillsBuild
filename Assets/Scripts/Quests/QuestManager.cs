using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestInstance> activeQuests = new List<QuestInstance>();
    public HashSet<string> completedQuests = new HashSet<string>();

    public event Action<QuestInstance> onQuestUpdated;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        QuestEvents.onFoundItem += HandleFoundItem;
        QuestEvents.onBuiltItem += HandleBuiltItem;
        QuestEvents.onAreaExplored += HandleAreaExplored;
        QuestEvents.onNPCTalked += HandleNPCTalked;
    }

    private void OnDisable()
    {
        QuestEvents.onFoundItem -= HandleFoundItem;
        QuestEvents.onBuiltItem -= HandleBuiltItem;
        QuestEvents.onAreaExplored -= HandleAreaExplored;
        QuestEvents.onNPCTalked -= HandleNPCTalked;
    }

    public QuestInstance GetQuestInstance(QuestData data)
    {
        return activeQuests.Find(q => q.questData == data);
    }

    public void AcceptQuest(QuestData data)
    {
        // Ensure the quest is not already active or completed
        if (HasQuest(data) || IsQuestCompleted(data)) return;

        // Add new quest instance to active quests
        QuestInstance questInstance = new QuestInstance(data);
        activeQuests.Add(questInstance);

        // Refresh the quest log UI here
        QuestJournalUI.Instance.RefreshQuestList();

        // Fire quest updated event
        onQuestUpdated?.Invoke(questInstance);
    }

    public bool HasQuest(QuestData data)
    {
        return activeQuests.Exists(q => q.questData == data);
    }

    public bool IsQuestCompleted(QuestData data)
    {
        return completedQuests.Contains(data.questId);
    }

    void HandleFoundItem(string itemId)
    {
        Debug.Log($"Handling found item: {itemId}");
        UpdateObjectives(ObjectiveType.Find, itemId);
    }

    void HandleBuiltItem(string itemId)
    {
        Debug.Log($"Handling built item: {itemId}");
        UpdateObjectives(ObjectiveType.Build, itemId);
    }

    void HandleAreaExplored(string areaId)
    {
        Debug.Log($"Handling explored area: {areaId}");
        UpdateObjectives(ObjectiveType.Explore, areaId);
    }

    void HandleNPCTalked(string npcId)
    {
        Debug.Log($"Handling NPC talked to: {npcId}");
        UpdateObjectives(ObjectiveType.TalkTo, npcId);
    }

    void UpdateObjectives(ObjectiveType type, string targetId)
    {
        foreach (var quest in activeQuests)
        {
            Debug.Log($"Checking quest: {quest.questData.name} for objective type: {type} and target ID: {targetId}");
            foreach (var objective in quest.objectivesProgress)
            {
                Debug.Log($"Checking objective type: {objective.data.objectiveType} with given type: {type}");
                Debug.Log($"Checking objective target: {objective.data.targetId} with given target: {targetId}");
                Debug.Log($"Checking complete status of objective: {objective.IsComplete}");
                if (objective.data.objectiveType == type && objective.data.targetId == targetId && !objective.IsComplete)
                {
                    Debug.Log("UPDATING OBJECTIVE PROGRESS");
                    objective.currentAmount++;
                    if (objective.currentAmount > objective.data.requiredAmount)
                    {
                        objective.currentAmount = objective.data.requiredAmount;
                    }
                    // Ensuring we refresh the quest log UI after updating the objective progress
                    QuestJournalUI.Instance.RefreshQuestList();

                    // Fire quest updated event
                    onQuestUpdated?.Invoke(quest);
                }
                else
                {
                    Debug.Log("Objective does not match event criteria or is already complete.");
                }
            }
        }
    }

    void GiveRewards(RewardData rewards)
    {
        // Reward logic here, e.g., add gold, experience, items to player inventory
        Debug.Log($"Granted {rewards.gold} gold and {rewards.experience} XP.");
    }

    public void TurnInQuest(QuestData questData, string npcId)
    {
        QuestInstance questInstance = GetQuestInstance(questData);

        if (questInstance == null || !questInstance.IsObjectivesComplete) return;

        questInstance.IsTurnedIn = true;
        activeQuests.Remove(questInstance);
        completedQuests.Add(questInstance.questData.questId);

        GiveRewards(questInstance.questData.rewards);

        // Ensuring we refresh the quest log UI after turning in a quest
        QuestJournalUI.Instance.RefreshQuestList();

        // Fire quest updated event
        onQuestUpdated?.Invoke(questInstance);
    }
}
