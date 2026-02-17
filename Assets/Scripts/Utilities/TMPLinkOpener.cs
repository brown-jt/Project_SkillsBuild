using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMPLinkOpener : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI descriptionWithLink;

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            descriptionWithLink,
            eventData.position,
            eventData.pressEventCamera
        );

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = descriptionWithLink.textInfo.linkInfo[linkIndex];
            string url = linkInfo.GetLinkID();

            Application.OpenURL(url);
        }
    }
}
