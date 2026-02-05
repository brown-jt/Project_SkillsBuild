using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI zoneText;

    private QuestInstance boundQuest;
    private QuestJournalUI journalUI;

    public void Bind(QuestInstance quest, QuestJournalUI journal)
    {
        boundQuest = quest;
        journalUI = journal;

        Debug.Log($"Binding quest entry UI: {quest.questData.title} in zone {quest.questData.zoneId}");

        titleText.text = quest.questData.title;
        zoneText.text = quest.questData.zoneId.ToString();

        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        journalUI.ShowQuestDetails(boundQuest);
    }
}
