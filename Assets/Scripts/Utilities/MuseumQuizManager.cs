using System.Collections.Generic;
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
    private int currentQustionIndex;
    
    public int CurrentQuestionIndex => currentQustionIndex;

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
            currentQustionIndex = 0;
        }
    }

    public void EndQuiz(bool passed)
    {
        if (passed)
        {
            quizTrigger.Passed(questionSet.quizId);
            questionSet = null;
        }
        else
        {
            // TODO: Reset quiz state to try again
        }
    }

    private void OnQuestUpdated(QuestInstance quest)
    {
        if (quest.questData.zoneId == ZoneId.Museum && quest.questData.questionSet != null && !quest.IsTurnedIn && !quest.IsObjectivesComplete)
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

    public void SubmitAnswer(List<int> selectedIndices, QuestionData questionData, Painting painting)
    {
        answeredQuestions++;

        // Checking contents against correct answer indices where order doesn't matter
        bool isCorrect = selectedIndices
            .OrderBy(x => x)
            .SequenceEqual(questionData.correctAnswerIndices.OrderBy(x => x));

        if (isCorrect) correctAnswers++;

        painting.DisplayResult(isCorrect, questionData);

        CheckQuizCompleted();
    }
}
