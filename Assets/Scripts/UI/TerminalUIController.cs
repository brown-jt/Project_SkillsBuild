using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TerminalUIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject startContents;
    public GameObject questionContents;
    public GameObject feedbackContents;
    public GameObject endContents;

    [Header("Start Panel Elements")]
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI subText;

    [Header("Question Panel Elements")]
    public TextMeshProUGUI questionText;
    public List<Button> optionButtons;

    [Header("Feedback Panel Elements")]
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI feedbackText;
    public Button continueButton;

    [Header("End Panel Elements")]
    public TextMeshProUGUI passText;
    public TextMeshProUGUI scoreText;
    public Button retryButton;
    public Button exitButton;

    private System.Action<int> onAnswerSelected;

    public void QuizStart(string title, string subtitle)
    {
        questionContents.SetActive(false);
        feedbackContents.SetActive(false);
        endContents.SetActive(false);
        startContents.SetActive(true);
        mainText.text = title;
        subText.text = subtitle;
    }

    public void ShowQuestion(string prompt, List<string> options, System.Action<int> callback)
    {
        SetButtonsInteractable(true);

        startContents.SetActive(false);
        feedbackContents.SetActive(false);
        endContents.SetActive(false);
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
        endContents.SetActive(false);
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

    public void ShowFinalResult(bool passed, string message)
    {
        questionContents.SetActive(false);
        feedbackContents.SetActive(false);
        endContents.SetActive(true);

        passText.text = passed ? "Passed" : "Failed";
        scoreText.text = message;

        retryButton.gameObject.SetActive(!passed);
    }

    private void SetButtonsInteractable(bool state)
    {
        foreach (var btn in optionButtons)
            btn.interactable = state;
    }
}
