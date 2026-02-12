using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : InteractableItem
{
    [Header("Terminal Setup")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private GameObject terminalUI;
    [SerializeField] private TerminalUIController uiController;
    [SerializeField] private InputActionReference cancelAction;

    private CameraFocusController cameraController;

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

        ShowQuestion
        (
            "This is a test question designed to show upon interacting with the terminal", new List<string>
            {
                "Option One",
                "Option Two",
                "Option Three",
                "Option Four",
            }
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

    public void ShowQuestion(string prompt, List<string> answers)
    {
        uiController.ShowQuestion(prompt, answers, (selectedIndex) => {});
    }
}
