using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlidingPuzzleTerminal : InteractableItem
{
    [Header("Terminal Setup")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private PaintingManager paintingManager;
    [SerializeField] private Canvas exitButtonCanvas;

    private CameraFocusController cameraController;

    private int associatedAnswerIndex;
    public int AssociatedAnswerIndex => associatedAnswerIndex;

    public bool IsSelected => tileManager.IsSolved;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraFocusController>();
    }

    private void Start()
    {
        exitButtonCanvas.enabled = false;
    }

    private void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;
        tileManager.OnSolved += OnSolved;
        tileManager.OnUnsolved += OnUnsolved;
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
        tileManager.OnSolved -= OnSolved;
        tileManager.OnUnsolved -= OnUnsolved;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        ExitTerminal();
    }

    private void OnSolved()
    {
        UpdateVisuals(true);
        ExitTerminal();
    }

    private void OnUnsolved()
    {
        UpdateVisuals(false);
    }

    public override void Interact()
    {
        if (!IsInteractable || associatedAnswerIndex == -1) return;

        if (!paintingManager.CanSelectMoreAnswers() && !IsSelected)
        {
            FeedbackNotificationsUI.Instance.AddNotification("You cannot select more answers. Deselect one by unsolving the puzzle to be able to choose another.");
            return;
        }

        InteractionManager.Instance.StartInteraction();

        // Disabling inputs
        FirstPersonController.Instance.SetInputEnabled(false);
        FirstPersonController.Instance.SetCameraLookLocked(true);
        FirstPersonController.Instance.ResetCameraPitch();

        // Enabling mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Enabling terminal UI and focusing camera
        cameraController.FocusOnTerminal(cameraFocusPoint);
        exitButtonCanvas.enabled = true;

        // Sending camera to tile manager for on click events via raycasting
        if (tileManager != null) tileManager.SetCamera(Camera.main);

        tileManager.EnableColour();
    }

    public void ExitTerminal()
    {
        cameraController.ReturnToPlayer();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FirstPersonController.Instance.SetCameraLookLocked(false);
        FirstPersonController.Instance.SetInputEnabled(true);

        if (tileManager != null) tileManager.SetCamera(null);
        exitButtonCanvas.enabled = false;

        StartCoroutine(InteractionManager.Instance.DelayedEndInteraction());

        tileManager.DisableColour();
    }

    public void SetAnswer(string answer, int answerIndex)
    {
        answerText.text = answer;
        associatedAnswerIndex = answerIndex;
    }

    public void Clear()
    {
        answerText.text = "Out of Order";
        associatedAnswerIndex = -1;
    }

    private void UpdateVisuals(bool selected)
    {
        if (selected)
        {
            answerText.color = Color.green;
        }
        else
        {
            answerText.color = Color.white;
        }
    }

    public void Reset()
    {
        answerText.text = "";
        associatedAnswerIndex = -1;
        tileManager.ResetPuzzle();
        UpdateVisuals(false);
    }
}
