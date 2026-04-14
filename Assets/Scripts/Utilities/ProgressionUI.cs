using UnityEngine;
using UnityEngine.InputSystem;

public class ProgressionUI : MonoBehaviour
{
    public GameObject progressionPanel;
    [SerializeField] private InputActionReference progressionAction;
    [SerializeField] private InputActionReference cancelAction;

    private void OnEnable()
    {
        progressionAction.action.Enable();
        progressionAction.action.performed += TogglePanel;

        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        progressionAction.action.performed -= TogglePanel;
        progressionAction.action.Disable();

        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void TogglePanel(InputAction.CallbackContext ctx)
    {
        bool isOpen = progressionPanel.activeSelf;

        if (isOpen) ClosePanel();
        else OpenPanel();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (progressionPanel.activeSelf) ClosePanel();
    }

    private void OpenPanel()
    {
        PlayerInputHandler.Instance.DisablePlayerInput();
        progressionPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ClosePanel()
    {
        PlayerInputHandler.Instance.EnablePlayerInput();
        progressionPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
