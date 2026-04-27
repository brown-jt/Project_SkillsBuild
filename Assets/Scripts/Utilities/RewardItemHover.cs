using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemData itemData;
    public Image icon;
    public RectTransform area;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.ShowRewardItemDetails(itemData, area);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }

    public void SetItemData(ItemData itemData)
    {
        this.itemData = itemData;
        icon.sprite = itemData.icon;
    }
}
