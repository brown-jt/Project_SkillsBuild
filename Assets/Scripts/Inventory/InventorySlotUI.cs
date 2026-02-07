using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    public int slotIndex;

    public void Refresh()
    {
        var slot = InventoryManager.Instance.inventorySlots[slotIndex];

        if (slot.IsEmpty)
        {
            icon.sprite = null;
            icon.enabled = false;
            quantityText.text = "";
        }
        else
        {
            icon.sprite = slot.itemData.icon;
            icon.enabled = true;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.Show(InventoryManager.Instance.inventorySlots[slotIndex]);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryDrag.Instance.BeginDrag(slotIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryDrag.Instance.Drag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryDrag.Instance.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryDrag.Instance.Drop(slotIndex);
    }
}
