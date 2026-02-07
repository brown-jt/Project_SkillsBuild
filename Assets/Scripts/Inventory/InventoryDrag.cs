using UnityEngine;
using UnityEngine.UI;

public class InventoryDrag : MonoBehaviour
{
    public static InventoryDrag Instance;

    public Image dragIcon = null;
    private int draggedSlotIndex = -1;

    private void Awake()
    {
        Instance = this;
        dragIcon.enabled = false;
    }

    public void BeginDrag(int slotIndex)
    {
        draggedSlotIndex = slotIndex;
        var slot = InventoryManager.Instance.inventorySlots[slotIndex];

        if (slot.IsEmpty) return;

        // Maybe lower this transparency?
        dragIcon.sprite = slot.itemData.icon;
        dragIcon.enabled = true;
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
        dragIcon.sprite = null;
        dragIcon.enabled = false;
        draggedSlotIndex = -1;
    }
}
