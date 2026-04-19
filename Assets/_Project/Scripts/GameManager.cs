using UnityEngine;

/// <summary>
/// The brain of the Tic-Tac-Toe game.
/// Owns the board state, whose turn it is, and the rules.
/// Does NOT handle visuals, sound, or UI - those are separate concerns.
/// </summary>
public class GameManager : MonoBehaviour
{
    // The possible states of a single cell on the board.
    public enum CellState { Empty, X, O }

    // Identifies which player is acting. Player1 places X, Player2 places O.
    public enum Player { Player1, Player2 }

    // The flat array representing the board. Indices 0-8 map to:
    //   0 | 1 | 2
    //   3 | 4 | 5
    //   6 | 7 | 8
    private CellState[] board = new CellState[9];

    // Whose turn it currently is.
    private Player currentPlayer = Player.Player1;

    // True once the game is decided (win or draw); ignores further clicks.
    private bool gameOver = false;

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
        board[cellIndex] = (currentPlayer == Player.Player1) ? CellState.X : CellState.O;
        Debug.Log($"{currentPlayer} placed {board[cellIndex]} at cell {cellIndex}.");

        // Swap turns. Win detection will come later.
        currentPlayer = (currentPlayer == Player.Player1) ? Player.Player2 : Player.Player1;

        return true;
    }

    // Public read-only accessors so other scripts can inspect state safely.
    public CellState GetCellState(int cellIndex) => board[cellIndex];
    public Player GetCurrentPlayer() => currentPlayer;
    public bool IsGameOver() => gameOver;
}