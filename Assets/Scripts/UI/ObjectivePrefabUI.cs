using TMPro;
using UnityEngine;

public class ObjectivePrefabUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI progressText;

    public void Bind(ObjectiveProgress objProgress)
    {
        objectiveText.text = objProgress.data.taskText;
        progressText.text = $"({objProgress.currentAmount}/{objProgress.data.requiredAmount})";
    }
}
