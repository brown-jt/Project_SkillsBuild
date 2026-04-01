using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private Slider slider;
    [SerializeField] private ZoneId zoneId;

    private int currentExperience;
    private int maxExperience;

    private void Start()
    {
        SetMaxExperience();
    }

    private void SetMaxExperience()
    {
        Dictionary<string, QuestData> quests = DatabaseManager.Instance.QuestsDict;

        foreach (QuestData quest in quests.Values)
        {
            if (quest.zoneId == zoneId)
            {
                maxExperience += quest.rewards.experience;
            }
        }

        slider.maxValue = maxExperience;
        slider.value = 0; // Initialize the slider value to 0 at the start
        percentageText.text = FormatCurrentExperienceAsPercentage();
    }

    public void AddExperience(int experience)
    {
        currentExperience += experience;
        slider.value = currentExperience;
        percentageText.text = FormatCurrentExperienceAsPercentage();
    }

    private string FormatCurrentExperienceAsPercentage()
    {
        float percentage = (float)currentExperience / maxExperience * 100;
        return $"{percentage:0.##}%";
    }
}
