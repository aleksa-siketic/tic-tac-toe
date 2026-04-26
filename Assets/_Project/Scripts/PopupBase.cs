using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all popups in the game.
/// Handles show/hide logic via a Content child GameObject and
/// supports an optional open delay (e.g. so a click sound plays before
/// the popup pop sound, or so the winning strike is visible before the
/// game-over popup appears).
/// Adds a subtle scale-in animation when opening.
/// </summary>
public abstract class PopupBase : MonoBehaviour
{
    [SerializeField] protected GameObject content;

    [Tooltip("Seconds to wait before the popup actually appears.")]
    [SerializeField] protected float openDelay = 0f;

    [Tooltip("Duration of the scale-in animation.")]
    [SerializeField] protected float openAnimationDuration = 0.25f;

    public bool IsOpen { get; private set; }

    private Coroutine activeRoutine;
    private RectTransform contentRect;

    protected virtual void Awake()
    {
        if (content != null)
        {
            content.SetActive(false);
            contentRect = content.GetComponent<RectTransform>();
        }
    }

    public virtual void Open()
    {
        if (IsOpen) return;
        IsOpen = true;

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }
        activeRoutine = StartCoroutine(OpenSequence());
    }

    public virtual void Close()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
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

    private IEnumerator OpenSequence()
    {
        if (openDelay > 0f)
        {
            yield return new WaitForSeconds(openDelay);
        }

        if (content != null)
        {
            content.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPopup();
        }

        OnOpened();

        yield return AnimateScaleIn();

        activeRoutine = null;
    }

    private IEnumerator AnimateScaleIn()
    {
        if (contentRect == null || openAnimationDuration <= 0f) yield break;

        const float startScale = 0.7f;
        const float overshoot = 1.05f;
        const float endScale = 1f;

        float halfDuration = openAnimationDuration * 0.7f;
        float overshootDuration = openAnimationDuration * 0.3f;

        // Phase 1: scale from startScale to overshoot
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
            float scale = Mathf.Lerp(startScale, overshoot, t);
            contentRect.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        // Phase 2: scale back from overshoot to endScale
        elapsed = 0f;
        while (elapsed < overshootDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / overshootDuration);
            float scale = Mathf.Lerp(overshoot, endScale, t);
            contentRect.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        contentRect.localScale = Vector3.one;
    }

    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }
}