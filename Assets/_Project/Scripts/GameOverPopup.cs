using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Popup shown at the end of a match.
/// Displays result text, match duration, and Retry/Exit buttons.
/// Subscribes to GameManager events to auto-open on game end.
/// </summary>
public class GameOverPopup : PopupBase
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private string playSceneName = "PlayScene";

    protected override void Awake()
    {
        base.Awake(); // important: lets PopupBase hide the content
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameOverPopup: GameManager.Instance is null.");
            return;
        }

        GameManager.Instance.OnGameEnded += HandleGameEnded;
        GameManager.Instance.OnGameStarted += HandleGameStarted;

        if (retryButton != null) retryButton.onClick.AddListener(OnRetryClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnded -= HandleGameEnded;
            GameManager.Instance.OnGameStarted -= HandleGameStarted;
        }
    }

    private void HandleGameStarted() => Close();

    private void HandleGameEnded(GameManager.GameResult result, int[] winningLine)
    {
        if (resultText != null)
        {
            resultText.text = result switch
            {
                GameManager.GameResult.Player1Wins => "Player 1 Wins!",
                GameManager.GameResult.Player2Wins => "Player 2 Wins!",
                GameManager.GameResult.Draw => "Draw!",
                _ => "Game Over"
            };
        }

        if (durationText != null && GameManager.Instance != null)
        {
            durationText.text = $"Time: {TimeFormatter.Format(GameManager.Instance.LastMatchDuration)}";
        }

        Open();
    }

    private void OnRetryClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.StartNewGame();
    }

    private void OnExitClicked()
    {
        SceneManager.LoadScene(playSceneName);
    }
}