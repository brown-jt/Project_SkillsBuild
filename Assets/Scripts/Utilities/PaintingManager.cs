using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    public string relevantQuizId;
    public Painting painting;
    public SlidingPuzzleTerminal[] puzzleTerminals;

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
        Debug.Log("CHECKING QUESTION SET: " + museumQuizManager.QuestionSet.quizId);
        if (museumQuizManager.QuestionSet.quizId == relevantQuizId)
        {
            questionSet = museumQuizManager.QuestionSet;
            currentQuestionIndex = 0;
            StartQuiz();
        }
    }

    private void StartQuiz()
    {
        QuestionData currentQuestion = questionSet.questions[currentQuestionIndex];
        painting.DisplayQuestion(currentQuestion);
        // Set the answer on all puzzle terminals
        for (int i = 0; i < puzzleTerminals.Length; i++)
        {
            if (i >= currentQuestion.answers.Count)
            {
                puzzleTerminals[i].Clear();
                puzzleTerminals[i].gameObject.SetActive(false);
            }
            else
            {
                puzzleTerminals[i].gameObject.SetActive(true);
                puzzleTerminals[i].SetAnswer(currentQuestion.answers[i]);
            }
        }
        autoCenter.Recenter();
    }
}
