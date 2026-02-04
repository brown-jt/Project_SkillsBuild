using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public static DialogSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialog(DialogData data)
    {
        foreach (var line in data.lines)
        {
            Debug.Log(line);
        }
    }
}
