using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Questions/QuestionSetData")]
public class QuestionSetData : ScriptableObject
{
    public List<QuestionData> questions;

    [Range(0f, 1f)]
    [Tooltip("Percentage of correct answers required to pass e.g., (0.8 = 80%)")]
    public float passPercentage = 0.8f;
}
