using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popup displaying game statistics.
/// Reads from StatsManager when opened and refreshes on stat changes.
/// </summary>
public class StatsPopup : PopupBase
{
    [SerializeField] private TextMeshProUGUI gamesPlayedText;
    [SerializeField] private TextMeshProUGUI player1WinsText;
    [SerializeField] private TextMeshProUGUI player2WinsText;
    [SerializeField] private TextMeshProUGUI drawsText;
    [SerializeField] private TextMeshProUGUI avgDurationText;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClicked);

        // Refresh whenever stats change while popup is open.
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.OnStatsChanged += RefreshDisplay;
        }
    }

    private void OnDestroy()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.OnStatsChanged -= RefreshDisplay;
        }
    }

    protected override void OnOpened()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (StatsManager.Instance == null) return;

        var s = StatsManager.Instance;
        if (gamesPlayedText != null) gamesPlayedText.text = $"Games played: {s.GamesPlayed}";
        if (player1WinsText != null) player1WinsText.text = $"Player 1 wins: {s.Player1Wins}";
        if (player2WinsText != null) player2WinsText.text = $"Player 2 wins: {s.Player2Wins}";
        if (drawsText != null) drawsText.text = $"Draws: {s.Draws}";
        if (avgDurationText != null) avgDurationText.text = $"Avg duration: {FormatDuration(s.AverageDurationSeconds)}";
    }

    private string FormatDuration(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return $"{minutes:00}:{secs:00}";
    }

    private void OnResetClicked()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ResetStats();
        }
    }
}