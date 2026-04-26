using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to any Button to make it play the UI click sound when pressed.
/// Hooks into Button.onClick and routes to AudioManager.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
        }
    }
}