using UnityEngine;

/// <summary>
/// Base class for all popups in the game.
/// Handles the shared open/close logic and toggles the visible content.
/// Specific popups inherit from this and add their own content logic.
/// </summary>
public abstract class PopupBase : MonoBehaviour
{
    // The container holding the popup visuals (dim + panel).
    // Assign the GameObject that should be shown/hidden.
    [SerializeField] protected GameObject content;

    // Tracks whether the popup is currently open.
    public bool IsOpen { get; private set; }

    protected virtual void Awake()
    {
        // Start hidden. Derived classes can override if they need to
        // run setup, but must call base.Awake() first.
        if (content != null)
        {
            content.SetActive(false);
        }
        IsOpen = false;
    }

    /// <summary>
    /// Opens the popup. Override OnOpened to add content-specific logic.
    /// </summary>
    public virtual void Open()
    {
        if (content != null)
        {
            content.SetActive(true);
        }
        IsOpen = true;
        OnOpened();
    }

    /// <summary>
    /// Closes the popup. Override OnClosed to add content-specific logic.
    /// </summary>
    public virtual void Close()
    {
        if (content != null)
        {
            content.SetActive(false);
        }
        IsOpen = false;
        OnClosed();
    }

    // Hooks for derived classes to react to open/close events.
    // Default implementations are empty.
    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }
}