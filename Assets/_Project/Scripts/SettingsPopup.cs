using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup with toggles for Music (BGM) and Sound Effects (SFX).
/// Reads/writes values through SettingsManager.
/// </summary>
public class SettingsPopup : PopupBase
{
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Button closeButton;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Wire up close button.
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        // Wire up toggle value-change handlers.
        if (musicToggle != null)
        {
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }
        if (sfxToggle != null)
        {
            sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);
        }
    }

    // When we open the popup, sync the toggle visuals with the stored settings.
    protected override void OnOpened()
    {
        if (SettingsManager.Instance == null) return;

        // Set without triggering the valueChanged event.
        if (musicToggle != null)
        {
            musicToggle.SetIsOnWithoutNotify(SettingsManager.Instance.MusicEnabled);
        }
        if (sfxToggle != null)
        {
            sfxToggle.SetIsOnWithoutNotify(SettingsManager.Instance.SfxEnabled);
        }
    }

    private void OnMusicToggleChanged(bool value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMusicEnabled(value);
        }
    }

    private void OnSfxToggleChanged(bool value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetSfxEnabled(value);
        }
    }
}