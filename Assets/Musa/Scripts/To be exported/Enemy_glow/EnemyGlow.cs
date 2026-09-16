using System.Collections;
using UnityEngine;

public class Enemy3DGlow : MonoBehaviour
{
    private Renderer meshRenderer;
    private Material enemyMaterial;

    // The shader property name for emission. 
    // "_EmissionColor" is standard for Unity Standard and URP Lit shaders.
    private const string EMISSION_KEYWORD = "_EmissionColor";

    [Header("Glow Settings")]
    [ColorUsage(true, true)] // Enables the HDR color picker in the Inspector
    public Color hitGlowColor = Color.red * 4f; // Multiplying increases HDR intensity
    public float flashDuration = 0.1f;
    public float fadeDuration = 0.2f;

    void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        // Using .material creates a local instance so other enemies don't flash too
        enemyMaterial = meshRenderer.material;

        // Ensure emission is locally enabled on the material keyword map
        enemyMaterial.EnableKeyword("_EMISSION");
    }

    public void TakeDamage()
    {
        StopAllCoroutines();
        StartCoroutine(GlowRoutine());
    }

    private IEnumerator GlowRoutine()
    {
        // 1. Instantly set emission to the bright HDR color
        enemyMaterial.SetColor(EMISSION_KEYWORD, hitGlowColor);

        // 2. Hold the peak glow for a moment
        yield return new WaitForSeconds(flashDuration);

        // 3. Smoothly fade the emission back to black (off)
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            Color currentGlow = Color.Lerp(hitGlowColor, Color.clear, elapsed / fadeDuration);
            enemyMaterial.SetColor(EMISSION_KEYWORD, currentGlow);
            yield return null;
        }

        // Ensure it resets completely to off
        enemyMaterial.SetColor(EMISSION_KEYWORD, Color.clear);
    }

    private void OnDestroy()
    {
        // Clean up the instantiated material when the enemy is destroyed
        if (enemyMaterial != null) Destroy(enemyMaterial);
    }
}

