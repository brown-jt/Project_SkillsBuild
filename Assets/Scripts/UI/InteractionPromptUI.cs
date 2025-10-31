using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptName;
    [SerializeField] private TextMeshProUGUI promptText;

    private bool isDisplayed = false;
    private void Start()
    {
        if (promptPanel == null || promptText == null || promptName == null)
        {
            Debug.LogError("InteractionPromptUI: UI elements are not assigned.");
        }
        else
        {
            // Hide the panel at start
            promptPanel.SetActive(false);
        }
    }

    public void Show(string message, string name = "")
    {
        if (isDisplayed) return;
        isDisplayed = true;
        promptPanel.SetActive(true);
        promptName.text = name;
        promptText.text = message;
    }

    public void Hide()
    {
        if (!isDisplayed) return;
        isDisplayed = false;
        promptPanel.SetActive(false);
    }
}
