using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // SFX entry to allow inspector assignment
    [System.Serializable]
    public class SFXEntry
    {
        public string key;
        public AudioClip clip;
    }

    // Singleton instance
    public static AudioManager Instance { get; private set; }

    public Transform playerTransform;

    // To handle inspector assignment of SFX clips and convert to dictionary for runtime use
    [SerializeField] private List<SFXEntry> sfxList;
    private Dictionary<string, AudioClip> sfxLibrary;

    // Volumes
    public float masterVolume = 100f;
    public float sfxVolume = 100f;
    public float musicVolume = 100f;

    // Source
    [SerializeField] private AudioSource uiSource; // for 2D UI sounds

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
        sfxLibrary = new Dictionary<string, AudioClip>();

        foreach (var entry in sfxList)
        {
            sfxLibrary[entry.key] = entry.clip;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        float volume = (masterVolume / 100f) * (sfxVolume / 100f);
        uiSource.PlayOneShot(clip, volume);
    }

    private void PlaySoundAtPosition(AudioClip clip, Vector3 position)
    {
        float volume = (masterVolume / 100f) * (sfxVolume / 100f);
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlaySFX(string key, Transform position = null)
    {
        if (sfxLibrary.TryGetValue(key, out AudioClip clip))
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
            Debug.LogWarning($"SFX key not found: {key}");
        }
    }
}
