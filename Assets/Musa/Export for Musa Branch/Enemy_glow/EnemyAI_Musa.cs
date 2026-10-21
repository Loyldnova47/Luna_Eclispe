using UnityEngine;
using System.Collections;

public class EnemyAI_Musa : Actor
{
    private Renderer meshRenderer;
    private Material originalMaterial;
    private AudioSource audioSource;

    [Header("Material Swap Settings")]
    [Tooltip("Drag your bright, pre-made glowing red HitGlow_Material here!")]
    public Material hitGlowMaterial;
    public float flashDuration = 0.15f;

    [Header("Death Settings")]
    public float deathDuration = 0.5f; // Increased slightly so the sound/effect has time to play!
    [Tooltip("Drag your custom Enemy Death sound effect clip here!")]
    public AudioClip deathSoundClip; 

    private bool isDying = false;

    [Header("Player Health Reward")]
    public int healthRewardOnDeath = 10;

    protected override void Awake()
    {
        base.Awake(); 

        audioSource = GetComponent<AudioSource>();

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            if (r.enabled && r.gameObject != this.gameObject)
            {
                meshRenderer = r;
                break;
            }
        }

        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<Renderer>();
        }

        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.sharedMaterial;
        }
    }

    public override void TakeDamage(int amount)
    {
        if (isDying) return; 
         
        currentHealth -= amount;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        }

        if (currentHealth <= 0)
        {
            Death(); 
            return;
        }

        // Play the normal hit sound effect only if they survived the hit
        if (audioSource != null && audioSource.clip != null && currentHealth > 0)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        if (meshRenderer != null && hitGlowMaterial != null && currentHealth > 0)
        {
            StopAllCoroutines();
            StartCoroutine(MaterialSwapRoutine());
        }
    }

    private IEnumerator MaterialSwapRoutine()
    {
        meshRenderer.material = hitGlowMaterial;
        yield return new WaitForSeconds(flashDuration);
        meshRenderer.material = originalMaterial;
    }

    protected override void Death()
    {
        if (isDying) return;
        isDying = true;

        StopAllCoroutines();
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        Debug.Log("<color=red>[Enemy AI]</color> Lethal hit landed! Executing death sequence...");

        // 1. Play the dedicated Death Sound Effect
        if (audioSource != null && deathSoundClip != null)
        {
            audioSource.PlayOneShot(deathSoundClip);
        }

        // 2. Disable moving logic so they freeze/stop chasing the player while dying
        var navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent != null) navMeshAgent.enabled = false;

        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false; // Prevents the player from bumping into a dead body

        // 3. Force the neon hit material to flash
        if (meshRenderer != null && hitGlowMaterial != null)
        {
            meshRenderer.material = hitGlowMaterial;
        }

        // 4. Reward the player
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerObj.SendMessage("AddHealth", (float)healthRewardOnDeath, SendMessageOptions.DontRequireReceiver);
        }

        // 5. DELAY: Wait to let the player see the flash and hear the death sound finish
        yield return new WaitForSeconds(deathDuration); 
                                                        
        if (meshRenderer != null) meshRenderer.material = originalMaterial;

        // 6. Cleanly destroy the enemy object
        base.Death();
    }

    private void OnDestroy()
    {
        if (meshRenderer != null && originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }
    }
}
