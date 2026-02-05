using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestJournalUI : MonoBehaviour
{
    public static QuestJournalUI Instance;

    public GameObject questJournalPanel;
    [SerializeField] private InputActionReference questJournalAction;
    [SerializeField] private InputActionReference cancelAction;

    [Header("Content Roots")]
    public Transform activeQuestsRoot;
    public Transform completedQuestsRoot;
    public Transform objectivesRoot;
    public Transform rewardsRoot;

    [Header("Content Prefabs")]
    public QuestEntryUI questEntryPrefab;
    // Objective prefab
    // Reward prefab

    [Header("Details Panel")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questZoneText;
    public TextMeshProUGUI questDescriptionText;

    private string currentTab = "Active";

    void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        questJournalAction.action.Enable();
        questJournalAction.action.performed += ToggleQuestJournal;

        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        questJournalAction.action.performed -= ToggleQuestJournal;
        questJournalAction.action.Disable();

        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void ToggleQuestJournal(InputAction.CallbackContext ctx)
    {
        bool isOpen = questJournalPanel.activeSelf;

        if (isOpen) CloseJournal();
        else OpenJournal();
    }
    
    private void OpenJournal()
    {
        PlayerInputHandler.Instance.DisablePlayerInput();
        questJournalPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseJournal()
    {
        PlayerInputHandler.Instance.EnablePlayerInput();
        questJournalPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (questJournalPanel.activeSelf) CloseJournal();
    }

    public void ShowQuestDetails(QuestInstance quest)
    {
        // Fill in the details panel with quest information
        questTitleText.text = quest.questData.title;
        questZoneText.text = quest.questData.zoneId.ToString();
        questDescriptionText.text = quest.questData.description;

        // Clearing old quest objectives and rewards
        ClearChildren(objectivesRoot);
        ClearChildren(rewardsRoot);

        // TODO: Populate objectives of quest

        // TODO: Populate rewards of quest

    }

    private void ClearChildren(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Transform child = root.GetChild(i);

            // Skip the Title so it isn't removed
            if (child.name == "Title") continue;

            Destroy(child.gameObject);
        }
    }

    public void SetCurrentTab(string tabName)
    {
        currentTab = tabName;
    }

    public void RefreshQuestList()
    {
        ClearChildren(activeQuestsRoot);
        ClearChildren(completedQuestsRoot);

        // Choose which quests to display based on current tab
        if (currentTab == "Active")
        {
            PopulateQuestList(activeQuestsRoot, QuestManager.Instance.activeQuests);
        }
        else if (currentTab == "Completed")
        {
            // TODO as current it's just a list of QuestIDs
        }
    }

    private void PopulateQuestList(Transform root, IEnumerable<QuestInstance> quests)
    {
        foreach (QuestInstance quest in quests)
        {
            QuestEntryUI entry = Instantiate(questEntryPrefab, root);
            entry.Bind(quest, this);
        }
    }
}
