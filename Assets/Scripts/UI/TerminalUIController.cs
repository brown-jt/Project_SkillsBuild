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
    public Button confirmButton;

    [Header("Feedback Panel Elements")]
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI feedbackText;
    public Button continueButton;

    [Header("End Panel Elements")]
    public TextMeshProUGUI passText;
    public TextMeshProUGUI scoreText;
    public Button retryButton;
    public Button exitButton;

    private System.Action<List<int>> onAnswerSelected;

    private List<int> selectedIndexes;
    private int requiredSelections;

    public void QuizStart(string title, string subtitle)
    {
        questionContents.SetActive(false);
        feedbackContents.SetActive(false);
        endContents.SetActive(false);
        startContents.SetActive(true);
        mainText.text = title;
        subText.text = subtitle;
    }

    public void ShowQuestion(string prompt, List<string> options, int numSelections, System.Action<List<int>> callback)
    {
        SetButtonsInteractable(true);

        startContents.SetActive(false);
        feedbackContents.SetActive(false);
        endContents.SetActive(false);
        questionContents.SetActive(true);

        questionText.text = prompt;
        onAnswerSelected = callback;
        requiredSelections = numSelections;

        // Reset selections for each new question and disable confirm button until selection(s) are made
        selectedIndexes = new List<int>();
        confirmButton.interactable = false;

        for (int i = 0; i < optionButtons.Count; i++)
        {
            if (i < options.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TMP_Text>().text = options[i];

                int index = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => SelectAnswer(index));

                optionButtons[i].interactable = true; // Ensure interactable at start
                optionButtons[i].image.color = Color.white; // Reset color
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }

        // Hooking up confirm button
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            SetButtonsInteractable(false);
            ConfirmAnswer();
        });
    }

    private void SelectAnswer(int index)
    {
        // Deselect if already selected
        if (selectedIndexes.Contains(index))
        {
            selectedIndexes.Remove(index);
            SetButtonSelectedVisual(index, false);
        }
        else
        {
            // Single selection mode
            if (requiredSelections == 1)
            {
                if (selectedIndexes.Count > 0)
                {
                    int previousIndex = selectedIndexes[0];
                    SetButtonSelectedVisual(previousIndex, false);
                    selectedIndexes.Clear();
                }

                selectedIndexes.Add(index);
                SetButtonSelectedVisual(index, true);
            }
            else
            {
                // Multi-selection mode
                if (selectedIndexes.Count < requiredSelections)
                {
                    selectedIndexes.Add(index);
                    SetButtonSelectedVisual(index, true);
                }
            }
        }

        // Check to see if we have the required number of selections to enable the confirm button
        confirmButton.interactable = selectedIndexes.Count == requiredSelections;
    }

    private void ConfirmAnswer()
    {
        SetButtonsInteractable(false);
        confirmButton.interactable = false;

        onAnswerSelected?.Invoke(new List<int>(selectedIndexes));
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

    private void SetButtonSelectedVisual(int index, bool selected)
    {
        var colors = optionButtons[index].colors;

        if (selected)
            optionButtons[index].image.color = Color.green;
        else
            optionButtons[index].image.color = Color.white;
    }
}
