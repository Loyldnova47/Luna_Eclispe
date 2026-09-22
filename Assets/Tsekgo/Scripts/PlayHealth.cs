using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("UI References")]
    [Tooltip("Drag your screen's green/red Health Slider UI object here!")]
    [SerializeField] private Slider healthSlider; 
    [Tooltip("Drag your Game Over / Death Screen UI Panel object here!")]
    [SerializeField] private GameObject deathScreenUI;

    [Header("First-Person Screen Flash Integration")]
    [Tooltip("Drag the object containing your DamageVisuals component script here!")]
    [SerializeField] private DamageVisuals damageVisuals; 

    // ================================================================
    // AUDIO FEEDBACK: HURT SOUND EFFECT SLOTS
    // ================================================================
    [Header("Audio Feedback")]
    [Tooltip("Drag your player hurt/grunt/impact audio clip here!")]
    [SerializeField] private AudioClip hurtSoundEffect;
    private AudioSource localAudioSource;
    // ================================================================

    [Header("Death Screen Settings")]
    [Tooltip("How long the game stays frozen on the Death Screen before you pop back at spawn.")]
    [SerializeField] private float respawnDelay = 3f; 

    // Respawn point reference trackers
    private RespawnScript respawn;
    private CharacterController controller;
    private PlayerController playerMovement;
    private bool isDead = false;

    void Awake()
    {
        GameObject respawnObj = GameObject.FindGameObjectWithTag("Respawn");
        if (respawnObj != null)
        {
            respawn = respawnObj.GetComponent<RespawnScript>();
        }
        
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerController>(); 

        // Automatically find or add an AudioSource component on Luna to handle the sound playback
        localAudioSource = GetComponent<AudioSource>();
        if (localAudioSource == null)
        {
            localAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (damageVisuals == null)
        {
            damageVisuals = GetComponent<DamageVisuals>();
        }

        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);
    }

    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health; 
        }
    }

    public void AddHealth(float amount)
    {   
        if (isDead) return; 

        health += amount;
        
        if (health > maxHealth)
            health = maxHealth;

        if (healthSlider != null)
            healthSlider.value = health;

        if (damageVisuals != null)
        {
            damageVisuals.TriggerHealFlash();
        }
            
        Debug.Log($"<color=green>[PlayerHealth]</color> Healed! Current health: {health}/{maxHealth}");
    }

        public void TakeDamage(float damage)
    {
        if (isDead) return; 

        health -= damage;

        if (healthSlider != null)
            healthSlider.value = health;

        if (damageVisuals != null && health > 0)
        {
            damageVisuals.TriggerDamageFlash();
        }

        // ================================================================
        // CLEAN HIT FEEDBACK (No Infinite Loops!)
        // ================================================================
        // Directly calls the camera shake function inside PlayerController 
        // using SendMessage so it NEVER bounces back here!
        SendMessage("CameraShakeRoutine", SendMessageOptions.DontRequireReceiver);

        if (localAudioSource != null && hurtSoundEffect != null && health > 0)
        {
            localAudioSource.pitch = Random.Range(0.92f, 1.08f);
            localAudioSource.PlayOneShot(hurtSoundEffect);
        }
        // ================================================================

        Debug.Log($"<color=orange>[PlayerHealth]</color> Damage taken cleanly! Current health: {health}/{maxHealth}");

        if (health <= 0)
        {   
            health = 0;
            HandleDeathSequence();
        }
    }


    void HandleDeathSequence()
    {
        isDead = true;
        Debug.LogWarning("<color=red>[PlayerHealth]</color> Player health hit zero! Triggering Death Screen...");

        if (deathScreenUI != null)
            deathScreenUI.SetActive(true);

        if (playerMovement != null)
            playerMovement.TriggerShake();

        Time.timeScale = 0f; 

        StartCoroutine(RespawnCountdownRoutine());
    }

    private IEnumerator RespawnCountdownRoutine()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        ExecuteRespawnReset();
    }

    void ExecuteRespawnReset()
    {
        Debug.Log("<color=magenta>[PlayerHealth]</color> Respawn timer finished. Restoring player baseline settings.");

        Time.timeScale = 1f;

        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);

        if (controller != null)
            controller.enabled = false;

        if (respawn != null && respawn.respawnPoint != null)
        {
            transform.position = respawn.respawnPoint.transform.position;
        }

        if (controller != null)
            controller.enabled = true;

        if (playerMovement != null)
            playerMovement.EnableControllerOnRespawn();

        health = maxHealth;

        if (healthSlider != null)
            healthSlider.value = health;

        isDead = false;
    }
}

