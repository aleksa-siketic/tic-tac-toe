using UnityEngine;

/// <summary>
/// Singleton managing all audio in the game.
/// Holds two AudioSources (one for looping BGM, one for one-shot SFX)
/// and exposes typed methods for each sound effect.
/// Reads on/off state from SettingsManager and reacts to changes.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip bgmClip;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip placementClip;
    [SerializeField] private AudioClip strikeClip;
    [SerializeField] private AudioClip popupClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Configure the BGM source.
        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.clip = bgmClip;
        }
    }

    private void Start()
    {
        // Subscribe to settings changes so BGM can start/stop when toggled.
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnMusicEnabledChanged += HandleMusicEnabledChanged;

            // Start BGM if music is currently enabled.
            if (SettingsManager.Instance.MusicEnabled)
            {
                PlayBgm();
            }
        }
        else
        {
            // No SettingsManager (e.g., GameScene standalone) - default to playing music.
            PlayBgm();
        }
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnMusicEnabledChanged -= HandleMusicEnabledChanged;
        }
    }

    private void HandleMusicEnabledChanged(bool enabled)
    {
        if (enabled) PlayBgm();
        else StopBgm();
    }

    public void PlayBgm()
    {
        if (bgmSource == null || bgmClip == null) return;
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void StopBgm()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    // === SFX ===
    // Each method respects the SFX toggle and gracefully no-ops if disabled.

    public void PlayClick() => PlaySfx(clickClip);
    public void PlayPlacement() => PlaySfx(placementClip);
    public void PlayStrike() => PlaySfx(strikeClip);
    public void PlayPopup() => PlaySfx(popupClip);

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        if (SettingsManager.Instance != null && !SettingsManager.Instance.SfxEnabled) return;
        sfxSource.PlayOneShot(clip);
    }
}