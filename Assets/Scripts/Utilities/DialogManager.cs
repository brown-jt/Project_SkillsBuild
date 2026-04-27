using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    public Queue<string> sentences = new Queue<string>();

    public GameObject dialogPanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogText;
    public float textSpeed = 0.015f;

    public FirstPersonController playerController;

    private Coroutine typingCoroutine;
    private string currentSentence;

    private System.Action onDialogComplete;
    public InputActionReference cancelAction;

    public GameObject keybindsUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += CancelDialog;
    }

    private void OnDisable()
    {
        cancelAction.action.performed -= CancelDialog;
        cancelAction.action.Disable();
    }

    private void CancelDialog(InputAction.CallbackContext ctx)
    {
        dialogPanel.SetActive(false);
        keybindsUI.SetActive(true);
        playerController.SetInputEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(InteractionManager.Instance.DelayedEndInteraction());
    }

    private void Start()
    {
        dialogPanel.SetActive(false);
    }

    /// <summary>
    /// Function that takes in DialogData and starts the dialog sequence. It also takes an optional callback that will be invoked when the dialog is complete.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onComplete"></param>
    public void StartDialog(DialogData data, string name, System.Action onComplete = null)
    {
        if (data == null)
        {
            FeedbackNotificationsUI.Instance.AddNotification("This character has nothing to say.");
            return;
        }

        dialogPanel.SetActive(true);
        keybindsUI.SetActive(false);
        playerController.SetInputEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        characterNameText.text = name;
        sentences.Clear();
        onDialogComplete = onComplete;

        foreach (string line in data.lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();

        InteractionManager.Instance.StartInteraction();
    }

    public void DisplayNextSentence()
    {
        // If a sentence is being typed, finish and display immediately
        if (typingCoroutine != null && dialogText.text != currentSentence)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = currentSentence;
            typingCoroutine = null;
            return;
        }

        // If no more sentences, end dialog
        if (sentences.Count == 0)
        {
            EndDialog();
            return;
        }

        // Otherwise display next sentence
        currentSentence = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    private void EndDialog()
    {
        dialogPanel.SetActive(false);
        keybindsUI.SetActive(true);
        playerController.SetInputEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onDialogComplete?.Invoke();
        onDialogComplete = null;

        StartCoroutine(InteractionManager.Instance.DelayedEndInteraction());
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
