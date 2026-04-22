using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the PlayScene's main menu.
/// Wires each menu button to its corresponding popup.
/// </summary>
public class PlaySceneUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Popups")]
    [SerializeField] private ThemePopup themePopup;
    [SerializeField] private StatsPopup statsPopup;
    [SerializeField] private SettingsPopup settingsPopup;
    [SerializeField] private ExitConfirmPopup exitConfirmPopup;

    private void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (statsButton != null) statsButton.onClick.AddListener(OnStatsClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnPlayClicked()
    {
        if (themePopup != null) themePopup.Open();
    }

    private void OnStatsClicked()
    {
        if (statsPopup != null) statsPopup.Open();
    }

    private void OnSettingsClicked()
    {
        if (settingsPopup != null) settingsPopup.Open();
    }

    private void OnExitClicked()
    {
        if (exitConfirmPopup != null) exitConfirmPopup.Open();
    }
}