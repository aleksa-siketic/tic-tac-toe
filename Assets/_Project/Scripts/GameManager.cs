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
    // The possible states of a single cell on the board.
    public enum CellState { Empty, X, O }

    // Identifies which player is acting. Player1 places X, Player2 places O.
    public enum Player { Player1, Player2 }

    // The possible outcomes at the end of a match.
    public enum GameResult { Player1Wins, Player2Wins, Draw }

    // The 8 winning lines on a 3x3 board: 3 rows, 3 columns, 2 diagonals.
    // Each inner array holds the three cell indices that form a line.
    private static readonly int[][] WinLines = new int[][]
    {
        new int[] { 0, 1, 2 }, // top row
        new int[] { 3, 4, 5 }, // middle row
        new int[] { 6, 7, 8 }, // bottom row
        new int[] { 0, 3, 6 }, // left column
        new int[] { 1, 4, 7 }, // middle column
        new int[] { 2, 5, 8 }, // right column
        new int[] { 0, 4, 8 }, // main diagonal
        new int[] { 2, 4, 6 }, // anti-diagonal
    };

    // The flat array representing the board. Indices 0-8 map to:
    //   0 | 1 | 2
    //   3 | 4 | 5
    //   6 | 7 | 8
    private CellState[] board = new CellState[9];

    // Whose turn it currently is.
    private Player currentPlayer = Player.Player1;

    // True once the game is decided (win or draw); ignores further clicks.
    private bool gameOver = false;

    // === Events ===
    // Other scripts subscribe to these to react when things happen.
    // The ? before Invoke handles the case where no one is subscribed.

    /// <summary>Fired when a mark is successfully placed. Passes cell index and new state.</summary>
    public event Action<int, CellState> OnMarkPlaced;

    /// <summary>Fired when the game ends. Passes the result and the winning line (null for draw).</summary>
    public event Action<GameResult, int[]> OnGameEnded;

    /// <summary>Fired when a new game starts (board reset).</summary>
    public event Action OnGameStarted;

    // Called automatically by Unity when the scene starts.
    private void Start()
    {
        StartNewGame();
    }

    /// <summary>
    /// Resets the board and starts a fresh match.
    /// Call this from a "Retry" button later.
    /// </summary>
    public void StartNewGame()
    {
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = CellState.Empty;
        }
        currentPlayer = Player.Player1;
        gameOver = false;
        Debug.Log("New game started. Player 1's turn.");
        OnGameStarted?.Invoke();
    }

    /// <summary>
    /// Attempts to place the current player's mark at the given cell index.
    /// Returns true if the move was valid and placed; false otherwise.
    /// </summary>
    public bool PlaceMark(int cellIndex)
    {
        // Guard: invalid index.
        if (cellIndex < 0 || cellIndex >= board.Length)
        {
            Debug.LogWarning($"PlaceMark: invalid cell index {cellIndex}.");
            return false;
        }

        // Guard: game already finished.
        if (gameOver)
        {
            return false;
        }

        // Guard: cell already occupied.
        if (board[cellIndex] != CellState.Empty)
        {
            return false;
        }

        // Place the mark based on whose turn it is.
        CellState placedState = (currentPlayer == Player.Player1) ? CellState.X : CellState.O;
        board[cellIndex] = placedState;
        Debug.Log($"{currentPlayer} placed {placedState} at cell {cellIndex}.");

        // Notify listeners that a mark was placed.
        OnMarkPlaced?.Invoke(cellIndex, placedState);

        // Check if this move ended the game.
        if (CheckForWin(out int[] winningLine))
        {
            gameOver = true;
            GameResult result = (currentPlayer == Player.Player1) ? GameResult.Player1Wins : GameResult.Player2Wins;
            Debug.Log($"Game over: {result}. Winning line: [{string.Join(",", winningLine)}]");
            OnGameEnded?.Invoke(result, winningLine);
            return true;
        }

        if (IsBoardFull())
        {
            gameOver = true;
            Debug.Log("Game over: Draw.");
            OnGameEnded?.Invoke(GameResult.Draw, null);
            return true;
        }

        // Game continues. Swap turns.
        currentPlayer = (currentPlayer == Player.Player1) ? Player.Player2 : Player.Player1;
        return true;
    }

    /// <summary>
    /// Checks all 8 winning lines. If any line has three matching non-empty marks,
    /// returns true and sets winningLine to the indices of that line.
    /// </summary>
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

    /// <summary>Returns true when every cell is occupied.</summary>
    private bool IsBoardFull()
    {
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == CellState.Empty) return false;
        }
        return true;
    }

    // Public read-only accessors so other scripts can inspect state safely.
    public CellState GetCellState(int cellIndex) => board[cellIndex];
    public Player GetCurrentPlayer() => currentPlayer;
    public bool IsGameOver() => gameOver;
}