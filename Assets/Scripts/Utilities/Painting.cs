using TMPro;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI questionNumberText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI passRateText;
    public GameObject activeText;
    public GameObject staticPassText;
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
        unstartedText.SetActive(false);
        terminalsParent.SetActive(false);
        button.SetActive(false);
        staticPassText.SetActive(true);
        paintingManager.ToggleSpotLights(true);
    }

    public void ClearExtraText()
    {
        progressText.text = "";
        questionNumberText.text = "";
        passRateText.text = "";
    }

    public void SetQuestionNumber(int number, int total)
    {
        questionNumberText.text = $"Question {number} of {total}"; 
    }

    public void SetProgress(int correct, int total)
    {
        progressText.text = $"Correct Answers: {correct}/{total} ({(float)correct / total * 100:F2}%)";
    }

    public void SetPassRate(float passPercentage)
    {
        if (passPercentage > 0) passRateText.text = $"Pass Rate: {passPercentage * 100:F2}%";
        else passRateText.text = "Practice Only. No pass rate. Finished upon completion.";
    }
}
