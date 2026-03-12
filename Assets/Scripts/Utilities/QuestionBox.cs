using TMPro;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialise(QuestionData questionData)
    {
        questionText.text = questionData.question;
    }
}
