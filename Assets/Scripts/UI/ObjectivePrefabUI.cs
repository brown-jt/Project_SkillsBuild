using TMPro;
using UnityEngine;

public class ObjectivePrefabUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI progressText;

    private readonly string inProgressColourHex = "#FFD166";
    private readonly string completedColourHex = "#06D6A0";

    public void Bind(ObjectiveProgress objProgress)
    {
        objectiveText.text = objProgress.data.taskText;

        string colourToUse = objProgress.IsComplete ? completedColourHex : inProgressColourHex;
        progressText.text = $"<color={colourToUse}>({objProgress.currentAmount}/{objProgress.data.requiredAmount})</color>";
    }
}
