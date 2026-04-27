using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    public QuestData relevantQuest;
    public Painting painting;
    public List<SlidingPuzzleTerminal> puzzleTerminals;
    [SerializeField] private ConfirmAnswerButton confirmButton;
    public GameObject[] spotLights;

    private QuestionSetData questionSet;
    private MuseumQuizManager museumQuizManager;

    private int currentQuestionIndex;
    [SerializeField] private QuestQuizTrigger questQuizTrigger;

    public AutoCenterChildren autoCenter;

    private void Awake()
    {
        museumQuizManager = FindFirstObjectByType<MuseumQuizManager>();
    }

    private void Start()
    {
        ToggleSpotLights(false);
        CheckIfCompleted();
    }

    private void OnEnable()
    {
        museumQuizManager.OnMuseumQuestionSetChanged += CheckQuestionSet;
        museumQuizManager.OnQuizPassed += HideObjects;
    }

    private void OnDisable()
    {
        museumQuizManager.OnMuseumQuestionSetChanged -= CheckQuestionSet;
        museumQuizManager.OnQuizPassed += HideObjects;
    }

    private void CheckQuestionSet()
    {
        if (museumQuizManager.QuestionSet.quizId == relevantQuest.questionSet.quizId)
        {
            questionSet = museumQuizManager.QuestionSet;
            currentQuestionIndex = 0;
            ShowQuestion();
        }
    }

    private void CheckIfCompleted()
    {
        if (QuestManager.Instance.IsQuestCompleted(relevantQuest) || QuestManager.Instance.AreQuestObjectivesComplete(relevantQuest))
        {
            HideObjects();
            painting.DisplayCompleted();
            ToggleSpotLights(true);
        }
    }

    private void HideObjects()
    {
        foreach (var terminal in puzzleTerminals)
        {
            terminal.gameObject.SetActive(false);
        }
        confirmButton.gameObject.SetActive(false);
    }

    private void ShowQuestion()
    {
        QuestionData currentQuestion = questionSet.questions[currentQuestionIndex];
        
        // Track question ID for the last displayed question to aid hint generation
        QuestionManager.Instance.SetQuestionIndexForZone(ZoneId.Museum, currentQuestion.questionId);

        painting.DisplayQuestion(currentQuestion);
        painting.SetQuestionNumber(currentQuestionIndex + 1, questionSet.questions.Count);
        if (currentQuestionIndex == 0)
        {
            painting.SetPassRate(questionSet.passPercentage);
            painting.SetProgress(0, questionSet.questions.Count);
        }

        // Set the answer on all puzzle terminals
        for (int i = 0; i < puzzleTerminals.Count; i++)
        {
            if (i >= currentQuestion.answers.Count)
            {
                puzzleTerminals[i].Clear();
                puzzleTerminals[i].gameObject.SetActive(false);
            }
            else
            {
                puzzleTerminals[i].gameObject.SetActive(true);
                puzzleTerminals[i].SetAnswer(currentQuestion.answers[i], i);
            }
        }
        autoCenter.Recenter();
    }

    public List<int> GetSelectedAnswerIndices()
    {
        List<int> selectedIndices = new();

        foreach (var terminal in puzzleTerminals)
        {
            if (terminal.IsSelected)
            {
                selectedIndices.Add(terminal.AssociatedAnswerIndex);
            }
        }

        Debug.Log("Selected answer indices: " + string.Join(", ", selectedIndices));
        return selectedIndices;
    }

    public bool CanSelectMoreAnswers()
    {
        int selectedCount = puzzleTerminals.Count(t => t.IsSelected);
        int maxAllowed = questionSet.questions[currentQuestionIndex].maxSelections;

        return selectedCount < maxAllowed;
    }
    
    public void SubmitAnswers(List<int> selectedIndices)
    {
        QuestionData currentQuestion = questionSet.questions[currentQuestionIndex];

        museumQuizManager.SubmitAnswer(selectedIndices, currentQuestion, painting);

        currentQuestionIndex++;

        if (currentQuestionIndex < questionSet.questions.Count) Invoke(nameof(GoToQuestion), 5f); // Delay to allow feedback to be seen
    }

    private void GoToQuestion()
    {
        if (currentQuestionIndex < questionSet.questions.Count)
        {
            foreach (var terminal in puzzleTerminals)
            {
                terminal.Reset();
            }
            ShowQuestion();
        }
    }

    public void ToggleSpotLights(bool isActive)
    {
        foreach (var spotLight in spotLights)
        {
            spotLight.SetActive(isActive);
        }
    }
}