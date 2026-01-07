using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DevConsoleUI : MonoBehaviour
{
    public GameObject consolePanel;
    public TMP_InputField inputField;
    public TMP_Text logText;
    public ScrollRect scrollRect;

    private bool isOpen;

    private DevConsoleInput inputActions;

    private void Awake()
    {
        consolePanel.SetActive(false);
        inputActions = new DevConsoleInput();
    }

    private void OnEnable()
    {
        inputActions.UI.ToggleConsole.performed += _ => ToggleConsole();
        inputActions.UI.CloseConsole.performed += _ => CloseConsole();
        inputActions.UI.SubmitCommand.performed += _ => { if (isOpen) SubmitCommand(); };
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.UI.ToggleConsole.performed -= _ => ToggleConsole();
        inputActions.UI.CloseConsole.performed -= _ => CloseConsole();
        inputActions.UI.SubmitCommand.performed -= _ => { if (isOpen) SubmitCommand(); };
    }

    private void ToggleConsole()
    {
        isOpen = !isOpen;
        consolePanel.SetActive(isOpen);

        if (isOpen)
        {
            inputField.text = "";
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    private void CloseConsole()
    {
        if (isOpen) isOpen = false;
    }

    private void SubmitCommand()
    {
        string input = inputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        Log($"> {input}");
        DevConsole.Instance.Execute(input);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void Log(string msg)
    {
        logText.text += msg + "\n";

        // Scroll to bottom
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public bool IsOpen() => isOpen;
}
