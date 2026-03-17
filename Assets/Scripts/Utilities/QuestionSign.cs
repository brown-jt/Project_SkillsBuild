using TMPro;
using UnityEngine;

public class QuestionSign : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text2;

    public void SetText(string text)
    {
        if (text != null)
        {
            text1.text = text;
            text2.text = text;
        }
    }

    public void Clear()
    {
        text1.text = "Out Of Order";
        text2.text = "Out Of Order";
    }
}