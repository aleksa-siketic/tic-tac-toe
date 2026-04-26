using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the GameScene HUD: timer, move counters, settings button.
/// Subscribes to GameManager events to stay in sync with game state.
/// </summary>
public class GameSceneUI : MonoBehaviour
{
    [Header("HUD Texts")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI player1MovesText;
    [SerializeField] private TextMeshProUGUI player2MovesText;

    [Header("Buttons")]
    [SerializeField] private Button settingsButton;

    [Header("Popups")]
    [SerializeField] private SettingsPopup settingsPopup;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameSceneUI: GameManager.Instance is null.");
            return;
        }

        GameManager.Instance.OnMarkPlaced += OnMarkPlaced;
        GameManager.Instance.OnGameStarted += OnGameStarted;

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        // Initialize the HUD text right away.
        UpdateMoveTexts();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMarkPlaced -= OnMarkPlaced;
            GameManager.Instance.OnGameStarted -= OnGameStarted;
        }
    }

    private void Update()
    {
        // Tick the timer every frame while the game is in progress.
        if (GameManager.Instance == null || timerText == null) return;
        timerText.text = $"Time: {TimeFormatter.Format(GameManager.Instance.CurrentMatchDuration)}";
    }

    private void OnGameStarted()
    {
        UpdateMoveTexts();
    }

    private void OnMarkPlaced(int cellIndex, GameManager.CellState state)
    {
        UpdateMoveTexts();
    }

    private void UpdateMoveTexts()
    {
        if (GameManager.Instance == null) return;
        if (player1MovesText != null) player1MovesText.text = $"P1: {GameManager.Instance.Player1Moves} moves";
        if (player2MovesText != null) player2MovesText.text = $"P2: {GameManager.Instance.Player2Moves} moves";
    }

    private void OnSettingsClicked()
    {
        if (settingsPopup != null) settingsPopup.Open();
    }
}