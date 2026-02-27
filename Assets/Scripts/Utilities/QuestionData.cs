using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Questions/QuestionData")]
public class QuestionData : ScriptableObject
{
    [TextArea] public string question;
    public List<string> answers;

    [Tooltip("How many answers can be selected - (1 = single | >1 = multi)")]
    public int maxSelections = 1;

    [Tooltip("Index of the correct answer(s) - for single selection, only the first index is considered")]
    public List<int> correctAnswerIndices;

    [TextArea] public string correctMessage;
    [TextArea] public string incorrectMessage;
}
