using UnityEngine;

public class GroqAgentAI : InteractableItem
{
    [SerializeField] private ZoneId zone;
    [SerializeField] private string course;
    [SerializeField] private string promptName; // Optional name to use in the prompt instead of the InteractableName
    [SerializeField] private GameObject hintIndicator;

    private QuestionSetData questionSet;
    private int questionIndex = 0;

    private void Start()
    {
        promptName ??= InteractableName;
        hintIndicator.SetActive(false);
    }

    private void Update()
    {
        {
            // Show hint indicator if player is on an active quest in this zone with questions
            bool hasActiveQuestWithQuestions = FindActiveQuestWithQuestions() != null;
            hintIndicator.SetActive(hasActiveQuestWithQuestions);
        }
    }

    public override void Interact()
    {
        // Firstly find the active quest in this zone with questions attached
        QuestInstance questInstance = FindActiveQuestWithQuestions();
        if (questInstance == null)
        {
            // If no quest return some empty placeholder dialog non AI-generated for now
            DialogManager.Instance.StartDialog(DialogDataHelper.CreateDialogDataFromText("You are currently not on any active quests in this zone that I can help with. Try accepting a quest first!"), InteractableName);
            return;
        }

        if (questionSet != null && questionSet != questInstance.questData.questionSet)
        {
            // New question set being assigned so we will need to reset questionIndex
            questionIndex = -1;
        }

        questionSet = questInstance.questData.questionSet;

        // Randomly setting the starting index for the questions
        if (questionIndex == -1) questionIndex = Random.Range(0, questionSet.questions.Count);

        // Picking one question to generate a hint for in this interaction sequence and display as dialog
        QuestionData question = questionSet.questions[questionIndex];
        string prompt = BuildPromptForQuestion(question);
        QuestionHintAIService.Instance.GetQuestionHint(questInstance.questData.questId, questionIndex, prompt, OnResponseReceived);

        // Moving index up by one
        questionIndex = (questionIndex + 1) % questionSet.questions.Count;
    }

    private QuestInstance FindActiveQuestWithQuestions()
    {
        foreach (QuestInstance quest in QuestManager.Instance.activeQuests)
        {
            if (quest.questData.zoneId == zone && quest.questData.questionSet != null)
            {
                return quest;
            }
        }
        return null;
    }

    private string BuildPromptForQuestion(QuestionData question)
    {
        string answer = string.Join(", ", question.correctAnswerIndices.ConvertAll(i => question.answers[i]));

        return
            $"You are a {promptName} in a {zone} setting teaching the course {course}.\n\n" +
            $"Question:\n{question.question}\n\n" +
            $"Correct Answer (must be included exactly):\n{answer}\n\n" +
            "Give a one-sentence hint using the answer.";
    }

    private void OnResponseReceived(string response)
    {
        string cleanedResponse = CleanOllamaResponse(response);

        DialogData aiDialog = DialogDataHelper.CreateDialogDataFromText(cleanedResponse);

        DialogManager.Instance.StartDialog(aiDialog, InteractableName);
    }

    private string CleanOllamaResponse(string response)
    {
        if (string.IsNullOrEmpty(response)) return response;

        // Removing common Ollama finish markers
        response = response.Replace("<|fim_suffix|>", "");
        response = response.Replace("<|fim_middle|>", "");
        response = response.Replace("<|fim_prefix|>", ""); // sometimes appears

        return response.Trim();
    }
}
