using TMPro;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI questionNumberText;
    public TextMeshProUGUI passRateText;
    public GameObject activeText;
    public GameObject staticPassText;
    public TextMeshProUGUI staticPassTextName;
    public GameObject unstartedText;
    public GameObject terminalsParent;
    public GameObject button;
    public PaintingManager paintingManager;

    private void Awake()
    {
        mainText.text = "";
        unstartedText.SetActive(true);
        terminalsParent.SetActive(false);
        button.SetActive(false);
    }

    public void DisplayQuestion(QuestionData data)
    {
        mainText.text = data.question;
        unstartedText.SetActive(false);
        terminalsParent.SetActive(true);
        button.SetActive(true);
        button.GetComponent<ConfirmAnswerButton>().SetInteractableName("Confirm Selection");
    }
    public void DisplayResult(bool correct, QuestionData data)
    {
        if (correct)
        {
            mainText.text = "<color=green><size=125%>Correct!</color></size>\n" + data.correctMessage;
        }
        else
        {
            mainText.text = "<color=red><size=125%>Incorrect!</color></size>\n" + data.incorrectMessage;
        }
        button.SetActive(false);
    }

    public void DisplayEnd(string message)
    {
        mainText.text = message;
        button.SetActive(false);
    }

    public void DisplayCompleted()
    {
        mainText.text = "";
        SetQuestionSetName(paintingManager.relevantQuest.questionSet.moduleName);
        unstartedText.SetActive(false);
        terminalsParent.SetActive(false);
        button.SetActive(false);
        staticPassText.SetActive(true);
        paintingManager.ToggleSpotLights(true);
    }

    public void ClearExtraText()
    {
        questionNumberText.text = "";
        passRateText.text = "";
    }

    public void SetQuestionNumber(int number, int total)
    {
        questionNumberText.text = $"Question {number} of {total}"; 
    }

    public void SetPassRate(float passPercentage)
    {
        if (passPercentage > 0) passRateText.text = $"Pass Rate: {passPercentage * 100:F2}%";
        else passRateText.text = "Practice Only. Always pass.";
    }

    private void SetQuestionSetName(string name)
    {
        staticPassTextName.text = $"{name}\nPuzzle completed";
    }

    public void ResetButton()
    {
        button.SetActive(true);
        button.GetComponent<ConfirmAnswerButton>().SetInteractableName("Try Again");
    }
}
