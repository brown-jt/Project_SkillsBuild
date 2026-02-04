using UnityEngine;
using UnityEngine.InputSystem;

public class QuestJournalUI : MonoBehaviour
{
    public GameObject questJournalPanel;
    [SerializeField] private InputActionReference questJournalAction;
    [SerializeField] private InputActionReference cancelAction;

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
}
