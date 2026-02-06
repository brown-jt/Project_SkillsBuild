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
    public Transform factoryActiveQuestsRoot;
    public Transform forestActiveQuestsRoot;
    public Transform warehouseActiveQuestsRoot;
    public Transform securityActiveQuestsRoot;
    public Transform museumActiveQuestsRoot;
    public Transform completedQuestsRoot;
    public Transform objectivesRoot;
    public Transform rewardsRoot;
    private Transform[] roots;

    [Header("Displays to Toggle")]
    public GameObject noActiveQuestsMessageLeft;
    public GameObject noActiveQuestsMessageRight;
    public GameObject noActiveQuestSelectedText;
    public GameObject activeQuestSelectedObject;

    [Header("Content Prefabs")]
    public QuestEntryUI questEntryPrefab;
    public ObjectivePrefabUI objectivePrefab;
    // Reward prefab

    [Header("Details Panel")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questZoneText;
    public TextMeshProUGUI questDescriptionText;

    private string currentTab = "Active";
    private bool hasSelectedQuest = false;
    private QuestInstance selectedQuest = null;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        roots = new Transform[]
        {
            factoryActiveQuestsRoot,
            forestActiveQuestsRoot,
            warehouseActiveQuestsRoot,
            securityActiveQuestsRoot,
            museumActiveQuestsRoot
        };

        // Upon game start, the quest journal should reflect any quests the player has already accepted, so we call RefreshQuestList to populate the UI
        // It should also start hidden
        RefreshQuestList();
        questJournalPanel.SetActive(false);
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

    private string GetZoneCourse(ZoneId zone)
    {
        switch(zone)
        {
            case ZoneId.Factory:
                return "Getting Started with Artificial Intelligence";
            case ZoneId.Forest:
                return "Getting Started with Threat Intelligence and Hunting";
            case ZoneId.Warehouse:
                return "Getting Started with Data";
            case ZoneId.Security:
                return "Getting Started with Cybersecurity";
            case ZoneId.Museum:
                return "Getting Started with Generative AI";
            default:
                return "Unknown Course";
        }
    }

    public void ShowQuestDetails(QuestInstance quest)
    {
        // Fill in the details panel with quest information
        questTitleText.text = quest.questData.title;
        questZoneText.text = $"{GetZoneCourse(quest.questData.zoneId)}\n<color=#888888><i>Zone: {quest.questData.zoneId}</i></color>";
        questDescriptionText.text = quest.questData.description;

        // Clearing old quest objectives and rewards
        ClearChildren(objectivesRoot);
        ClearChildren(rewardsRoot);

        // TODO: Populate objectives of quest
        foreach (ObjectiveProgress objProgress in quest.objectivesProgress)
        {
            ObjectivePrefabUI objUI = Instantiate(objectivePrefab, objectivesRoot);
            objUI.Bind(objProgress);
        }

        // TODO: Populate rewards of quest using below reward data
        RewardData rewards = quest.questData.rewards;

        // Show the details panel and hide the "No active quest selected" message
        hasSelectedQuest = true;
        selectedQuest = quest;
        noActiveQuestSelectedText.SetActive(false);
        activeQuestSelectedObject.SetActive(true);
    }

    private void ClearChildren(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Transform child = root.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void SetCurrentTab(string tabName)
    {
        currentTab = tabName;
    }

    public void RefreshQuestList()
    {
        foreach (Transform root in roots)
        {
            ClearChildren(root);
        }

        // Choose which quests to display based on current tab
        if (currentTab == "Active")
        {
            // Check each zone and populate the corresponding root with active quests for that zone
            PopulateQuestList(roots, QuestManager.Instance.activeQuests);
        }
        else if (currentTab == "Completed")
        {
            // TODO as current it's just a list of QuestIDs
        }

        // Check to see if a root has children, and if not, hide itself and title header by way of parent object
        bool hasAnyQuests = false;
        foreach (Transform root in roots)
        {
            bool hasQuests = root.childCount > 0;
            root.parent.gameObject.SetActive(hasQuests);

            if (hasQuests) hasAnyQuests = true;
        }

        // If none of the roots have quests, show a "No active quests" message in the UI
        noActiveQuestsMessageLeft.SetActive(!hasAnyQuests);
        noActiveQuestsMessageRight.SetActive(!hasAnyQuests);

        if (!hasAnyQuests) hasSelectedQuest = false;

        // However if there are active quests but no quest is selected, show a "No active quest selected" message in the details panel
        noActiveQuestSelectedText.SetActive(hasAnyQuests && !hasSelectedQuest);
        activeQuestSelectedObject.SetActive(hasSelectedQuest);

        // Furthermore update the selected quest details if there is a selected quest, to reflect any changes in progress
        if (hasSelectedQuest) ShowQuestDetails(selectedQuest);
    }

    private void PopulateQuestList(Transform[] roots, IEnumerable<QuestInstance> quests)
    {
        foreach (QuestInstance quest in quests)
        {
            Transform questRoot = null;
            switch(quest.questData.zoneId)
            {
                case ZoneId.Factory:
                    questRoot = roots[0];
                    break;
                case ZoneId.Forest:
                    questRoot = roots[1];
                    break;
                case ZoneId.Warehouse:
                    questRoot = roots[2];
                    break;
                case ZoneId.Security:
                    questRoot = roots[3];
                    break;
                case ZoneId.Museum:
                    questRoot = roots[4];
                    break;
            }
            QuestEntryUI entry = Instantiate(questEntryPrefab, questRoot);
            entry.Bind(quest, this);
        }
    }
}
