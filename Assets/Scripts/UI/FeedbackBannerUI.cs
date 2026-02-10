using System.Collections;
using TMPro;
using UnityEngine;

public class FeedbackBannerUI : MonoBehaviour
{
    public static FeedbackBannerUI Instance;

    [Header("References")]
    public GameObject banner;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI bonusText;
    public TextMeshProUGUI subText;

    [Header("Fade Settings")]
    public CanvasGroup canvasGroup; // Used for a super simple fade-in-out currently
    public float fadeInDuration = 2.0f;
    public float fadeOutDuration = 1.0f;
    public float timeVisibile = 2.0f;

    Coroutine fadeCoroutine;


    private void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0;
        banner.SetActive(false);
    }

    public void ShowBanner(string mainTextVal, string subTextVal, string bonusTextVal = "")
    {
        mainText.text = mainTextVal;
        bonusText.text = bonusTextVal;
        subText.text = subTextVal;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInOutRoutine());
    }

    IEnumerator FadeInOutRoutine()
    {
        banner.SetActive(true);

        // Fade in
        yield return Fade(0f, 1f, true);

        // Stay visible
        yield return new WaitForSeconds(timeVisibile);

        // Fade out
        yield return Fade(1f, 0f, false);

        banner.SetActive(false);
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
