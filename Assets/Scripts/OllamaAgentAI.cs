using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OllamaAgentAI : InteractableItem
{
    [SerializeField] private ZoneId zone;
    [SerializeField] private string course;
    [SerializeField] private string promptName; // Optional name to use in the prompt instead of the InteractableName

    private QuestionSetData questionSet;
    private int questionIndex = 0;

    private void Start()
    {
        promptName ??= InteractableName;
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
            $"You are a {promptName} in a {zone} setting teaching the course called {course}.\n" +
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
