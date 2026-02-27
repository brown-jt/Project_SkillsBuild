using UnityEngine;
public class QuestQuizTrigger : MonoBehaviour
{
    public void Passed(string quizId)
    {
        QuestEvents.QuizPassed(quizId);
    }
}
