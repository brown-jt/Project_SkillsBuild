using UnityEngine;
using UnityEngine.InputSystem;

public class SlidingPuzzleTerminal : InteractableItem
{
    [Header("Terminal Setup")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private TileManager tileManager;

    private QuestionSetData questionSet;

    private CameraFocusController cameraController;

    private QuestQuizTrigger quizTrigger;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFocusController>();
        quizTrigger = GetComponent<QuestQuizTrigger>();
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

        Debug.Log("Checking active quests " + QuestManager.Instance.activeQuests.Count);

        foreach (QuestInstance questInstance in QuestManager.Instance.activeQuests)
        {
            if (questInstance.questData.questionSet != null)
            {
                Debug.Log($"Found question set for quest: {questInstance.questData.title}");
                questionSet = questInstance.questData.questionSet;
                break;
            }
        }

        if (questionSet == null)
        {
            Debug.LogError("No Quests with QuestionSet data to show!");
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
        cameraController.FocusOnTerminal(cameraFocusPoint);

        // Sending camera to terminal
        if (tileManager != null) tileManager.SetCamera(Camera.main);
    }

    public void ExitTerminal()
    {
        cameraController.ReturnToPlayer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FirstPersonController.Instance.SetCameraLookLocked(false);
        FirstPersonController.Instance.SetInputEnabled(true);

        if (tileManager != null) tileManager.SetCamera(null);
    }
}
