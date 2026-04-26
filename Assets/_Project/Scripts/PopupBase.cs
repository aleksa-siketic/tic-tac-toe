using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all popups in the game.
/// Handles show/hide logic via a Content child GameObject and
/// supports an optional open delay (e.g. so a click sound plays before
/// the popup pop sound, or so the winning strike is visible before the
/// game-over popup appears).
/// </summary>
public abstract class PopupBase : MonoBehaviour
{
    [SerializeField] protected GameObject content;

    [Tooltip("Seconds to wait before the popup actually appears. " +
             "Useful to let a preceding sound finish or to let the player " +
             "see what just happened before the popup shows.")]
    [SerializeField] protected float openDelay = 0f;

    public bool IsOpen { get; private set; }

    private Coroutine openRoutine;

    protected virtual void Awake()
    {
        if (content != null)
        {
            content.SetActive(false);
        }
    }

    public virtual void Open()
    {
        if (IsOpen) return;
        IsOpen = true;

        if (openDelay > 0f)
        {
            openRoutine = StartCoroutine(OpenAfterDelay());
        }
        else
        {
            ActivateAndAnnounce();
        }
    }

    public virtual void Close()
    {
        // If the popup was queued to open but hasn't yet, cancel the queue.
        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
            openRoutine = null;
        }

        if (content != null)
        {
            content.SetActive(false);
        }

        if (IsOpen)
        {
            IsOpen = false;
            OnClosed();
        }
    }

    private IEnumerator OpenAfterDelay()
    {
        yield return new WaitForSeconds(openDelay);
        openRoutine = null;
        ActivateAndAnnounce();
    }

    private void ActivateAndAnnounce()
    {
        if (content != null)
        {
            content.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPopup();
        }

        OnOpened();
    }

    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }
}