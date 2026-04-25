using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single cell on the Tic-Tac-Toe board.
/// Handles click input and updates its visual mark (X or O).
/// Reads sprites from the active theme via ThemeManager.
/// Does NOT contain game logic - delegates to GameManager.
/// </summary>
[RequireComponent(typeof(Button))]
public class BoardCell : MonoBehaviour
{
    // The index of this cell in the board (0-8). Set in the Inspector.
    [SerializeField] private int cellIndex;

    // The child Image that displays the X or O sprite. Set in the Inspector.
    [SerializeField] private Image markImage;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnCellClicked);

        if (gameManager != null)
        {
            gameManager.OnGameStarted += ResetVisual;
        }

        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.OnThemeChanged += OnThemeChanged;
        }

        ResetVisual();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnGameStarted -= ResetVisual;
        }
        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.OnThemeChanged -= OnThemeChanged;
        }
    }

    private void OnCellClicked()
    {
        bool placed = gameManager.PlaceMark(cellIndex);
        if (!placed) return;

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

        Sprite sprite = GetSpriteForState(state);
        if (sprite == null)
        {
            // No theme available; leave the mark hidden rather than show nothing.
            Debug.LogWarning($"BoardCell: no sprite available for {state}. Is ThemeManager present in the scene?");
            markImage.gameObject.SetActive(false);
            return;
        }

        markImage.sprite = sprite;
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

    /// <summary>
    /// Called when the user picks a different theme.
    /// Re-renders the current cell so the new theme's sprite shows.
    /// </summary>
    private void OnThemeChanged(ThemeManager.Theme _)
    {
        if (gameManager == null) return;
        UpdateVisual(gameManager.GetCellState(cellIndex));
    }

    private Sprite GetSpriteForState(GameManager.CellState state)
    {
        if (ThemeManager.Instance == null) return null;

        ThemeManager.Theme theme = ThemeManager.Instance.CurrentTheme;
        if (theme == null) return null;

        return state == GameManager.CellState.X ? theme.xSprite : theme.oSprite;
    }
}