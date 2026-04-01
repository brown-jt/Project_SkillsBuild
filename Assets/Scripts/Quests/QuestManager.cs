using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestInstance> activeQuests = new List<QuestInstance>();
    public List<QuestInstance> completedQuests = new List<QuestInstance>();

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

    private void Start()
    {
        LoadQuestsFromDatabase();
    }

    private void OnEnable()
    {
        QuestEvents.onFoundItem += HandleFoundItem;
        QuestEvents.onBuiltItem += HandleBuiltItem;
        QuestEvents.onAreaExplored += HandleAreaExplored;
        QuestEvents.onNPCTalked += HandleNPCTalked;
        QuestEvents.onQuizPassed += HandleQuizPassed;
    }

    private void OnDisable()
    {
        QuestEvents.onFoundItem -= HandleFoundItem;
        QuestEvents.onBuiltItem -= HandleBuiltItem;
        QuestEvents.onAreaExplored -= HandleAreaExplored;
        QuestEvents.onNPCTalked -= HandleNPCTalked;
        QuestEvents.onQuizPassed -= HandleQuizPassed;
    }

    public QuestInstance GetQuestInstance(QuestData data)
    {
        return activeQuests.Find(q => q.questData.questId == data.questId);
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

        // Sync with database
        DatabaseManager.Instance.AcceptQuest(data);
        DatabaseManager.Instance.AddQuestObjectives(questInstance);

        // Fire quest updated event
        onQuestUpdated?.Invoke(questInstance);

        // Pre-generating some AI hints for the questions within the quest
        PreGenerateQuestionHintsForQuest(questInstance);
    }

    private void LoadQuest(QuestData data, List<ObjectiveProgress> objectivesProgress)
    {
        if (data == null) return;

        QuestInstance questInstance = new QuestInstance(data);

        // Replacing default objective progress with loaded progress from database
        questInstance.objectivesProgress = objectivesProgress;

        // Adding the loaded quest instance to active quests without checking for duplicates since this is only called during initial load from database
        activeQuests.Add(questInstance);

        // Fire quest updated event
        onQuestUpdated?.Invoke(questInstance);
    }

    public bool HasQuest(QuestData data)
    {
        return activeQuests.Exists(q => q.questData.questId == data.questId);
    }

    public bool IsQuestCompleted(QuestData data)
    {
        return completedQuests.Exists(q => q.questData.questId == data.questId);
    }
    
    public bool AreQuestObjectivesComplete(QuestData data)
    {
        QuestInstance instance = GetQuestInstance(data);
        return instance != null && instance.IsObjectivesComplete;
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

    void HandleQuizPassed(string quizId)
    {
        Debug.Log($"Handling Quiz passed with ID: {quizId}");
        UpdateObjectives(ObjectiveType.PassQuiz, quizId);
    }

    void UpdateObjectives(ObjectiveType type, string targetId)
    {
        foreach (var quest in activeQuests)
        {
            for(int i = 0; i < quest.objectivesProgress.Count; i++)
            {
                var objective = quest.objectivesProgress[i];

                if (objective.data.objectiveType == type && objective.data.targetId == targetId && !objective.IsComplete)
                {
                    // Update objective progress
                    objective.currentAmount++;
                    if (objective.currentAmount > objective.data.requiredAmount)
                    {
                        objective.currentAmount = objective.data.requiredAmount;
                    }

                    // Save objective progress to database
                    DatabaseManager.Instance.UpdateObjectiveProgress(quest, i, objective.currentAmount, objective.IsComplete);

                    if (objective.IsComplete)
                    {
                        // Fire quest updated event
                        onQuestUpdated?.Invoke(quest);

                        // Show feedback banner
                        FeedbackBannerUI.Instance.ShowBanner("Objective Updated", objective.data.taskText, quest.questData.title);
                    }
                }
                else
                {
                    Debug.Log("Objective does not match event criteria or is already complete.");
                }
            }
        }

        // Ensuring we refresh the quest log UI after updating the objective progress
        QuestJournalUI.Instance.RefreshQuestList();
    }

    void GiveRewards(RewardData rewards)
    {
        // Reward logic here, e.g., add gold, experience, items to player inventory
        InventoryManager.Instance.AddGold(rewards.gold);
        if (rewards.items.Count > 0)
        {
            foreach (var item in rewards.items)
            {
                InventoryManager.Instance.AddItem(item);
            }
        }

        // TODO - Wire experience into the course experience progression bar whenever implemented
    }

    public void TurnInQuest(QuestData questData, string npcId)
    {
        QuestInstance questInstance = GetQuestInstance(questData);

        if (questInstance == null || !questInstance.IsObjectivesComplete) return;

        questInstance.IsTurnedIn = true;
        activeQuests.Remove(questInstance);
        completedQuests.Add(questInstance);

        GiveRewards(questInstance.questData.rewards);

        // Ensuring we refresh the quest log UI after turning in a quest
        QuestJournalUI.Instance.RefreshQuestList();

        // Sync with database
        DatabaseManager.Instance.CompleteQuest(questData);

        // Fire quest updated event
        onQuestUpdated?.Invoke(questInstance);
    }

    private void LoadQuestsFromDatabase()
    {
        // Active quests
        var activeQuestDatas = DatabaseManager.Instance.LoadActiveQuests();
        foreach (var questData in activeQuestDatas)
        {
            List<DatabaseManager.QuestObjectiveRow> dbQuestObjectiveRows = DatabaseManager.Instance.LoadQuestObjectives(questData.questId);
            List<ObjectiveProgress> questObjectives = ConvertDbRowsToObjectiveProgress(questData, dbQuestObjectiveRows);

            if (questData != null) LoadQuest(questData, questObjectives);
        }

        // Completed quests are a little different as we don't want to complete a second time and give rewards again, so we just add them to the completed list without accepting
        var completedQuestDatas = DatabaseManager.Instance.LoadCompletedQuests();
        foreach (var questData in completedQuestDatas)
        {
            if (questData != null)
            {
                QuestInstance instance = new QuestInstance(questData) { IsTurnedIn = true };
                completedQuests.Add(instance);
                onQuestUpdated?.Invoke(instance);
            }
        }

        // Ensure UI is up to date after loading quests
        QuestJournalUI.Instance.RefreshQuestList();
    }

    private List<ObjectiveProgress> ConvertDbRowsToObjectiveProgress(QuestData questData, List<DatabaseManager.QuestObjectiveRow> dbRows)
    {
        var progressList = new List<ObjectiveProgress>();

        // Make sure the order matches QuestData.objectives
        for (int i = 0; i < questData.objectives.Count; i++)
        {
            var objData = questData.objectives[i];
            var progress = new ObjectiveProgress { data = objData };

            // Find the matching row in dbRows
            var dbRow = dbRows.FirstOrDefault(r => r.objective_index == i);
            if (dbRow != null)
            {
                progress.currentAmount = dbRow.current_amount;
            }

            progressList.Add(progress);
        }

        return progressList;
    }

    private void PreGenerateQuestionHintsForQuest(QuestInstance questInstance)
    {
        if (questInstance == null || questInstance.questData.questionSet == null) return;

        QuestionSetData questionSet = questInstance.questData.questionSet;

        for (int i = 0; i < questionSet.questions.Count; i++)
        {
            QuestionData question = questionSet.questions[i];
            int index = i;

            string prompt = BuildPromptForQuestion(question);
            OllamaManager.Instance.GenerateResponse(prompt, (res) =>
            {
                DatabaseManager.Instance.InsertAIResponse(questInstance.questData.questId, index, res);
            });
        }
    }

    private string BuildPromptForQuestion(QuestionData question)
    {
        string answer = string.Join(", ", question.correctAnswerIndices.ConvertAll(i => question.answers[i]));

        return
            $"You are an in-game guide helping a player.\n" +
            "The player is on a quest involving the following question that you need to help:\n" +
            $"Question: {question.question}\n" +
            $"Answer: {answer}\n\n" +
            "Provide **ONE single-sentence hint** that:\n" +
            "- includes the answer **exactly as written**\n" +
            "- explains why it is correct in context\n" +
            "- is phrased differently each time\n" +
            "- does not mention the AI's name, the course, the zone, or the object being interacted with.\n\n" +
            "If the answer cannot be explained independently such as just '80%', use the question for more information and create your response.\n" +
            "Do not label it. Do not use quotes.";
    }
}
