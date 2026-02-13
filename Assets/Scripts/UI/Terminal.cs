using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : InteractableItem
{
    [Header("Terminal Setup")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private GameObject terminalUI;
    [SerializeField] private TerminalUIController uiController;
    [SerializeField] private InputActionReference cancelAction;

    [Header("Question Content")]
    [SerializeField] private QuestionSetData questionSet;

    private CameraFocusController cameraController;

    // Internal question structure
    private List<QuestionData> shuffledQuestions;
    private int currentQuestionIndex;
    private int correctCount;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFocusController>();
        terminalUI.SetActive(false);
    }

    private void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        ExitTerminal();
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        // Disabling inputs
        FirstPersonController.Instance.SetInputEnabled(false);
        FirstPersonController.Instance.SetCameraLookLocked(true);
        FirstPersonController.Instance.ResetCameraPitch();

        // Enabling mouse
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

        // Enabling terminal UI and focusing camera
        terminalUI.SetActive(true);
        cameraController.FocusOnTerminal(cameraFocusPoint);

        uiController.QuizStart(
            $"{questionSet.moduleName}", $"This is a {questionSet.questions.Count}-question quiz. " +
            $"You must get {questionSet.passPercentage*100}% to pass it. Don’t worry though! " +
            $"If you aren’t successful at first, you can review the course and retake the quiz " +
            $"as many times as needed for your completion."
        );
    }

    public void ExitTerminal()
    {
        terminalUI.SetActive(false);
        cameraController.ReturnToPlayer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FirstPersonController.Instance.SetCameraLookLocked(false);
        FirstPersonController.Instance.SetInputEnabled(true);
    }

    public void StartQuestionSet()
    {
        shuffledQuestions = new List<QuestionData>(questionSet.questions);
        shuffledQuestions = shuffledQuestions.OrderBy(q => Random.value).ToList();

        currentQuestionIndex = 0;
        correctCount = 0;

        ShowCurrentQuestion();
    }

    private void ShowCurrentQuestion()
    {
        if (currentQuestionIndex >= shuffledQuestions.Count)
        {
            EndQuestionSet();
            return;
        }

        QuestionData q = shuffledQuestions[currentQuestionIndex];

        uiController.ShowQuestion(q.question, q.answers, selectedIndex =>
        {
            bool isCorrect = q.correctAnswerIndices.Contains(selectedIndex);

            if (isCorrect)
                correctCount++;

            uiController.ShowFeedback(isCorrect, isCorrect ? q.correctMessage : q.incorrectMessage, () =>
            {
                currentQuestionIndex++;
                ShowCurrentQuestion();
            });
        });
    }

    private void EndQuestionSet()
    {
        float score = (float)correctCount / shuffledQuestions.Count;
        bool passed = score >= questionSet.passPercentage;

        string scoreText = $"Score: {score*100:F2}% ({correctCount}/{shuffledQuestions.Count})";

        uiController.ShowFinalResult(passed, scoreText);
    }
}
