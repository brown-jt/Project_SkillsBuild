using TMPro;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public TextMeshProUGUI paintingText;
    public GameObject staticPassText;

    public void DisplayQuestion(QuestionData data)
    {
        paintingText.text = data.question;
    }
    public void DisplayResult(bool correct, QuestionData data)
    {
        if (correct)
        {
            paintingText.text = "<color=green><size=125%>Correct!</color></size>\n" + data.correctMessage;
        }
        else
        {
            paintingText.text = "<color=red><size=125%>Incorrect!</color></size>\n" + data.incorrectMessage;
        }
    }

    public void DisplayEnd(string message)
    {
        paintingText.text = message;
    }

    public void DisplayCompleted()
    {
        paintingText.text = "";
        staticPassText.SetActive(true);
    }

    public void ClearText()
    {
        paintingText = null;
    }

}
