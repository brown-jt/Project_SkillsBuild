using TMPro;
using UnityEngine;

public class QuestionSign : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private TextMeshProUGUI answerText2;

    public void SetAnswer(string answer)
    {
        if (answerText != null)
        {
            answerText.text = answer;
            answerText2.text = answer;
        }
    }

    public void Clear()
    {
        if (answerText != null)
        { 
            answerText.text = "Out Of Order";
            answerText2.text = "Out Of Order";
        }
    }
}