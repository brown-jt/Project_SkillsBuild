using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public GameObject panel;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(InventorySlot slot)
    {
        if (slot.IsEmpty) return;

        title.text = slot.itemData.itemName;
        description.text = slot.itemData.description;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
