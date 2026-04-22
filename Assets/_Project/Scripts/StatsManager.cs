using System;
using UnityEngine;

/// <summary>
/// Singleton that tracks and persists game statistics across sessions.
/// Accumulates games played, wins, draws, and total duration.
/// </summary>
public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    // PlayerPrefs keys.
    private const string KeyGamesPlayed = "stats.gamesPlayed";
    private const string KeyPlayer1Wins = "stats.player1Wins";
    private const string KeyPlayer2Wins = "stats.player2Wins";
    private const string KeyDraws = "stats.draws";
    private const string KeyTotalDurationSeconds = "stats.totalDurationSeconds";

    // Current stat values. All start at 0 if no save exists.
    public int GamesPlayed { get; private set; }
    public int Player1Wins { get; private set; }
    public int Player2Wins { get; private set; }
    public int Draws { get; private set; }
    public float TotalDurationSeconds { get; private set; }

    /// <summary>Average duration in seconds. Returns 0 if no games played yet.</summary>
    public float AverageDurationSeconds =>
        GamesPlayed > 0 ? TotalDurationSeconds / GamesPlayed : 0f;

    // Fired whenever stats change, so any open StatsPopup can refresh.
    public event Action OnStatsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadStats();
    }

    private void LoadStats()
    {
        GamesPlayed = PlayerPrefs.GetInt(KeyGamesPlayed, 0);
        Player1Wins = PlayerPrefs.GetInt(KeyPlayer1Wins, 0);
        Player2Wins = PlayerPrefs.GetInt(KeyPlayer2Wins, 0);
        Draws = PlayerPrefs.GetInt(KeyDraws, 0);
        TotalDurationSeconds = PlayerPrefs.GetFloat(KeyTotalDurationSeconds, 0f);
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt(KeyGamesPlayed, GamesPlayed);
        PlayerPrefs.SetInt(KeyPlayer1Wins, Player1Wins);
        PlayerPrefs.SetInt(KeyPlayer2Wins, Player2Wins);
        PlayerPrefs.SetInt(KeyDraws, Draws);
        PlayerPrefs.SetFloat(KeyTotalDurationSeconds, TotalDurationSeconds);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Records the result of a completed match and persists.
    /// Call this from GameManager when the game ends.
    /// </summary>
    public void RecordGameResult(GameManager.GameResult result, float durationSeconds)
    {
        GamesPlayed++;
        TotalDurationSeconds += durationSeconds;

        switch (result)
        {
            case GameManager.GameResult.Player1Wins:
                Player1Wins++;
                break;
            case GameManager.GameResult.Player2Wins:
                Player2Wins++;
                break;
            case GameManager.GameResult.Draw:
                Draws++;
                break;
        }

        SaveStats();
        OnStatsChanged?.Invoke();
    }

    /// <summary>Clears all stats and persists the reset.</summary>
    public void ResetStats()
    {
        GamesPlayed = 0;
        Player1Wins = 0;
        Player2Wins = 0;
        Draws = 0;
        TotalDurationSeconds = 0f;
        SaveStats();
        OnStatsChanged?.Invoke();
    }
}