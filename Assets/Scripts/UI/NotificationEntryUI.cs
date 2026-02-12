using TMPro;
using UnityEngine;
using System.Collections;

public class NotificationEntryUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI messageText;
    public CanvasGroup canvasGroup; // Used for a super simple fade-in-out currently

    [Header("Fade Settings")]
    public float fadeInDuration = 0.25f;
    public float fadeOutDuration = 0.25f;
    public float timeVisible = 2.0f;

    private Coroutine fadeCoroutine;

    public void SetMessage(string message)
    {
        messageText.text = message;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInOutRoutine());
    }

    IEnumerator FadeInOutRoutine()
    {
        // Fade in
        yield return Fade(0f, 1f, true);

        // Stay visible
        yield return new WaitForSeconds(timeVisible);

        // Fade out
        yield return Fade(1f, 0f, false);

        Destroy(gameObject);
    }

    IEnumerator Fade(float start, float end, bool fadeIn)
    {
        float t = 0f;
        float fadeDuration = fadeIn ? fadeInDuration : fadeOutDuration;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = end;
    }
}
