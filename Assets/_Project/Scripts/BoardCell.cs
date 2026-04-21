using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single cell on the Tic-Tac-Toe board.
/// Handles click input and updates its visual mark (X or O).
/// Does NOT contain game logic - delegates to GameManager.
/// </summary>
[RequireComponent(typeof(Button))]
public class BoardCell : MonoBehaviour
{
    // The index of this cell in the board (0-8). Set in the Inspector.
    [SerializeField] private int cellIndex;

    // The child Image that displays the X or O sprite. Set in the Inspector.
    [SerializeField] private Image markImage;

    // The sprites to show for each player's mark. Set in the Inspector.
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnCellClicked);

        // Subscribe to game start events so we can reset our visual.
        if (gameManager != null)
        {
            gameManager.OnGameStarted += ResetVisual;
        }

        // Start with the mark invisible.
        ResetVisual();
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent errors when the scene unloads.
        if (gameManager != null)
        {
            gameManager.OnGameStarted -= ResetVisual;
        }
    }

    private void OnCellClicked()
    {
        bool placed = gameManager.PlaceMark(cellIndex);
        if (!placed)
        {
            return;
        }

        GameManager.CellState state = gameManager.GetCellState(cellIndex);
        UpdateVisual(state);
    }

    /// <summary>
    /// Shows the correct sprite based on the cell's state.
    /// Called after a successful placement.
    /// </summary>
    public void UpdateVisual(GameManager.CellState state)
    {
        if (state == GameManager.CellState.Empty)
        {
            markImage.gameObject.SetActive(false);
            return;
        }

        markImage.sprite = (state == GameManager.CellState.X) ? xSprite : oSprite;
        markImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the mark. Called when a new game starts.
    /// </summary>
    private void ResetVisual()
    {
        if (markImage != null)
        {
            markImage.gameObject.SetActive(false);
        }
    }
}