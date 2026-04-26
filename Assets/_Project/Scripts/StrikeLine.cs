using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Draws the strike line through the three winning cells when the game ends with a win.
/// Subscribes to GameManager events to show/hide itself automatically.
/// Animates the line drawing in over a short duration for a satisfying win moment.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class StrikeLine : MonoBehaviour
{
    [SerializeField] private float straightLength = 900f;
    [SerializeField] private float diagonalLength = 1270f;
    [SerializeField] private float rowOffset = 290f;
    [SerializeField] private RectTransform boardRect;

    [Tooltip("How long the line takes to draw in.")]
    [SerializeField] private float animationDuration = 0.3f;

    private RectTransform lineRect;
    private Image image;
    private Coroutine drawRoutine;

    private void Awake()
    {
        lineRect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        // Hide the image by default (but keep the GameObject active so events work).
        image.enabled = false;
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("StrikeLine: GameManager.Instance is null.");
            return;
        }

        GameManager.Instance.OnGameEnded += HandleGameEnded;
        GameManager.Instance.OnGameStarted += HandleGameStarted;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnded -= HandleGameEnded;
            GameManager.Instance.OnGameStarted -= HandleGameStarted;
        }
    }

    private void HandleGameStarted()
    {
        if (drawRoutine != null)
        {
            StopCoroutine(drawRoutine);
            drawRoutine = null;
        }
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
        Vector2 boardOffset = boardRect != null ? boardRect.anchoredPosition : Vector2.zero;
        lineRect.anchoredPosition = position + boardOffset;
        lineRect.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (drawRoutine != null)
        {
            StopCoroutine(drawRoutine);
        }
        drawRoutine = StartCoroutine(AnimateDraw(length));

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayStrike();
        }
    }

    private IEnumerator AnimateDraw(float targetLength)
    {
        image.enabled = true;
        float currentHeight = lineRect.sizeDelta.y;

        if (animationDuration <= 0f)
        {
            lineRect.sizeDelta = new Vector2(targetLength, currentHeight);
            drawRoutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animationDuration);
            float width = Mathf.Lerp(0f, targetLength, t);
            lineRect.sizeDelta = new Vector2(width, currentHeight);
            yield return null;
        }

        lineRect.sizeDelta = new Vector2(targetLength, currentHeight);
        drawRoutine = null;
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