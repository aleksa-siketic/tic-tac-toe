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

    // Cached reference to the GameManager; found at startup.
    private GameManager gameManager;

    private void Start()
    {
        // Find the GameManager in the scene. FindFirstObjectByType is the
        // modern replacement for the deprecated FindObjectOfType.
        gameManager = FindAnyObjectByType<GameManager>();

        // Hook up our click handler to the Button component.
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnCellClicked);

        // Start with the mark invisible.
        if (markImage != null)
        {
            markImage.gameObject.SetActive(false);
        }
    }

    private void OnCellClicked()
    {
        // Ask the GameManager to place a mark. If it returns false,
        // the move was invalid (cell taken or game over) - do nothing.
        bool placed = gameManager.PlaceMark(cellIndex);
        if (!placed)
        {
            return;
        }

        // Move was accepted. Reveal the appropriate sprite.
        GameManager.CellState state = gameManager.GetCellState(cellIndex);
        UpdateVisual(state);
    }

    /// <summary>
    /// Shows the correct sprite based on the cell's state.
    /// Called after a successful placement; can also be called to reset.
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
}