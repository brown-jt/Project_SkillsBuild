using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestJournalUI : MonoBehaviour
{
    public static QuestJournalUI Instance;

    public GameObject questJournalPanel;
    [SerializeField] private InputActionReference questJournalAction;
    [SerializeField] private InputActionReference cancelAction;

    [Header("Active Content Roots")]
    public Transform factoryActiveQuestsRoot;
    public Transform forestActiveQuestsRoot;
    public Transform warehouseActiveQuestsRoot;
    public Transform securityActiveQuestsRoot;
    public Transform museumActiveQuestsRoot;
    private Transform[] activeRoots;

    [Header("Completed Content Roots")]
    public Transform factoryCompletedQuestsRoot;
    public Transform forestCompletedQuestsRoot;
    public Transform warehouseCompletedQuestsRoot;
    public Transform securityCompletedQuestsRoot;
    public Transform museumCompletedQuestsRoot;
    private Transform[] completedRoots;

    private Transform[] allQuestroots;

    [Header("Objectives Root")]
    public Transform objectivesRoot;

    [Header("Active Section gameobjects to toggle")]
    public GameObject noActiveQuestsMessageLeft;
    public GameObject noActiveQuestsMessageRight;
    public GameObject noActiveQuestSelectedText;
    public GameObject activeQuestSelectedObject;

    [Header("Completed Section gameobjects to toggle")]
    public GameObject noCompletedQuestsMessageLeft;
    public GameObject noCompletedQuestsMessageRight;
    public GameObject noCompletedQuestSelectedText;
    public GameObject completedQuestSelectedObject;
    [Space]
    public GameObject rightPanelActive;
    public GameObject rightPanelCompleted;

    [Header("Active Rewards References")]
    public TextMeshProUGUI goldRewardText;
    public TextMeshProUGUI xpRewardText;
    // TODO: Add item rewards here as well when that system is implemented

    [Header("Completed Rewards References")]
    public TextMeshProUGUI completedGoldRewardText;
    public TextMeshProUGUI completedXpRewardText;
    // TODO: Add item rewards here as well when that system is implemented

    [Header("Content Prefabs")]
    public QuestEntryUI questEntryPrefab;
    public ObjectivePrefabUI objectivePrefab;

    [Header("Active Quest Details Panel")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questZoneText;
    public TextMeshProUGUI questDescriptionText;

    [Header("Completed Quest Details Panel")]
    public TextMeshProUGUI completedQuestTitleText;
    public TextMeshProUGUI completedQuestZoneText;
    public TextMeshProUGUI completedQuestDescriptionText;

    private string currentTab = "Active";

    private bool hasSelectedQuest = false;
    private QuestInstance selectedQuest = null;

    private bool hasSelectedCompletedQuest = false;
    private QuestInstance selectedCompletedQuest = null;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        activeRoots = new Transform[]
        {
            factoryActiveQuestsRoot,
            forestActiveQuestsRoot,
            warehouseActiveQuestsRoot,
            securityActiveQuestsRoot,
            museumActiveQuestsRoot
        };

        completedRoots = new Transform[]
        {
            factoryCompletedQuestsRoot,
            forestCompletedQuestsRoot,
            warehouseCompletedQuestsRoot,
            securityCompletedQuestsRoot,
            museumCompletedQuestsRoot
        };

        // Combine all roots into a single array for easier management when clearing
        allQuestroots = new Transform[activeRoots.Length + completedRoots.Length];
        activeRoots.CopyTo(allQuestroots, 0);
        completedRoots.CopyTo(allQuestroots, activeRoots.Length);

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

        if (selectedQuest != null)
            Debug.Log("Currently selected quest: " + selectedQuest.questData.title);
        else
            Debug.Log("No currently selected quest.");
    }

    public void CloseQuestJournal()
    {
        CloseJournal();
    }

    private void OpenJournal()
    {
        // Refresh the quest list to ensure the most up-to-date information is shown whenever the journal is opened.
        // This is important in case there were any changes to quests while the journal was closed that weren't externally handled via events.
        RefreshQuestList();

        PlayerInputHandler.Instance.DisablePlayerInput();
        questJournalPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioManager.Instance.PlaySFX("Quest_Open");
    }

    private void CloseJournal()
    {
        PlayerInputHandler.Instance.EnablePlayerInput();
        questJournalPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        AudioManager.Instance.PlaySFX("Quest_Close");
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
        if (!quest.IsTurnedIn)
        {
            // ===== ACTIVE SECTION =====
            // Fill in the active details panel with quest information
            questTitleText.text = quest.questData.title;
            questZoneText.text = $"{GetZoneCourse(quest.questData.zoneId)}\n<color=#888888><i>Zone: {quest.questData.zoneId}</i></color>";
            questDescriptionText.text = quest.questData.description;

            // Clearing old quest objectives
            ClearChildren(objectivesRoot);

            // Populate objectives of quest
            foreach (ObjectiveProgress objProgress in quest.objectivesProgress)
            {
                ObjectivePrefabUI objUI = Instantiate(objectivePrefab, objectivesRoot);
                objUI.Bind(objProgress);
            }

            // Alter rewards of quest using below reward data
            RewardData rewards = quest.questData.rewards;
            goldRewardText.text = rewards.gold.ToString();
            xpRewardText.text = rewards.experience.ToString();

            // Show the details panel and hide the "No active quest selected" message
            hasSelectedQuest = true;
            selectedQuest = quest;

            noActiveQuestSelectedText.SetActive(false);
            activeQuestSelectedObject.SetActive(true);
        }
        else
        {
            // ===== COMPLETED SECTION =====
            // Fill in the completed details panel with quest information
            completedQuestTitleText.text = quest.questData.title;
            completedQuestZoneText.text = $"{GetZoneCourse(quest.questData.zoneId)}\n<color=#888888><i>Zone: {quest.questData.zoneId}</i></color>";
            completedQuestDescriptionText.text = quest.questData.description;

            // Alter rewards of quest using below reward data
            RewardData rewards = quest.questData.rewards;
            completedGoldRewardText.text = rewards.gold.ToString();
            completedXpRewardText.text = rewards.experience.ToString();

            // Show the details panel and hide the "No completed quest selected" message
            hasSelectedCompletedQuest = true;
            selectedCompletedQuest = quest;

            noCompletedQuestSelectedText.SetActive(false);
            completedQuestSelectedObject.SetActive(true);
        }
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
        if (currentTab == "Active")
        {
            rightPanelCompleted.SetActive(false);
            rightPanelActive.SetActive(true);
        }
        else if (currentTab == "Completed")
        {
            rightPanelActive.SetActive(false);
            rightPanelCompleted.SetActive(true);
        }
    }

    public void RefreshQuestList()
    {
        foreach (Transform root in allQuestroots)
            {
                ClearChildren(root);
            }

        // Check each zone and populate the corresponding root with active quests for that zone
        PopulateQuestList(activeRoots, QuestManager.Instance.activeQuests);
        PopulateQuestList(completedRoots, QuestManager.Instance.completedQuests);

        // ======= ACTIVE SECTION =======
        // Check to see if a root has children, and if not, hide itself and title header by way of parent object
        bool hasAnyQuests = false;
        foreach (Transform root in activeRoots)
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

        // Upon completing a quest, if the player is currently viewing that quest's details, let's remove the details and show the "No active quest selected" message, since that quest is no longer active
        if (currentTab == "Active" && hasSelectedQuest && !QuestManager.Instance.activeQuests.Contains(selectedQuest))
        {
            hasSelectedQuest = false;
            selectedQuest = null;

            noActiveQuestSelectedText.SetActive(true);
            activeQuestSelectedObject.SetActive(false);
        }


        // ======= COMPLETED SECTION =======
        // Check to see if a root has children, and if not, hide itself and title header by way of parent object
        bool hasAnyCompletedQuests = false;
        foreach (Transform root in completedRoots)
        {
            bool hasQuests = root.childCount > 0;
            root.parent.gameObject.SetActive(hasQuests);
            if (hasQuests) hasAnyCompletedQuests = true;
        }

        // If none of the roots have quests, show a "No active quests" message in the UI
        noCompletedQuestsMessageLeft.SetActive(!hasAnyCompletedQuests);
        noCompletedQuestsMessageRight.SetActive(!hasAnyCompletedQuests);

        if (!hasAnyCompletedQuests) hasSelectedCompletedQuest = false;

        // However if there are active quests but no quest is selected, show a "No active quest selected" message in the details panel
        noCompletedQuestSelectedText.SetActive(hasAnyCompletedQuests && !hasSelectedCompletedQuest);
        completedQuestSelectedObject.SetActive(hasSelectedCompletedQuest);

        // Furthermore update the selected quest details if there is a selected quest, to reflect any changes in progress
        if (hasSelectedCompletedQuest) ShowQuestDetails(selectedCompletedQuest);
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

            if (selectedQuest == quest) entry.SetSelectedVisual(true);
            if (selectedCompletedQuest == quest) entry.SetSelectedVisual(true);
        }
    }

    public void OnQuestEntrySelection(QuestEntryUI selectedEntry)
    {
        // Deselect all other entries
        QuestEntryUI[] allEntries = FindObjectsByType<QuestEntryUI>(FindObjectsSortMode.None);
        foreach (QuestEntryUI entry in allEntries)
        {
            if (entry != selectedEntry) entry.SetSelectedVisual(false);
        }
        selectedEntry.SetSelectedVisual(true);
    }
}
