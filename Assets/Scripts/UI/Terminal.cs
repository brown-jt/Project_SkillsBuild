using UnityEngine;
using System.Collections.Generic;

public class Terminal : InteractableItem
{
    [Header("Terminal Setup")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private GameObject terminalUI;
    [SerializeField] private TerminalUIController uiController;

    private CameraFocusController cameraController;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraFocusController>();
        terminalUI.SetActive(false);
    }

    public override void Interact()
    {
        if (!IsInteractable) return;

        PlayerInputHandler.Instance.DisablePlayerInput();
        FirstPersonController.Instance.SetCameraLookLocked(false);
        FirstPersonController.Instance.ResetCameraPitch();
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
        FirstPersonController.Instance.SetCameraLookLocked(true);
        PlayerInputHandler.Instance.EnablePlayerInput();
    }

    public void ShowQuestion(string prompt, List<string> answers)
    {
        uiController.ShowQuestion(prompt, answers, (selectedIndex) => {});
    }
}
