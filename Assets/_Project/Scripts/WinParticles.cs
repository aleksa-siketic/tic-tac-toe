using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns a burst of UI image particles when a player wins.
/// Each particle scatters outward, rotates, and fades over its lifetime.
/// Subscribes to GameManager.OnGameEnded to trigger automatically.
/// </summary>
public class WinParticles : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private RectTransform spawnOrigin;
    [SerializeField] private RectTransform particleParent;
    [SerializeField] private Sprite[] particleSprites;
    [SerializeField] private int particleCount = 40;

    [Header("Animation")]
    [SerializeField] private float minSpeed = 400f;
    [SerializeField] private float maxSpeed = 900f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float minSize = 100f;
    [SerializeField] private float maxSize = 200f;
    [SerializeField] private float gravity = -600f;

    [Header("Color")]
    [SerializeField] private Color[] particleColors = new Color[]
    {
        new Color(1f, 0.9f, 0.2f, 1f),    // gold
        new Color(1f, 1f, 1f, 1f),         // white
        new Color(1f, 0.5f, 0.1f, 1f),    // orange
        new Color(0.3f, 0.9f, 1f, 1f),    // cyan
    };

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnded += HandleGameEnded;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameEnded -= HandleGameEnded;
        }
    }

    private void HandleGameEnded(GameManager.GameResult result, int[] winningLine)
    {
        // Only celebrate wins, not draws.
        if (result == GameManager.GameResult.Draw) return;
        if (particleSprites == null || particleSprites.Length == 0) return;
        if (particleParent == null) return;

        Vector2 origin = spawnOrigin != null ? spawnOrigin.anchoredPosition : Vector2.zero;

        for (int i = 0; i < particleCount; i++)
        {
            SpawnParticle(origin);
        }
    }

    private void SpawnParticle(Vector2 origin)
    {
        GameObject go = new GameObject("Particle", typeof(RectTransform), typeof(Image));
        RectTransform rect = go.GetComponent<RectTransform>();
        Image img = go.GetComponent<Image>();

        rect.SetParent(particleParent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = origin;

        float size = Random.Range(minSize, maxSize);
        rect.sizeDelta = new Vector2(size, size);

        img.sprite = particleSprites[Random.Range(0, particleSprites.Length)];
        img.raycastTarget = false;
        if (particleColors != null && particleColors.Length > 0)
        {
            img.color = particleColors[Random.Range(0, particleColors.Length)];
        }

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

        StartCoroutine(AnimateParticle(rect, img, velocity));
    }

    private IEnumerator AnimateParticle(RectTransform rect, Image img, Vector2 initialVelocity)
    {
        float elapsed = 0f;
        Vector2 velocity = initialVelocity;
        float rotationSpeed = Random.Range(-360f, 360f);
        Color startColor = img.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;

            // Apply gravity to velocity.
            velocity.y += gravity * Time.deltaTime;

            // Update position.
            rect.anchoredPosition += velocity * Time.deltaTime;

            // Rotate.
            rect.localRotation = Quaternion.Euler(0f, 0f, rect.localRotation.eulerAngles.z + rotationSpeed * Time.deltaTime);

            // Fade out in the second half.
            float alpha = t < 0.5f ? 1f : Mathf.Lerp(1f, 0f, (t - 0.5f) / 0.5f);
            img.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(rect.gameObject);
    }
}