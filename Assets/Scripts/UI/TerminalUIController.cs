using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TerminalUIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject questionContents;
    public GameObject feedbackContents;

    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public List<Button> optionButtons;

    [Header("Feedback")]
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI feedbackText;
    public Button continueButton;

    private System.Action<int> onAnswerSelected;

    public void ShowQuestion(string prompt, List<string> options, System.Action<int> callback)
    {
        feedbackContents.SetActive(false);
        questionContents.SetActive(true);

        questionText.text = prompt;
        onAnswerSelected = callback;

        for (int i = 0; i < optionButtons.Count; i++)
        {
            if (i < options.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TMP_Text>().text = options[i];
                int index = i;
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
        SetButtonsInteractable(false);
        onAnswerSelected?.Invoke(index);
    }

    public void ShowFeedback(bool correct, string message, System.Action onContinue)
    {
        questionContents.SetActive(false);
        feedbackContents.SetActive(true);

        answerText.text = correct ? "Correct!" : "Incorrect!";
        feedbackText.text = message;

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            SetButtonsInteractable(true);
            onContinue?.Invoke();
        });
    }

    public void ShowFinalResult(string result)
    {
        questionContents.SetActive(false);
        feedbackContents.SetActive(true);

        answerText.text = result;

        SetButtonsInteractable(false);
    }

    private void SetButtonsInteractable(bool state)
    {
        foreach (var btn in optionButtons)
            btn.interactable = state;
    }
}
