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

    private GameManager gameManager;

    protected override void Awake()
    {
        base.Awake(); // important: lets PopupBase hide the content
    }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameOverPopup: no GameManager found in scene.");
            return;
        }

        gameManager.OnGameEnded += HandleGameEnded;
        gameManager.OnGameStarted += HandleGameStarted;

        if (retryButton != null) retryButton.onClick.AddListener(OnRetryClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnGameEnded -= HandleGameEnded;
            gameManager.OnGameStarted -= HandleGameStarted;
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

        if (durationText != null)
        {
            durationText.text = "Time: 00:00"; // real timer comes later
        }

        Open();
    }

    private void OnRetryClicked()
    {
        if (gameManager != null) gameManager.StartNewGame();
    }

    private void OnExitClicked()
    {
        SceneManager.LoadScene(playSceneName);
    }
}