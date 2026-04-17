using TMPro;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public TextMeshProUGUI paintingText;
    public GameObject staticPassText;
    public GameObject unstartedText;
    public GameObject terminalsParent;
    public GameObject button;

    private void Awake()
    {
        paintingText.text = "";
        unstartedText.SetActive(true);
        terminalsParent.SetActive(false);
        button.SetActive(false);
    }

    public void DisplayQuestion(QuestionData data)
    {
        paintingText.text = data.question;
        unstartedText.SetActive(false);
        terminalsParent.SetActive(true);
        button.SetActive(true);
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
        button.SetActive(false);
    }

    public void DisplayEnd(string message)
    {
        paintingText.text = message;
        button.SetActive(false);
    }

    public void DisplayCompleted()
    {
        paintingText.text = "";
        unstartedText.SetActive(false);
        terminalsParent.SetActive(false);
        button.SetActive(false);
        staticPassText.SetActive(true);
    }

    public void ClearText()
    {
        paintingText = null;
    }

}
