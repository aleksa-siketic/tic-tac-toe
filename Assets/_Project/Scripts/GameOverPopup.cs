using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the Game Over popup shown at the end of a match.
/// Subscribes to GameManager events to appear/disappear automatically.
/// Exposes Retry and Exit button handlers.
/// </summary>
public class GameOverPopup : MonoBehaviour
{
    // The dim overlay behind the popup. Toggled with the popup.
    [SerializeField] private GameObject dimBackground;

    // The popup visual panel. Toggled with the dim overlay.
    [SerializeField] private GameObject popupPanel;

    // UI elements to populate with result data.
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI durationText;

    // Buttons to wire up.
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;

    // The name of the scene to load when the Exit button is pressed.
    [SerializeField] private string playSceneName = "PlayScene";

    private GameManager gameManager;

    private void Awake()
    {
        SetVisible(false);
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

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }
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
        SetVisible(false);
    }

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
            durationText.text = "Time: 00:00";
        }

        SetVisible(true);
    }

    // Toggles both the dim background and the popup panel together.
    private void SetVisible(bool visible)
    {
        if (dimBackground != null) dimBackground.SetActive(visible);
        if (popupPanel != null) popupPanel.SetActive(visible);
    }

    private void OnRetryClicked()
    {
        if (gameManager != null)
        {
            gameManager.StartNewGame();
        }
    }

    private void OnExitClicked()
    {
        SceneManager.LoadScene(playSceneName);
    }
}