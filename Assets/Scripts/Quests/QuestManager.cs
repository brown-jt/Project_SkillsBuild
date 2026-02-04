using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestInstance> activeQuests = new List<QuestInstance>();
    public HashSet<string> completedQuests = new HashSet<string>();

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
        activeQuests.Add(new QuestInstance(data));
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
        UpdateObjectives(ObjectiveType.Find, itemId);
    }

    void HandleBuiltItem(string itemId)
    {
        UpdateObjectives(ObjectiveType.Build, itemId);
    }

    void HandleAreaExplored(string areaId)
    {
        UpdateObjectives(ObjectiveType.Explore, areaId);
    }

    void HandleNPCTalked(string npcId)
    {
        UpdateObjectives(ObjectiveType.TalkTo, npcId);
    }

    void UpdateObjectives(ObjectiveType type, string targetId)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var objective in quest.objectivesProgress)
            {
                if (objective.data.objectiveType == type && objective.data.targetId == targetId && !objective.IsComplete)
                {
                    objective.currentAmount++;
                    if (objective.currentAmount > objective.data.requiredAmount)
                    {
                        objective.currentAmount = objective.data.requiredAmount;
                    }
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
    }
}
