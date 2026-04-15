using System.Linq;
using TMPro;
using UnityEngine;

public class WarehouseQuizManager : MonoBehaviour
{
    [Header("Default Minigame Rewards")]
    [SerializeField] private int goldReward = 100;
    [SerializeField] private float experienceReward = 50.0f;

    [Header("References")]
    [SerializeField] private GameObject timerUI;
    [SerializeField] private TextMeshProUGUI statusUI;
    [SerializeField] private TextMeshProUGUI timerText;

    private float timer = 0.0f;
    private bool quizActive = false;
    private QuestQuizTrigger quizTrigger;

    // To change current question set
    public QuestionSetData questionSet;

    // Score tracking
    private int totalQuestions;
    private int answeredQuestions;
    private int correctAnswers;

    private void Start()
    {
        timerUI.SetActive(false);
        quizTrigger = GetComponent<QuestQuizTrigger>();
    }

    private void Update()
    {
        if (quizActive)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        float milliseconds = (timer * 1000) % 1000;

        if (minutes > 0)
            timerText.text = $"{minutes}:{seconds:00}:{milliseconds:000}";
        else
            timerText.text = $"{seconds}:{milliseconds:000}";
    }

    public void StartQuiz()
    {
        timer = 0.0f;
        totalQuestions = questionSet.questions.Count;

        statusUI.text = $"Pass Percentage: {questionSet.passPercentage * 100}%\n" +
                        $"Current Score: {correctAnswers}/{totalQuestions}";

        quizActive = true;
        timerUI.SetActive(true);
    }

    public void EndQuiz(bool passed)
    {
        quizActive = false;
        timerUI.SetActive(false);

        if (passed)
        {
            quizTrigger.Passed(questionSet.quizId);
            questionSet = null;
            // TODO: Determine rewards based on performance (e.g., time taken, correct answers)
        }

        else
        {
            var gotPercentage = (float)correctAnswers / totalQuestions * 100;
            FeedbackBannerUI.Instance.ShowBanner("Minigame Failed!", "Press the button to call the truck when ready to try again!", $"You got {gotPercentage}% correct. You needed {questionSet.passPercentage * 100}% to pass.");
        }
    }

    private void CheckQuizCompleted()
    {
        if (answeredQuestions >= totalQuestions)
        {
            float score = (float)correctAnswers / totalQuestions;
            bool passed = score >= questionSet.passPercentage;

            string scoreText = $"Score: {score * 100:F2}% ({correctAnswers}/{totalQuestions})";

            // If passed, we need to trigger the objective completion event for the quest that contains this question set
            EndQuiz(passed);
        }
    }

    public void SubmitAnswer(string answer, QuestionData questionData, Shelf shelf)
    {
        answeredQuestions++;

        // Assuming only one correct answer for simplicity for now
        // TODO: Handle multiple correct answers somehow later
        int correctIndex = questionData.correctAnswerIndices.FirstOrDefault();
        string correctAnswer = questionData.answers[correctIndex];

        bool isCorrect = answer == correctAnswer;

        if (isCorrect) correctAnswers++;

        shelf.DisplayResult(isCorrect, questionData);

        statusUI.text = $"Pass Percentage: {questionSet.passPercentage*100}%\n" +
                        $"Current Score: {correctAnswers}/{totalQuestions}";

        CheckQuizCompleted();
    }
}
