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

        terminalUI.SetActive(true);
        cameraController.FocusOnTerminal(cameraFocusPoint);
        PlayerInputHandler.Instance.DisablePlayerInput();

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
        PlayerInputHandler.Instance.EnablePlayerInput();
    }

    public void ShowQuestion(string prompt, List<string> answers)
    {
        uiController.ShowQuestion(prompt, answers, (selectedIndex) => {});
    }
}
