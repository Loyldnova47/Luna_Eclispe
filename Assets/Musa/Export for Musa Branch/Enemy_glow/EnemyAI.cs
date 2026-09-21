using UnityEngine;
using System.Collections;

public class EnemyAI_Musa : Actor
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

    [Header("Player Health Reward")]
    public int healthRewardOnDeath = 10;

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
        if (isDying) return; // Prevent further damage if already dying
         
        currentHealth -= amount;
        Debug.Log($"<color=orange>[Enemy AI]</color> Health remaining: {currentHealth}/{maxHealth}");

        // Beautiful! This tells your floating health bar canvas to update instantly
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        }

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

        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            // 2. Automated Message: Tells the player object to execute its AddHealth function
            playerObj.SendMessage("AddHealth", (float)healthRewardOnDeath, SendMessageOptions.DontRequireReceiver);
            Debug.Log($"<color=green>[Enemy AI]</color> Message sent: Reward {healthRewardOnDeath} HP to Player!");
        }
        else
        {
            // FIX: Safely closed this else block so it stops breaking your delay loop below!
            Debug.LogWarning("<color=yellow>[Enemy AI]</color> Missing Player! Reward message skipped.");
        }

        // 3. DELAY: Wait a moment to let the player see the glow effect
        yield return new WaitForSeconds(deathDuration); 
                                                        
        // 4. Revert rendering safely just before destruction
        if (meshRenderer != null) meshRenderer.material = originalMaterial;

        // 5. Run the original base.Death() functionality to destroy the game object cleanly
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
