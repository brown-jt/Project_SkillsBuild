using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;

    private void Start()
    {
        // Listen for value changes
        slider.onValueChanged.AddListener(OnSensitivityChanged);

        // Apply initial value
        OnSensitivityChanged(FirstPersonController.Instance.MouseSensitivity);
    }

    private void OnSensitivityChanged(float value)
    {
        FirstPersonController.Instance.SetMouseSensitivity(value);
        slider.value = value;
        valueText.text = value.ToString();
    }
}