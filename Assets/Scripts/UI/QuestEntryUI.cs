using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI titleText;

    private QuestInstance boundQuest;
    private QuestJournalUI journalUI;

    private ColorBlock defaultButtonColours;

    public void Bind(QuestInstance quest, QuestJournalUI journal)
    {
        boundQuest = quest;
        journalUI = journal;

        titleText.text = quest.questData.title;

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
}
