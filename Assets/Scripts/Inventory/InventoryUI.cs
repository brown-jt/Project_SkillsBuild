using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public GameObject inventoryPanel;
    [SerializeField] private InputActionReference inventoryAction;
    [SerializeField] private InputActionReference cancelAction;

    public InventorySlotUI[] slots;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].slotIndex = i;
        }
    }

    private void OnEnable()
    {
        inventoryAction.action.Enable();
        inventoryAction.action.performed += ToggleInventory;

        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        inventoryAction.action.performed -= ToggleInventory;
        inventoryAction.action.Disable();

        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    private void ToggleInventory(InputAction.CallbackContext ctx)
    {
        bool isOpen = inventoryPanel.activeSelf;

        if (isOpen) CloseInventory();
        else OpenInventory();
    }

    private void OpenInventory()
    {
        PlayerInputHandler.Instance.DisablePlayerInput();
        inventoryPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseInventory()
    {
        ResetVisualsAll();
        PlayerInputHandler.Instance.EnablePlayerInput();
        inventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (inventoryPanel.activeSelf) CloseInventory();
    }

    private void Start()
    {
        inventoryPanel.SetActive(false);
        RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (InventorySlotUI slot in slots)
        {
            slot.Refresh();
        }
    }

    public void ResetVisualsAll()
    {
        foreach (InventorySlotUI slot in slots)
        {
            slot.ResetVisuals();
        }
    }
}
