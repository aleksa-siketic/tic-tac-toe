using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Popup for selecting a theme (X/O color pair) before starting the game.
/// Each toggle in the inspector array represents one theme.
/// Start button saves selection and loads GameScene.
/// </summary>
public class ThemePopup : PopupBase
{
    [SerializeField] private Toggle[] themeToggles;
    [SerializeField] private Button startButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private string gameSceneName = "GameScene";

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        if (closeButton != null) closeButton.onClick.AddListener(Close);

        // Wire up each toggle to know its index when clicked.
        for (int i = 0; i < themeToggles.Length; i++)
        {
            int capturedIndex = i; // capture for the lambda
            if (themeToggles[i] != null)
            {
                themeToggles[i].onValueChanged.AddListener(isOn =>
                {
                    if (isOn) OnThemeSelected(capturedIndex);
                });
            }
        }
    }

    // Sync the toggle visuals with the currently stored theme when opened.
    protected override void OnOpened()
    {
        if (ThemeManager.Instance == null) return;

        int current = ThemeManager.Instance.SelectedIndex;
        for (int i = 0; i < themeToggles.Length; i++)
        {
            if (themeToggles[i] != null)
            {
                themeToggles[i].SetIsOnWithoutNotify(i == current);
            }
        }
    }

    private void OnThemeSelected(int index)
    {
        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.SelectTheme(index);
        }
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}