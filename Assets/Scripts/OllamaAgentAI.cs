using UnityEngine;

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
        prompt = $"You are a {promptName} in a RPG world based on helping teach a player about different learning courses. You are currently in a {zone} zone helping teach the {course} course.";
    }

    public override void Interact()
    {
        // Complete the prompt with the question data from the player's active quests in this zone
        bool questStarted = FinishPromptWithQuestionData();

        // Call OllamaManger or provide feedback to player if no relevant quests with question data are active
        if (questStarted) OllamaManager.Instance.GenerateResponse(prompt, OnResponseReceived);
        else DialogManager.Instance.StartDialog(DialogDataHelper.CreateDialogDataFromText("You are currently not on any active quests in this zone that I can help with. Try accepting a quest first!"), InteractableName);
    }

    private bool FinishPromptWithQuestionData()
    {
        questionSet = null;
        prompt = $"You are a {promptName} in a RPG world based on helping teach a player about different learning courses. You are currently in a {zone} zone helping teach the {course} course.";

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

        prompt += $" The player is currently stuck on the following questions: ";

        for (int i = 0; i < questionSet.questions.Count; i++)
        {
            QuestionData q = questionSet.questions[i];
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
                prompt += $" Q{i+1}: {q.question} - Answers are {answers}.";
            }
            else
            {
                // Single answer
                string answer = q.answers[q.correctAnswerIndices[0]];
                prompt += $" Q{i+1}: {q.question} - Answer is {answer}.";
            }
        }

        prompt += $" Can you give a descriptive single sentence explaining one of these answers to the player to help them understand the material better? Ensure you give it in the context of a {promptName} that a player inte";
        return true;
    }

    private void OnResponseReceived(string response)
    {
        DialogData aiDialog = DialogDataHelper.CreateDialogDataFromText(response);
        DialogManager.Instance.StartDialog(aiDialog, InteractableName);
    }
}
