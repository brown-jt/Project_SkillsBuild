using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : InteractableItem
{
    [Header("Terminal Setup")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private Transform failCinematicFocusPoint;
    [SerializeField] private Transform successCinematicFocusPoint;
    [SerializeField] private GameObject terminalUI;
    [SerializeField] private TerminalUIController uiController;
    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private TriggerRelay scannerTriggerRelay;

    private QuestionSetData questionSet;

    private CameraFocusController cameraController;

    private QuestQuizTrigger quizTrigger;

    private GameObject robotInScanner;

    // Internal question structure
    private List<QuestionData> shuffledQuestions;
    private int currentQuestionIndex;
    private int correctCount;

    private bool quizActive = false;

    private void Awake()
    {
        scannerTriggerRelay.OnEnter += OnScannerEnter;
        scannerTriggerRelay.OnExit += OnScannerExit;
    }

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFocusController>();
        quizTrigger = GetComponent<QuestQuizTrigger>();
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

    private void OnScannerEnter(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            Debug.Log($"Entered: {other.name}");
            robotInScanner = other.gameObject;
        }
    }

    private void OnScannerExit(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            Debug.Log($"Exited: {other.name}");
            robotInScanner = null;
        }
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        ExitTerminal();
    }

    public override void Interact()
    {
        if (!IsInteractable) return;
        if (!robotInScanner) 
        {
            FeedbackNotificationsUI.Instance.AddNotification("Please wait for a robot to enter the scanner before accessing the terminal.", 3f);
            return;
        }

        Debug.Log("Checking active quests " + QuestManager.Instance.activeQuests.Count);

        foreach (QuestInstance questInstance in QuestManager.Instance.activeQuests)
        {
            if (questInstance.questData.questionSet != null && questInstance.questData.zoneId == ZoneId.Factory)
            {
                Debug.Log($"Found question set for quest: {questInstance.questData.title}");
                questionSet = questInstance.questData.questionSet;
                break;
            }
        }

        if (questionSet == null)
        {
            FeedbackNotificationsUI.Instance.AddNotification("No active quiz found. Accept a quest first then try again.", 3f);
            return;
        }

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

        if (currentQuestionIndex == 0 && !quizActive) // Only show start message if we haven't started the quiz yet
            uiController.QuizStart(
            $"{questionSet.moduleName}", $"This is a {questionSet.questions.Count}-question quiz. " +
            $"You must get {questionSet.passPercentage*100}% to pass it. Don’t worry though! " +
            $"If you aren’t successful at first, you can review the course and retake the quiz " +
            $"as many times as needed for your completion."
        );

        else ShowCurrentQuestion();

        quizActive = true;
    }

    public void ExitTerminal()
    {
        if (!quizActive) terminalUI.SetActive(false);

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

        uiController.ShowQuestion(q.question, q.answers, q.maxSelections, selectedIndexes =>
        {
            bool isCorrect = AreAnswersCorrect(selectedIndexes, q.correctAnswerIndices);

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

        string scoreText = $"Score: {score * 100:F2}% ({correctCount}/{shuffledQuestions.Count})";

        // If passed, we need to trigger the objective completion event for the quest that contains this question set
        if (passed)
        {
            quizTrigger.Passed(questionSet.quizId);
            questionSet = null;
        }

        HandleRobotInScanner(passed);
        uiController.ShowFinalResult(passed, scoreText);
        quizActive = false;
        currentQuestionIndex = 0;
    }

    private bool AreAnswersCorrect(List<int> selected, List<int> correct)
    {
        // Must select same number of answers
        if (selected.Count != correct.Count)
            return false;

        // Compare ignoring order
        return !selected.Except(correct).Any();
    }

    private void HandleRobotInScanner(bool passed)
    {
        if (robotInScanner == null) return;

        if (passed) robotInScanner.GetComponent<RobotStaticMover>().StartWalking();
        else robotInScanner.GetComponent<DissolveController>().StartDissolve();

        StartCoroutine(Cinematic(passed));
    }

    private IEnumerator Cinematic(bool success)
    {
        if (success) cameraController.FocusOnTerminal(successCinematicFocusPoint);
        else cameraController.FocusOnTerminal(failCinematicFocusPoint);

        PlayerInputHandler.Instance.DisablePlayerInput();
        PlayerInputHandler.Instance.DisablePlayerUIInput();

        yield return new WaitForSeconds(4f);

        PlayerInputHandler.Instance.EnablePlayerInput();
        PlayerInputHandler.Instance.EnablePlayerUIInput();
        cameraController.FocusOnTerminal(cameraFocusPoint);
    }
}
