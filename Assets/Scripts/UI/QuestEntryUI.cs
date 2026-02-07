using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statusText;

    private QuestInstance boundQuest;
    private QuestJournalUI journalUI;

    private ColorBlock defaultButtonColours;
    private readonly string inProgressColourHex = "#FFD166";
    private readonly string completedColourHex = "#06D6A0";

    public void Bind(QuestInstance quest, QuestJournalUI journal)
    {
        boundQuest = quest;
        journalUI = journal;

        titleText.text = quest.questData.title;
        SetStatusText(quest.IsObjectivesComplete);

        button.onClick.AddListener(OnButtonClicked);

        defaultButtonColours = button.colors;
    }

    private void OnButtonClicked()
    {
        journalUI.ShowQuestDetails(boundQuest);
        journalUI.OnQuestEntrySelection(this);
    }

    public void SetSelectedVisual(bool selected)
    {
        if (selected)
        {
            ColorBlock selectedBlock = defaultButtonColours;
            selectedBlock.normalColor = defaultButtonColours.selectedColor;
            selectedBlock.highlightedColor = defaultButtonColours.selectedColor;
            selectedBlock.selectedColor = defaultButtonColours.selectedColor;

            button.colors = selectedBlock;
        }
        else
        {
            button.colors = defaultButtonColours;
        }
    }

    private void SetStatusText(bool isCompleted)
    {
        if (boundQuest.IsTurnedIn)
        {
            statusText.gameObject.SetActive(false);
            return;
        }

        statusText.text = isCompleted ? "Done" : "In Progress";

        string hexColour = isCompleted ? completedColourHex : inProgressColourHex;
        Color colour = ColorUtility.TryParseHtmlString(hexColour, out var parsedColor) ? parsedColor : Color.white;
        statusText.color = colour;
    }
}
