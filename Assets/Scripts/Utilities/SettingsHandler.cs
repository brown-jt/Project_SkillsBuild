using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SettingsHandler : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityText;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private TextMeshProUGUI masterText;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;

    private void Start()
    {
        // Listen for value changes
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Apply initial value
        OnSensitivityChanged(FirstPersonController.Instance.MouseSensitivity);
        OnMasterVolumeChanged(AudioManager.Instance.masterVolume);
        OnMusicVolumeChanged(AudioManager.Instance.musicVolume);
        OnSFXVolumeChanged(AudioManager.Instance.sfxVolume);
    }

    private void OnSensitivityChanged(float value)
    {
        FirstPersonController.Instance.SetMouseSensitivity(value);
        sensitivitySlider.value = value;
        sensitivityText.text = value.ToString();
    }

    private void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance.masterVolume = value;
        masterSlider.value = value;
        masterText.text = value.ToString();
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.musicVolume = value;
        musicSlider.value = value;
        musicText.text = value.ToString();
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.sfxVolume = value;
        sfxSlider.value = value;
        sfxText.text = value.ToString();
    }
}

