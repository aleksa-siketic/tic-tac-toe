using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adjusts the CanvasScaler's match value based on the current aspect ratio
/// so that UI scales by whichever screen dimension is more constrained.
/// In portrait (taller than wide), match width.
/// In landscape (wider than tall), match height.
/// Re-evaluates every frame so device rotation / window resize is handled live.
/// </summary>
[RequireComponent(typeof(CanvasScaler))]
public class OrientationManager : MonoBehaviour
{
    private CanvasScaler scaler;
    private float lastAspect = -1f;

    private void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
    }

    private void Update()
    {
        float aspect = (float)Screen.width / Screen.height;
        if (Mathf.Approximately(aspect, lastAspect)) return;
        lastAspect = aspect;

        // Reference resolution is portrait (e.g. 1080x1920).
        // In portrait orientation we want to fit width (match=0).
        // In landscape we want to fit height (match=1) so content doesn't get cut off.
        bool isLandscape = aspect > 1f;
        scaler.matchWidthOrHeight = isLandscape ? 1f : 0f;
    }
}