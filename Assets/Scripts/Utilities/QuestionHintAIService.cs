using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class QuestionHintAIService : MonoBehaviour
{
    public static QuestionHintAIService Instance;

    private const int MIN_HINTS = 2;

    private void Awake()
    {
        // Ensure singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GetQuestionHint(string questId, int questionIndex, string prompt, Action<string> onComplete)
    {
        var db = DatabaseManager.Instance;
        List<DatabaseManager.QuestionHintRow> hints = db.GetAIResponses(questId, questionIndex);

        // If we have hints we are going to return one via helper function SelectBestHint
        if (hints.Count > 0)
        {
            var selectedHint = SelectBestHint(hints);

            db.UpdateAIResponseUsage(selectedHint.id, false);

            onComplete?.Invoke(selectedHint.response);

            // Maintain the hint pool in the background after sending off a response
            StartCoroutine(MaintainHintPool(questId, questionIndex, prompt, hints));
        }
        else
        {
            // If we have no hints (should never happen but as failsafe) we must attempt to generate and return one immediately
            GroqManager.Instance.GenerateResponse(prompt, (res) =>
            {
                db.InsertAIResponse(questId, questionIndex, res);
                onComplete?.Invoke(res);

                // Start building pool in the background after generating this first response
                StartCoroutine(MaintainHintPool(questId, questionIndex, prompt, null));
            });
        }
    }

    private DatabaseManager.QuestionHintRow SelectBestHint(List<DatabaseManager.QuestionHintRow> hints)
    {
        // Returns the first created hint
        return hints
            .OrderBy(h => string.IsNullOrEmpty(h.created_at) ? DateTime.MinValue : DateTime.Parse(h.created_at))
            .First();
    }

    private IEnumerator MaintainHintPool(string questId, int questionIndex, string prompt, List<DatabaseManager.QuestionHintRow> currentHints)
    {
        // Ensure we wait a frame after sending completed response before starting coroutine
        yield return null;

        var db = DatabaseManager.Instance;

        var hints = currentHints ?? db.GetAIResponses(questId, questionIndex);

        // Ensure minimum hints are met at least
        if (hints.Count < MIN_HINTS)
        {
            int needed = MIN_HINTS - hints.Count;

            for (int i = 0; i < needed; i++)
            {
                yield return GenerateAndStoreHint(questId, questionIndex, prompt);
            }
        }
    }

    private IEnumerator GenerateAndStoreHint(string questId, int questionIndex, string prompt)
    {
        bool stored = false;

        GroqManager.Instance.GenerateResponse(prompt, (res) =>
        {
            DatabaseManager.Instance.InsertAIResponse(questId, questionIndex, res);
            stored = true;
        });

        while (!stored) yield return null;
    }
}
