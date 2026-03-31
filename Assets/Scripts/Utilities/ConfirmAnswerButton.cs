using System.Collections.Generic;
using UnityEngine;

public class ConfirmAnswerButton : InteractableItem
{
    [Header("References")]
    [SerializeField] private MuseumQuizManager quizManager;
    [SerializeField] private PaintingManager paintingManager;

    private void Start()
    {
        quizManager = FindFirstObjectByType<MuseumQuizManager>();
    }
    override public void Interact()
    {
        if (!IsInteractable || quizManager == null || paintingManager == null) return;

        List<int> selectedAnswers = paintingManager.GetSelectedAnswerIndices();

        if (selectedAnswers.Count == 0)
        {
            FeedbackNotificationsUI.Instance.AddNotification("You must select at least one answer before confirming.");
            return;
        }

        int maxSelections = quizManager.QuestionSet.questions[quizManager.CurrentQuestionIndex].maxSelections;

        if (selectedAnswers.Count > maxSelections)
        {
            FeedbackNotificationsUI.Instance.AddNotification($"Maximum answer selection is {maxSelections}. Please modify your selections accordingly.");
            return;
        }

        paintingManager.SubmitAnswers(selectedAnswers);
    }
}
