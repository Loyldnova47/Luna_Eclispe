using UnityEngine;
using System.Collections;

public class EnemyAI : Actor
{
    private Renderer meshRenderer;
    private Material originalMaterial;

    [Header("Material Swap Settings")]
    [Tooltip("Drag your bright, pre-made glowing red HitGlow_Material here!")]
    public Material hitGlowMaterial;
    public float flashDuration = 0.15f;

    [Header("Death Settings")]
    
    public float deathDuration = 0.15f;

    private bool isDying = false;



    protected override void Awake()
    {
        base.Awake(); // Setup base health from Actor script

        // Find where the actual 3D model/mesh is hiding on this object
        meshRenderer = GetComponentInChildren<Renderer>();

        if (meshRenderer != null)
        {
            // Cache the normal, everyday material the enemy is wearing
            originalMaterial = meshRenderer.sharedMaterial;
        }
        else
        {
            Debug.LogError($"<color=red>[Glow Error]</color> Could not find any Renderer on {gameObject.name} or its children!");
        }
    }

    public override void TakeDamage(int amount)
    {
        // base.TakeDamage(amount); // Automatically drops health value internally
        if (isDying) return; // Prevent further damage if already dying
         
        currentHealth -= amount;
        Debug.Log($"<color=orange>[Enemy AI]</color> Health remainig: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Death(); // Trigger our delayed death routine
            return;
        }

        // Trigger material swap loop if an attack lands and enemy is still alive
        if (meshRenderer != null && hitGlowMaterial != null && currentHealth > 0)
        {
            StopAllCoroutines();
            StartCoroutine(MaterialSwapRoutine());
        }
    }

    private IEnumerator MaterialSwapRoutine()
    {
        // 1. Instantly swap to your bright, pre-made glowing red material asset
        meshRenderer.material = hitGlowMaterial;

        // 2. Hold it on screen for a split second flash
        yield return new WaitForSeconds(flashDuration);

        // 3. Revert back to the normal material safely
        meshRenderer.material = originalMaterial;
    }


    // Completely overrides the base class Death function to handle our delay timer
    protected override void Death()
    {
        if (isDying) return;
        isDying = true;

        StopAllCoroutines();
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        Debug.Log("<color=red>[Enemy AI]</color> Lethal hit landed! Freezing and glowing red...");

        // 1. Force the neon hit material to show the killing blow impact
        if (meshRenderer != null && hitGlowMaterial != null)
        {
            meshRenderer.material = hitGlowMaterial;
        }

        // 2. DELAY: Keep them in the world glowing red for your custom duration
        yield return new WaitForSeconds(deathDuration);

        // 3. Revert rendering safely just before destruction
        if (meshRenderer != null) meshRenderer.material = originalMaterial;

        // 4. Run the original base.Death() functionality to destroy the game object cleanly
        base.Death();
    }

    private void OnDestroy()
    {
        // Clean up to prevent material instance leaks when enemy dies
        if (meshRenderer != null && originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }
    }
}
