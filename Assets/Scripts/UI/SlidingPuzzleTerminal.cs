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

    private CameraFocusController cameraController;

    private string associatedAnswer;

    private bool puzzleFinished => tileManager.IsSolved;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFocusController>();
    }

    private void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;
        tileManager.OnComplete += ExitTerminal;
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
        tileManager.OnComplete -= ExitTerminal;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        ExitTerminal();
    }

    public override void Interact()
    {
        if (!IsInteractable || puzzleFinished || associatedAnswer == string.Empty) return;

        // Disabling inputs
        FirstPersonController.Instance.SetInputEnabled(false);
        FirstPersonController.Instance.SetCameraLookLocked(true);
        FirstPersonController.Instance.ResetCameraPitch();

        // Enabling mouse
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

        // Enabling terminal UI and focusing camera
        cameraController.FocusOnTerminal(cameraFocusPoint);

        // Sending camera to tile manager for on click events via raycasting
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

    public void SetAnswer(string answer)
    {
        answerText.text = answer;
        associatedAnswer = answer;
    }

    public void Clear()
    {
        associatedAnswer = string.Empty;
        answerText.text = "Out of Order";
    }
}
