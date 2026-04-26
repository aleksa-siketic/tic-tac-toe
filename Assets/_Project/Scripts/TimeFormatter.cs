using UnityEngine;

/// <summary>
/// Static utility for formatting time durations as MM:SS strings.
/// Used by HUD timers, stats display, and game-over popups.
/// </summary>
public static class TimeFormatter
{
    /// <summary>
    /// Formats a duration in seconds as a "MM:SS" string.
    /// Negative or invalid values are clamped to 0.
    /// </summary>
    public static string Format(float seconds)
    {
        int total = Mathf.Max(0, Mathf.FloorToInt(seconds));
        int minutes = total / 60;
        int secs = total % 60;
        return $"{minutes:00}:{secs:00}";
    }
}