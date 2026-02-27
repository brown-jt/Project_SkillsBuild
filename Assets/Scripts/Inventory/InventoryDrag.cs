using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryDrag : MonoBehaviour
{
    public static InventoryDrag Instance;

    public Image dragIcon = null;
    private int draggedSlotIndex = -1;

    public bool IsDragging => draggedSlotIndex != -1;

    private void Awake()
    {
        Instance = this;
        dragIcon.gameObject.SetActive(false);
    }

    public void BeginDrag(int slotIndex)
    {
        var slot = InventoryManager.Instance.inventorySlots[slotIndex];
        var slotUI = InventoryUI.Instance.slots[slotIndex];

        if (slot.IsEmpty) return;

        draggedSlotIndex = slotIndex;

        slotUI.SetIconOpacity(0f);

        if (dragIcon != null)
        {
            dragIcon.sprite = slot.itemData.icon;
            dragIcon.gameObject.SetActive(true);
            Cursor.visible = false;
        }
    }

    public void Drag(Vector2 position)
    {
        dragIcon.transform.position = position;
    }

    public void Drop(int targetSlotIndex)
    {
        if (draggedSlotIndex == -1) return;

        var slotA = InventoryManager.Instance.inventorySlots[draggedSlotIndex];
        var slotB = InventoryManager.Instance.inventorySlots[targetSlotIndex];
        
        (slotA.itemData, slotB.itemData) = (slotB.itemData, slotA.itemData);
        (slotA.quantity, slotB.quantity) = (slotB.quantity, slotA.quantity);

        InventoryUI.Instance.RefreshAll();
    }

    public void EndDrag()
    {
        if (draggedSlotIndex == -1)
            return;

        bool droppedOnUI = IsPointerInsideInventoryPanel();

        // If NOT dropped on UI we shall drop into world
        if (!droppedOnUI)
        {
            DropToWorld(draggedSlotIndex);
        }

        if (dragIcon != null)
        {
            dragIcon.sprite = null;
            dragIcon.gameObject.SetActive(false);
            Cursor.visible = true;
        }

        var originalSlotUI = InventoryUI.Instance.slots[draggedSlotIndex];
        originalSlotUI.SetIconOpacity(1f);

        draggedSlotIndex = -1;

        InventoryUI.Instance.RefreshAll();
    }

    private bool IsPointerInsideInventoryPanel()
    {
        RectTransform panelRect = InventoryUI.Instance.InventoryPanelRect;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        return RectTransformUtility.RectangleContainsScreenPoint(
            panelRect,
            mousePosition,
            null
        );
    }

    private void DropToWorld(int slotIndex)
    {
        var slot = InventoryManager.Instance.inventorySlots[slotIndex];

        if (slot.IsEmpty) return;
        if (slot.itemData.prefab == null) return;

        ItemData itemToDrop = slot.itemData;

        // Remove from inventory
        slot.itemData = null;
        slot.quantity = 0;

        Transform player = PlayerInputHandler.Instance.transform;

        // Small forward offset
        float forwardOffset = 0.2f;

        // Spawn slightly above ground so it falls naturally
        Vector3 origin = player.position + Vector3.up * 0.5f;
        Vector3 forward = player.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 spawnPosition = origin + forward * forwardOffset;

        GameObject droppedItem = Instantiate(itemToDrop.prefab, spawnPosition, Quaternion.identity);

        if (droppedItem.TryGetComponent<Rigidbody>(out var rb))
        {
            // Very small forward toss to simulate dropping without excessive force
            rb.linearVelocity = forward * 1.5f;
        }
    }
}
