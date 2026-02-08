using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public RectTransform panel;

    private RectTransform canvasRect;
    public Vector2 padding = new Vector2(12f, 12f);

    private void Awake()
    {
        // Ensure singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        panel.gameObject.SetActive(false);
    }

    public void Show(InventorySlot slot, RectTransform slotRect)
    {
        if (slot.IsEmpty) return;

        title.text = slot.itemData.itemName;
        description.text = slot.itemData.description;

        panel.gameObject.SetActive(true);
        PositionTooltipRelativeToSlot(slotRect);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }

    private void PositionTooltipRelativeToSlot(RectTransform slotRect)
    {
        // Get world corners of the slot
        Vector3[] corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);

        Vector2 bottomLeft = corners[0];
        Vector2 topRight = corners[2];

        Vector2 tooltipSize = panel.sizeDelta;
        Vector2 pos = new Vector2(topRight.x, bottomLeft.y); // bottom-right of slot

        bool offRight = pos.x + tooltipSize.x > Screen.width;
        bool offBottom = pos.y - tooltipSize.y < 0;

        if (offRight)
            pos.x = bottomLeft.x - tooltipSize.x;

        if (offBottom)
            pos.y = topRight.y + tooltipSize.y;

        panel.position = pos;
    }
}
