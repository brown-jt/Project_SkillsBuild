using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

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

    private void Start()
    {
        dialogPanel.SetActive(false);
    }

    public void StartDialog(DialogData data, System.Action onComplete = null)
    {
        if (data == null)
        {
            Debug.LogError("No DialogData provided!");
            return;
        }

        dialogPanel.SetActive(true);
        playerController.SetInputEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        characterNameText.text = data.characterName;
        sentences.Clear();
        onDialogComplete = onComplete;

        foreach (string line in data.lines)
        {
            Debug.Log("Enqueuing line: " + line);
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
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
        Debug.Log("Dialog ended.");

        dialogPanel.SetActive(false);
        playerController.SetInputEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onDialogComplete?.Invoke();
        onDialogComplete = null;
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
