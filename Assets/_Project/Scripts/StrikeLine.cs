using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Draws the strike line through the three winning cells when the game ends with a win.
/// Subscribes to GameManager events to show/hide itself automatically.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class StrikeLine : MonoBehaviour
{
    [SerializeField] private float straightLength = 900f;
    [SerializeField] private float diagonalLength = 1270f;
    [SerializeField] private float rowOffset = 290f;

    private RectTransform lineRect;
    private Image image;
    private GameManager gameManager;

    private void Awake()
    {
        // Awake runs even if the GameObject starts disabled, as long as the script is enabled.
        // We grab component references here.
        lineRect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Hide the image by default (but keep the GameObject active so events work).
        image.enabled = false;
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("StrikeLine: no GameManager found in scene.");
            return;
        }

        gameManager.OnGameEnded += HandleGameEnded;
        gameManager.OnGameStarted += HandleGameStarted;
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnGameEnded -= HandleGameEnded;
            gameManager.OnGameStarted -= HandleGameStarted;
        }
    }

    private void HandleGameStarted()
    {
        image.enabled = false;
    }

    private void HandleGameEnded(GameManager.GameResult result, int[] winningLine)
    {
        if (result == GameManager.GameResult.Draw || winningLine == null)
        {
            image.enabled = false;
            return;
        }

        (Vector2 position, float angle, float length) = GetStrikeConfig(winningLine);

        lineRect.anchoredPosition = position;
        lineRect.localRotation = Quaternion.Euler(0f, 0f, angle);
        lineRect.sizeDelta = new Vector2(length, lineRect.sizeDelta.y);

        image.enabled = true;
        Debug.Log($"StrikeLine: drew line [{string.Join(",", winningLine)}] at pos {position}, angle {angle}, length {length}");
    }

    private (Vector2 position, float angle, float length) GetStrikeConfig(int[] line)
    {
        // Rows
        if (line[0] == 0 && line[1] == 1) return (new Vector2(0f,  rowOffset), 0f, straightLength);
        if (line[0] == 3 && line[1] == 4) return (new Vector2(0f,  0f),        0f, straightLength);
        if (line[0] == 6 && line[1] == 7) return (new Vector2(0f, -rowOffset), 0f, straightLength);

        // Columns
        if (line[0] == 0 && line[1] == 3) return (new Vector2(-rowOffset, 0f), 90f, straightLength);
        if (line[0] == 1 && line[1] == 4) return (new Vector2(0f,         0f), 90f, straightLength);
        if (line[0] == 2 && line[1] == 5) return (new Vector2( rowOffset, 0f), 90f, straightLength);

        // Diagonals
        if (line[0] == 0 && line[2] == 8) return (Vector2.zero, -45f, diagonalLength);
        if (line[0] == 2 && line[2] == 6) return (Vector2.zero,  45f, diagonalLength);

        Debug.LogWarning($"StrikeLine: unknown line [{string.Join(",", line)}]");
        return (Vector2.zero, 0f, straightLength);
    }
}