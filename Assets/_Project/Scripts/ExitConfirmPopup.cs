using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Confirmation popup shown before quitting the application.
/// Yes quits the game; No closes the popup.
/// </summary>
public class ExitConfirmPopup : PopupBase
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(Close);
    }

    private void OnYesClicked()
    {
        // Quit the application. In the editor, Application.Quit does nothing;
        // use a conditional to also stop play mode for testing convenience.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}