using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestJournalToggle : MonoBehaviour
{
    public GameObject activeScrollView;
    public GameObject completedScrollView;

    public Button activeButton;
    public Button completedButton;

    private ColorBlock activeButtonColors;
    private ColorBlock completedButtonColors;

    private void Awake()
    {
        // Caching the original colours for the buttons
        activeButtonColors = activeButton.colors;
        completedButtonColors = completedButton.colors;
    }

    private void Start()
    {
        ViewActive(); // Default view on first open
    }

    public void ViewActive()
    {
        // Toggle content
        completedScrollView.SetActive(false);
        activeScrollView.SetActive(true);
        QuestJournalUI.Instance.SetCurrentTab("Active");

        // Manually update button visuals
        SetButtonSelected(activeButton, completedButton);
    }

    public void ViewCompleted()
    {
        // Toggle content
        activeScrollView.SetActive(false);
        completedScrollView.SetActive(true);
        QuestJournalUI.Instance.SetCurrentTab("Completed");

        // Manually update button visuals
        SetButtonSelected(completedButton, activeButton);
    }

    private void SetButtonSelected(Button selected, Button deselected)
    {
        // Set the selected button to its pressed state
        ColorBlock selectedColors = selected.colors;
        selectedColors.normalColor = selectedColors.pressedColor;
        selected.colors = selectedColors;

        // Reset the deselected button to its original colors
        if (deselected == activeButton)
            deselected.colors = activeButtonColors;
        else
            deselected.colors = completedButtonColors;
    }
}
