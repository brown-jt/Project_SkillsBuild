using System.Linq;
using TMPro;
using UnityEngine;

public class QuestionBox : HoldableItem
{
    [Header("Question Settings")]
    public TextMeshProUGUI questionText;

    private QuestionData questionData;
    private QuestionSign[] questionSigns;

    public void Initialise(QuestionData questionData)
    {
        this.questionData = questionData;
        questionText.text = questionData.question;

        // Find and cache all question signs in the scene and sorting them by their name as ending _1 -> _5
        questionSigns = FindObjectsByType<QuestionSign>(FindObjectsSortMode.None)
            .OrderBy(sign => int.Parse(sign.name.Split('_').Last()))
            .ToArray();
    }

    protected override void OnPickedUp()
    {
        // Set the answer on all question signs when picked up
        for (int i = 0; i < questionData.answers.Count; i++)
        {
            questionSigns[i].SetAnswer(questionData.answers[i]);
        }
    }

    protected override void OnDropped()
    {
        // Clear the answer on all question signs when dropped
         if (questionSigns != null)
         {
             foreach (var sign in questionSigns)
             {
                 sign.Clear();
             }
        }
    }

}
