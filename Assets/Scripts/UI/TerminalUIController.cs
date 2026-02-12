using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TerminalUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public List<Button> optionButtons;

    private System.Action<int> onAnswerSelected;

    public void ShowQuestion(string prompt, List<string> options, System.Action<int> callback)
    {
        questionText.text = prompt;
        onAnswerSelected = callback;

        for (int i = 0; i < optionButtons.Count; i++)
        {
            if (i < options.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TMP_Text>().text = options[i];
                int index = i; // local copy for lambda
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => SelectAnswer(index));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void SelectAnswer(int index)
    {
        onAnswerSelected?.Invoke(index);
    }
}
