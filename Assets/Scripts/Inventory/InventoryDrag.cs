using UnityEngine;
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

        slotUI.SetIconOpacity(0.5f);

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
        if (dragIcon != null)
        {
            dragIcon.sprite = null;
            dragIcon.gameObject.SetActive(false);
            Cursor.visible = true;
        }

        if (draggedSlotIndex != -1)
        {
            var originalSlotUI = InventoryUI.Instance.slots[draggedSlotIndex];
            originalSlotUI.SetIconOpacity(1f);
        }

        draggedSlotIndex = -1;
    }
}
