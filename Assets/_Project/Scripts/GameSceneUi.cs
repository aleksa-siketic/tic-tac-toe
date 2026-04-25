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

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameSceneUI: no GameManager found.");
            return;
        }

        gameManager.OnMarkPlaced += OnMarkPlaced;
        gameManager.OnGameStarted += OnGameStarted;

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        // Initialize the HUD text right away.
        UpdateMoveTexts();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnMarkPlaced -= OnMarkPlaced;
            gameManager.OnGameStarted -= OnGameStarted;
        }
    }

    private void Update()
    {
        // Tick the timer every frame while the game is in progress.
        if (gameManager == null || timerText == null) return;
        timerText.text = $"Time: {FormatDuration(gameManager.CurrentMatchDuration)}";
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
        if (gameManager == null) return;
        if (player1MovesText != null) player1MovesText.text = $"P1: {gameManager.Player1Moves} moves";
        if (player2MovesText != null) player2MovesText.text = $"P2: {gameManager.Player2Moves} moves";
    }

    private string FormatDuration(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        int minutes = total / 60;
        int secs = total % 60;
        return $"{minutes:00}:{secs:00}";
    }

    private void OnSettingsClicked()
    {
        if (settingsPopup != null) settingsPopup.Open();
    }
}