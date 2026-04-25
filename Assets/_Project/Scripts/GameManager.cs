using System;
using UnityEngine;

/// <summary>
/// The brain of the Tic-Tac-Toe game.
/// Owns the board state, whose turn it is, and the rules.
/// Does NOT handle visuals, sound, or UI - those are separate concerns.
/// Broadcasts events so other scripts can react without being called directly.
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum CellState { Empty, X, O }
    public enum Player { Player1, Player2 }
    public enum GameResult { Player1Wins, Player2Wins, Draw }

    private static readonly int[][] WinLines = new int[][]
    {
        new int[] { 0, 1, 2 },
        new int[] { 3, 4, 5 },
        new int[] { 6, 7, 8 },
        new int[] { 0, 3, 6 },
        new int[] { 1, 4, 7 },
        new int[] { 2, 5, 8 },
        new int[] { 0, 4, 8 },
        new int[] { 2, 4, 6 },
    };

    private CellState[] board = new CellState[9];
    private Player currentPlayer = Player.Player1;
    private bool gameOver = false;

    // === Match tracking ===
    private float matchStartTime;
    private float lastMatchDuration;
    private int player1Moves;
    private int player2Moves;

    // === Events ===
    public event Action<int, CellState> OnMarkPlaced;
    public event Action<GameResult, int[]> OnGameEnded;
    public event Action OnGameStarted;

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = CellState.Empty;
        }
        currentPlayer = Player.Player1;
        gameOver = false;
        player1Moves = 0;
        player2Moves = 0;
        matchStartTime = Time.time;
        Debug.Log("New game started. Player 1's turn.");
        OnGameStarted?.Invoke();
    }

    public bool PlaceMark(int cellIndex)
    {
        if (cellIndex < 0 || cellIndex >= board.Length)
        {
            Debug.LogWarning($"PlaceMark: invalid cell index {cellIndex}.");
            return false;
        }
        if (gameOver) return false;
        if (board[cellIndex] != CellState.Empty) return false;

        CellState placedState = (currentPlayer == Player.Player1) ? CellState.X : CellState.O;
        board[cellIndex] = placedState;

        // Increment the move counter for the player who just moved.
        if (currentPlayer == Player.Player1) player1Moves++;
        else player2Moves++;

        Debug.Log($"{currentPlayer} placed {placedState} at cell {cellIndex}.");
        OnMarkPlaced?.Invoke(cellIndex, placedState);

        if (CheckForWin(out int[] winningLine))
        {
            EndGame(currentPlayer == Player.Player1 ? GameResult.Player1Wins : GameResult.Player2Wins, winningLine);
            return true;
        }

        if (IsBoardFull())
        {
            EndGame(GameResult.Draw, null);
            return true;
        }

        currentPlayer = (currentPlayer == Player.Player1) ? Player.Player2 : Player.Player1;
        return true;
    }

    private void EndGame(GameResult result, int[] winningLine)
    {
        gameOver = true;
        lastMatchDuration = Time.time - matchStartTime;
        Debug.Log($"Game over: {result}. Duration: {lastMatchDuration:F1}s. Winning line: {(winningLine != null ? string.Join(",", winningLine) : "none")}");

        // Record the result in StatsManager (singleton, persistent across scenes).
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.RecordGameResult(result, lastMatchDuration);
        }

        OnGameEnded?.Invoke(result, winningLine);
    }

    private bool CheckForWin(out int[] winningLine)
    {
        foreach (int[] line in WinLines)
        {
            CellState a = board[line[0]];
            CellState b = board[line[1]];
            CellState c = board[line[2]];

            if (a != CellState.Empty && a == b && b == c)
            {
                winningLine = line;
                return true;
            }
        }
        winningLine = null;
        return false;
    }

    private bool IsBoardFull()
    {
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == CellState.Empty) return false;
        }
        return true;
    }

    // Public read-only accessors.
    public CellState GetCellState(int cellIndex) => board[cellIndex];
    public Player GetCurrentPlayer() => currentPlayer;
    public bool IsGameOver() => gameOver;

    /// <summary>Elapsed time of the current match in seconds.</summary>
    public float CurrentMatchDuration => gameOver ? lastMatchDuration : Time.time - matchStartTime;

    /// <summary>Duration of the most recently completed match.</summary>
    public float LastMatchDuration => lastMatchDuration;

    public int Player1Moves => player1Moves;
    public int Player2Moves => player2Moves;
}