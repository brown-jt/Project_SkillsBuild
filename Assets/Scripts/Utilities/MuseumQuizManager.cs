using System;
using System.Linq;
using UnityEngine;

public class MuseumQuizManager : MonoBehaviour
{
    // Question set being asked to the player currently
    private QuestionSetData questionSet;
    public QuestionSetData QuestionSet => questionSet;

    // Action to subscribe to for paintings/puzzle terminals
    public Action OnMuseumQuestionSetChanged;
    private QuestQuizTrigger quizTrigger;

    // Score tracking
    private int totalQuestions;
    private int answeredQuestions;
    private int correctAnswers;

    private void Start()
    {
        quizTrigger = GetComponent<QuestQuizTrigger>();
    }

    private void OnEnable()
    {
        QuestManager.Instance.onQuestUpdated += OnQuestUpdated;
    }

    public void SetQuestionSet(QuestionSetData questionSet)
    {
        this.questionSet = questionSet;
        OnMuseumQuestionSetChanged?.Invoke();
        StartQuiz();
    }

    public void StartQuiz()
    {
        if (questionSet != null)
        {
            totalQuestions = questionSet.questions.Count;
        }
    }

    public void EndQuiz(bool passed)
    {
        if (passed)
        {
            quizTrigger.Passed(questionSet.quizId);
            questionSet = null;
        }
    }

    private void OnQuestUpdated(QuestInstance quest)
    {
        if (quest.questData.zoneId == ZoneId.Museum && !quest.IsTurnedIn && quest.questData.questionSet != null)
        {
            SetQuestionSet(quest.questData.questionSet);
        }
        else
        {
            // TODO if required to handle
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

    public void SubmitAnswer(string answer, QuestionData questionData, Painting painting)
    {
        answeredQuestions++;

        // Assuming only one correct answer for simplicity for now
        // TODO: Handle multiple correct answers somehow later
        int correctIndex = questionData.correctAnswerIndices.FirstOrDefault();
        string correctAnswer = questionData.answers[correctIndex];

        bool isCorrect = answer == correctAnswer;

        if (isCorrect) correctAnswers++;

        painting.DisplayResult(isCorrect, questionData);

        CheckQuizCompleted();
    }
}
