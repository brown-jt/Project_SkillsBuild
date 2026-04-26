using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    // SFX entry to allow inspector assignment
    [System.Serializable]
    public class AudioEntry
    {
        public string key;
        public AudioClip clip;
    }

    // Singleton instance
    public static AudioManager Instance { get; private set; }

    public Transform playerTransform;

    // To handle inspector assignment of SFX clips and convert to dictionary for runtime use
    [SerializeField] private List<AudioEntry> audioList;
    private Dictionary<string, AudioClip> audioLibrary;

    // Volumes
    public float masterVolume = 100f;
    public float sfxVolume = 100f;
    public float musicVolume = 100f;

    // Sources
    [SerializeField] private AudioSource uiSource; // for 2D UI sounds
    [SerializeField] private AudioSource musicSource; // for music

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

        // Convert SFX list to dictionary for efficient lookup
        audioLibrary = new Dictionary<string, AudioClip>();

        foreach (var entry in audioList)
        {
            audioLibrary[entry.key] = entry.clip;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        float volume = ToVolume(masterVolume) * ToVolume(sfxVolume);
        uiSource.PlayOneShot(clip, volume);
    }

    private void PlaySoundAtPosition(AudioClip clip, Vector3 position)
    {
        float volume = ToVolume(masterVolume) * ToVolume(sfxVolume);
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlaySFX(string key, Transform position = null)
    {
        if (audioLibrary.TryGetValue(key, out AudioClip clip))
        {
            if (!position)
            {
                PlaySound(clip);
            }
            else
            {
                PlaySoundAtPosition(clip, position.position);
            }
        }
        else
        {
            Debug.LogWarning($"Audio key not found: {key}");
        }
    }

    public void PlayMusic(string key)
    {
        if (audioLibrary.TryGetValue(key, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.volume = GetMusicVolume();
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Audio key not found: {key}");
        }
    }

    public void SetMusicVolume()
    {
        musicSource.volume = GetMusicVolume();
    }

    public float GetMusicVolume()
    {
        return ToVolume(masterVolume) * ToVolume(musicVolume);
    }

    private float ToVolume(float percent)
    {
        float linear = Mathf.Clamp01(percent / 100f);

        return linear * linear; // Squaring for exponential falloff
    }

    public void FadeToNewMusic(string key, float fadeDuration = 0.5f)
    {
        if (audioLibrary.TryGetValue(key, out AudioClip newClip))
        {
            StartCoroutine(FadeMusicCoroutine(newClip, fadeDuration));
        }
        else
        {
            Debug.LogWarning($"Audio key not found: {key}");
        }
    }

    private IEnumerator FadeMusicCoroutine(AudioClip newClip, float fadeDuration)
    {
        float volume = GetMusicVolume();

        // Fade out current music
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(volume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in new music
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, volume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = volume; // Ensure final volume is set
    }
}
