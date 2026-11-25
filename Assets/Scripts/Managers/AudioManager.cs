using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    // Global access point. Anyone can read it, only this class can set it.
    public static AudioManager Instance { get; private set; }

    [SerializeField, Tooltip("Background music clip")]
    private AudioClip backgroundMusic;

    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    private AudioSource audioSource;

    // Read-only access for other scripts (e.g. the Demo slider).
    public float Volume => volume;

    private void Awake()
    {
        // Duplicate guard: if another instance already exists, remove this one.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; // stop here so the duplicate never starts music
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
    }

    // The one place that changes the volume, so the stored value
    // and the AudioSource can never get out of sync.
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}