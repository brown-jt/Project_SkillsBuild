using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OllamaAgentAI : InteractableItem
{
    [SerializeField] private ZoneId zone;
    [SerializeField] private string course;
    [SerializeField] private string promptName; // Optional name to use in the prompt instead of the InteractableName

    private QuestionSetData questionSet;
    private string prompt;

    private void Start()
    {
        promptName ??= InteractableName;
        ResetBasePrompt();
    }

    public override void Interact()
    {
        // Complete the prompt with the question data from the player's active quests in this zone
        bool questStarted = FinishPromptWithQuestionData();

        // Call OllamaManger or provide feedback to player if no relevant quests with question data are active
        Debug.Log(prompt);
        if (questStarted) OllamaManager.Instance.GenerateResponse(prompt, OnResponseReceived);
        else DialogManager.Instance.StartDialog(DialogDataHelper.CreateDialogDataFromText("You are currently not on any active quests in this zone that I can help with. Try accepting a quest first!"), InteractableName);
    }

    private bool FinishPromptWithQuestionData()
    {
        questionSet = null;
        ResetBasePrompt();

        foreach (QuestInstance questInstance in QuestManager.Instance.activeQuests)
        {
            if (questInstance.questData.zoneId == zone && questInstance.questData.questionSet != null)
            {
                questionSet = questInstance.questData.questionSet;
                break;
            }
        }

        if (questionSet == null || questionSet.questions.Count == 0)
        {
            Debug.Log("The player currently has no active quests in this zone with question data, so the Ollama Agent will not provide any information.");
            return false;
        }

        prompt += $" The player is on quest involving the following questions and answers:";

        // Before passing questions to LLM I will shuffle to further increase the chance of varying hints
        List<QuestionData> questionsShuffled = questionSet.questions.OrderBy(q => UnityEngine.Random.value).ToList();

        foreach (QuestionData q in questionsShuffled)
        {
            if (q.correctAnswerIndices.Count > 1)
            {
                // Multiple answers
                string answers = "";
                for (int j = 0; j < q.correctAnswerIndices.Count; j++)
                {
                    string answer = q.answers[q.correctAnswerIndices[j]];
                    answers += answer;
                    if (j < q.correctAnswerIndices.Count - 1)
                        answers += ", ";
                }
                prompt += $" Question: {q.question} - Answers: {answers}.";
            }
            else
            {
                // Single answer
                string answer = q.answers[q.correctAnswerIndices[0]];
                prompt += $" Question: {q.question} - Answer: {answer}.";
            }
        }

        prompt += " Randomly select one of the questions listed above and provide a **single-sentence hint** that:" +
          " 1) includes the correct answer **exactly as written**," +
          " 2) explains why it is correct in context," +
          " 3) is phrased differently each time," +
          " 4) is concise, friendly, and informative," +
          " 5) does not mention the AI's name, the course, the zone, or the object being interacted with." +
          " Only provide the hint sentence, nothing else." +
          " If the answer cannot be explained independently such as just '80%', use the question for more information and create your response." +
          " Do not use speech marks or answer a question directly for the player. Do not use the word Hint: or Answer: at the start of your sentence, it should be **only the single-sentence hint**.";

        return true;
    }

    private void ResetBasePrompt()
    {
        prompt = $"You are an in-game source of guidance for the {course} course in the {zone} zone of an RPG." +
            " Your task is to help the player understand the answers to their questions by giving clear, concise and friendly hints.";
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
