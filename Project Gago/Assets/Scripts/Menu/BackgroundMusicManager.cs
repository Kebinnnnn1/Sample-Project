using UnityEngine;
using System.Collections;

/// <summary>
/// Manages background music playback with fade in/out effects.
/// Supports two songs with smooth transitions between them.
/// </summary>
public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;

    [Header("Music Clips")]
    [Tooltip("First background music track")]
    [SerializeField] private AudioClip song1;
    [Tooltip("Second background music track")]
    [SerializeField] private AudioClip song2;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private float crossfadeDuration = 3f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loopMusic = true;

    private AudioSource currentSource;
    private AudioSource nextSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Create audio sources if not assigned
        if (audioSource1 == null)
        {
            audioSource1 = gameObject.AddComponent<AudioSource>();
            audioSource1.playOnAwake = false;
            audioSource1.loop = loopMusic;
        }

        if (audioSource2 == null)
        {
            audioSource2 = gameObject.AddComponent<AudioSource>();
            audioSource2.playOnAwake = false;
            audioSource2.loop = loopMusic;
        }

        currentSource = audioSource1;
        nextSource = audioSource2;
    }

    private void Start()
    {
        if (playOnStart && song1 != null)
        {
            PlaySong1WithFadeIn();
        }
    }

    /// <summary>
    /// Plays song 1 with a fade in effect.
    /// </summary>
    public void PlaySong1WithFadeIn()
    {
        if (song1 == null)
        {
            Debug.LogWarning("BackgroundMusicManager: Song 1 is not assigned!");
            return;
        }

        StopAllFades();
        currentSource.clip = song1;
        currentSource.loop = loopMusic;
        fadeCoroutine = StartCoroutine(FadeIn(currentSource, fadeInDuration));
    }

    /// <summary>
    /// Plays song 2 with a fade in effect.
    /// </summary>
    public void PlaySong2WithFadeIn()
    {
        if (song2 == null)
        {
            Debug.LogWarning("BackgroundMusicManager: Song 2 is not assigned!");
            return;
        }

        StopAllFades();
        currentSource.clip = song2;
        currentSource.loop = loopMusic;
        fadeCoroutine = StartCoroutine(FadeIn(currentSource, fadeInDuration));
    }

    /// <summary>
    /// Crossfades from the current song to song 1.
    /// </summary>
    public void CrossfadeToSong1()
    {
        if (song1 == null)
        {
            Debug.LogWarning("BackgroundMusicManager: Song 1 is not assigned!");
            return;
        }

        StopAllFades();
        fadeCoroutine = StartCoroutine(Crossfade(song1));
    }

    /// <summary>
    /// Crossfades from the current song to song 2.
    /// </summary>
    public void CrossfadeToSong2()
    {
        if (song2 == null)
        {
            Debug.LogWarning("BackgroundMusicManager: Song 2 is not assigned!");
            return;
        }

        StopAllFades();
        fadeCoroutine = StartCoroutine(Crossfade(song2));
    }

    /// <summary>
    /// Fades out the current music.
    /// </summary>
    public void FadeOutMusic()
    {
        StopAllFades();
        fadeCoroutine = StartCoroutine(FadeOut(currentSource, fadeOutDuration));
    }

    /// <summary>
    /// Stops all music immediately.
    /// </summary>
    public void StopMusic()
    {
        StopAllFades();
        audioSource1.Stop();
        audioSource2.Stop();
        audioSource1.volume = 0f;
        audioSource2.volume = 0f;
    }

    /// <summary>
    /// Pauses the current music.
    /// </summary>
    public void PauseMusic()
    {
        if (currentSource.isPlaying)
        {
            currentSource.Pause();
        }
    }

    /// <summary>
    /// Resumes the paused music.
    /// </summary>
    public void ResumeMusic()
    {
        if (!currentSource.isPlaying && currentSource.clip != null)
        {
            currentSource.UnPause();
        }
    }

    /// <summary>
    /// Sets the maximum volume for the music.
    /// </summary>
    /// <param name="volume">Volume value between 0 and 1.</param>
    public void SetVolume(float volume)
    {
        maxVolume = Mathf.Clamp01(volume);
        if (currentSource.isPlaying)
        {
            currentSource.volume = maxVolume;
        }
    }

    private void StopAllFades()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private IEnumerator FadeIn(AudioSource source, float duration)
    {
        source.volume = 0f;
        source.Play();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, maxVolume, elapsed / duration);
            yield return null;
        }

        source.volume = maxVolume;
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    private IEnumerator Crossfade(AudioClip newClip)
    {
        // Setup next source with new clip
        nextSource.clip = newClip;
        nextSource.loop = loopMusic;
        nextSource.volume = 0f;
        nextSource.Play();

        float elapsed = 0f;
        float startVolume = currentSource.volume;

        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / crossfadeDuration;

            // Fade out current, fade in next
            currentSource.volume = Mathf.Lerp(startVolume, 0f, t);
            nextSource.volume = Mathf.Lerp(0f, maxVolume, t);

            yield return null;
        }

        // Finalize
        currentSource.Stop();
        currentSource.volume = 0f;
        nextSource.volume = maxVolume;

        // Swap sources for next crossfade
        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }
}
