using TMPro;
using UnityEngine;

public class WarehouseQuizManager : MonoBehaviour
{
    [Header("Quiz Settings")]
    public QuestionSetData questionSet;

    [Header("Default Minigame Rewards")]
    [SerializeField] private int goldReward = 100;
    [SerializeField] private float experienceReward = 50.0f;

    [Header("References")]
    [SerializeField] private GameObject timerUI;
    [SerializeField] private TextMeshProUGUI timerText;

    private float timer = 0.0f;
    private bool quizActive = false;

    private void Start()
    {
        timerUI.SetActive(false);
    }

    private void Update()
    {
        if (quizActive)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        float milliseconds = (timer * 1000) % 1000;

        if (minutes > 0)
            timerText.text = $"{minutes}:{seconds:00}:{milliseconds:000}";
        else
            timerText.text = $"{seconds}:{milliseconds:000}";
    }

    public void StartQuiz()
    {
        timerUI.SetActive(true);
        quizActive = true;
        timer = 0.0f;
    }

    public void EndQuiz()
    {
        quizActive = false;
        // TODO: Determine rewards based on performance (e.g., time taken, correct answers)
    }
}
