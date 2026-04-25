using System;
using UnityEngine;

/// <summary>
/// Singleton holding the currently selected theme (X/O sprite pair).
/// Persists the selection across sessions.
/// </summary>
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    private const string KeySelectedTheme = "theme.selected";

    // Simple data container for one theme.
    [Serializable]
    public class Theme
    {
        public string name;
        public Sprite xSprite;
        public Sprite oSprite;
    }

    // All available themes, configured in the Inspector.
    [SerializeField] private Theme[] themes;

    public int SelectedIndex { get; private set; }

    public Theme CurrentTheme => themes[SelectedIndex];

    public event Action<Theme> OnThemeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved theme, clamp in case theme count has changed.
        int saved = PlayerPrefs.GetInt(KeySelectedTheme, 0);
        SelectedIndex = Mathf.Clamp(saved, 0, themes.Length - 1);
    }

    /// <summary>
    /// Changes the selected theme, persists, and broadcasts the change.
    /// </summary>
    public void SelectTheme(int index)
    {
        if (index < 0 || index >= themes.Length) return;
        if (index == SelectedIndex) return;

        SelectedIndex = index;
        PlayerPrefs.SetInt(KeySelectedTheme, index);
        PlayerPrefs.Save();
        OnThemeChanged?.Invoke(themes[index]);
    }

    public int ThemeCount => themes != null ? themes.Length : 0;

    public Theme GetTheme(int index)
    {
        if (index < 0 || index >= themes.Length) return null;
        return themes[index];
    }
}