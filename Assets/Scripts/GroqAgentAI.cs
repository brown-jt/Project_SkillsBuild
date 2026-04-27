using UnityEngine;

public class GroqAgentAI : InteractableItem
{
    [SerializeField] private ZoneId zone;
    [SerializeField] private string course;
    [SerializeField] private string promptName; // Optional name to use in the prompt instead of the InteractableName
    [SerializeField] private GameObject hintIndicator;

    private QuestionSetData questionSet;

    // Follow player variables
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private Animator animator;

    private Vector3 lastPosition;
    private float smoothedSpeed;

    private void Start()
    {
        promptName ??= InteractableName;
        hintIndicator.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        HandleHintIndicator();
        FollowPlayer();
        LookAtPlayer();
        UpdateAnimation();
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

        questionSet = questInstance.questData.questionSet;

        // Picking the current question to generate a hint for in this interaction sequence and display as dialog
        int currentQuestionIndex = QuestionManager.Instance.GetQuestionIndexForZone(zone);
        if (currentQuestionIndex == -1)
        {
            DialogManager.Instance.StartDialog(DialogDataHelper.CreateDialogDataFromText("I cannot help you with a question when you are not currently on one. Try starting a quiz first then talk to me again, I'll be of more use then."), InteractableName);
            return;
        }

        QuestionData question = questionSet.questions.Find(q => q.questionId == currentQuestionIndex);
        string prompt = BuildPromptForQuestion(question);
        QuestionHintAIService.Instance.GetQuestionHint(questInstance.questData.questId, currentQuestionIndex, prompt, OnResponseReceived);
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

    private void HandleHintIndicator()
    {
        bool hasActiveQuestWithQuestions = FindActiveQuestWithQuestions() != null;
        bool currentQuestionValid = QuestionManager.Instance.GetQuestionIndexForZone(zone) != -1;
        hintIndicator.SetActive(hasActiveQuestWithQuestions && currentQuestionValid);
    }

    private void FollowPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Only follow if outside stop distance
        if (distance > stopDistance)
        {
            Vector3 targetPosition = player.position;
            targetPosition.y = transform.position.y; // Prevent moving up/down, only follow on the horizontal plane

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        float rawSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        // Normalize to 0 - 0.5 range for idle and walk (ignoring running as unnecessary for this agent)
        float normalizedSpeed = Mathf.Clamp01(rawSpeed / followSpeed) * 0.5f;

        // Fast but slightly smoothed response
        animator.SetFloat("Speed", normalizedSpeed, 0.05f, Time.deltaTime);

        lastPosition = transform.position;
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;

        directionToPlayer.y = 0; // Keep only horizontal direction as we don't want the agent tilting up/down

        if (directionToPlayer.sqrMagnitude > 0.001f) // Avoids a zero-length vector
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
        }
    }
}
