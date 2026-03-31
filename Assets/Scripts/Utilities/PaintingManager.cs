using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    public string relevantQuizId;
    public Painting painting;
    public List<SlidingPuzzleTerminal> puzzleTerminals;

    private QuestionSetData questionSet;
    private MuseumQuizManager museumQuizManager;

    private int currentQuestionIndex;
    [SerializeField] private QuestQuizTrigger questQuizTrigger;

    public AutoCenterChildren autoCenter;

    private void Awake()
    {
        museumQuizManager = FindFirstObjectByType<MuseumQuizManager>();
    }

    private void OnEnable()
    {
        museumQuizManager.OnMuseumQuestionSetChanged += CheckQuestionSet;
    }

    private void OnDisable()
    {
        museumQuizManager.OnMuseumQuestionSetChanged -= CheckQuestionSet;
    }

    private void CheckQuestionSet()
    {
        if (museumQuizManager.QuestionSet.quizId == relevantQuizId)
        {
            questionSet = museumQuizManager.QuestionSet;
            currentQuestionIndex = 0;
            ShowQuestion();
        }
    }

    private void ShowQuestion()
    {
        QuestionData currentQuestion = questionSet.questions[currentQuestionIndex];
        painting.DisplayQuestion(currentQuestion);
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

        Invoke(nameof(GoToQuestion), 5f); // Delay to allow feedback to be seen
    }

    private void GoToQuestion()
    {
        if (currentQuestionIndex >= questionSet.questions.Count)
        {
            return;
        }

        foreach (var terminal in puzzleTerminals)
        {
            terminal.Reset();
        }

        ShowQuestion();
    }
}
