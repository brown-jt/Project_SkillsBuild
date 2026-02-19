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

    private Image background;
    private Color normalColor;
    private Color hoverColor;

    public RectTransform slotRect;

    private void Awake()
    {
        slotRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        background = GetComponent<Image>();
        normalColor = background.color;
        
        // Make hover color slightly lighter
        hoverColor = new Color(
            Mathf.Min(normalColor.r + 0.2f, 1f),
            Mathf.Min(normalColor.g + 0.2f, 1f),
            Mathf.Min(normalColor.b + 0.2f, 1f),
            normalColor.a
        );
    }

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
            icon.SetNativeSize();
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.Show(InventoryManager.Instance.inventorySlots[slotIndex], slotRect);

        // For better feedback let's highlight hovered inventory slots
        background.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();

        // Reset colour change when no longer hovering that slot
        background.color = normalColor;
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
        TooltipUI.Instance.Show(InventoryManager.Instance.inventorySlots[slotIndex], slotRect);
    }

    public void SetIconOpacity(float alpha)
    {
        if (icon != null)
        {
            Color c = icon.color;
            c.a = alpha;
            icon.color = c;
        }
    }

    public void ResetVisuals()
    {
        // Reset background color
        background.color = normalColor;

        // Hide the icon tooltip if it's open
        TooltipUI.Instance.Hide();
    }
}
