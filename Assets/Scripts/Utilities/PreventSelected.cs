using UnityEngine;
using UnityEngine.EventSystems;

public class PreventSelected : MonoBehaviour
{
    public void ClearSelectionOnClick()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
