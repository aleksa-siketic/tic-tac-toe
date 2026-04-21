using System;
using UnityEngine;

/// <summary>
/// Singleton that stores player settings (audio toggles, etc.)
/// and persists them across scenes and sessions using PlayerPrefs.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    // Static instance accessible from anywhere.
    public static SettingsManager Instance { get; private set; }

    // PlayerPrefs keys. Use constants to avoid typos.
    private const string KeyMusic = "settings.music";
    private const string KeySfx = "settings.sfx";

    // Current state. Defaults to true (audio on).
    public bool MusicEnabled { get; private set; } = true;
    public bool SfxEnabled { get; private set; } = true;

    // Events for other scripts (like AudioManager) to react to changes.
    public event Action<bool> OnMusicEnabledChanged;
    public event Action<bool> OnSfxEnabledChanged;

    private void Awake()
    {
        // Singleton enforcement: destroy any duplicates.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // This GameObject persists across scene loads so settings survive scene transitions.
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    /// <summary>
    /// Reads saved settings from PlayerPrefs. Defaults to true if never set.
    /// PlayerPrefs stores ints (0/1) for bools.
    /// </summary>
    private void LoadSettings()
    {
        MusicEnabled = PlayerPrefs.GetInt(KeyMusic, 1) == 1;
        SfxEnabled = PlayerPrefs.GetInt(KeySfx, 1) == 1;
    }

    /// <summary>
    /// Updates the music setting and persists it.
    /// </summary>
    public void SetMusicEnabled(bool enabled)
    {
        if (MusicEnabled == enabled) return;
        MusicEnabled = enabled;
        PlayerPrefs.SetInt(KeyMusic, enabled ? 1 : 0);
        PlayerPrefs.Save();
        OnMusicEnabledChanged?.Invoke(enabled);
    }

    /// <summary>
    /// Updates the SFX setting and persists it.
    /// </summary>
    public void SetSfxEnabled(bool enabled)
    {
        if (SfxEnabled == enabled) return;
        SfxEnabled = enabled;
        PlayerPrefs.SetInt(KeySfx, enabled ? 1 : 0);
        PlayerPrefs.Save();
        OnSfxEnabledChanged?.Invoke(enabled);
    }
}