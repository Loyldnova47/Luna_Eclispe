using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("UI References")]
    [SerializeField] private GameObject deathScreenUI;
    [SerializeField] private UnityEngine.UI.Slider healthSlider; // Slot for your layout UI slider

    [Header("Settings")]
    [SerializeField] private float respawnDelay = 3f; // Delay before respawning after death
    private Vector3 startPosition;
    private CharacterController controller;

    private PlayerController playerMovement;
    private bool isDead = false;

    void Awake()
    {
        startPosition = transform.position;
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerController>(); // Reference to the PlayerController script

        // Ensure the death screen UI is initially inactive
        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);

        Debug.Log($"<color=cyan>[PlayerHealth]</color> Initialized. Starting Position: {startPosition}. Health: {health}/{maxHealth}");
    }

    void Start()
    {
        // Initialize health to maxHealth at the start
        health = maxHealth;
        // Update the health slider if it's assigned
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }
           

    public void AddHealth(float amount)
    {
        // Safe-check to ensure we don't heal a dead player
        if (amount < 0) amount = Mathf.Abs(amount);

        float oldHealth = health;


        //    if (health > maxHealth)
        //        health = maxHealth;

       health = Mathf.Clamp(health + amount, 0f, maxHealth);

        if (healthSlider != null)
            healthSlider.value = health;

        Debug.Log($"<color=green>[PlayerHealth] HEALED!</color> Gained +{amount} HP. Health: {oldHealth} -> {health}/{maxHealth}");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // Player is already dead, no need to take more damage
#if UNITY_EDITOR
        UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= UnityEditor.EditorUtility.ClearProgressBar; // safe-check helper
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor");
        var clearMethod = logEntries?.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod?.Invoke(null, null);
#endif 

        float oldHealth = health; 
        health -= damage;

        //Force update the health slider if it's assigned
        if (healthSlider != null)
            healthSlider.value = health;
        // This will now always be the only message on screen!
        Debug.Log($"<color=orange>[PlayerHealth] DAMAGE TAKEN!</color> Lost -{damage} HP. Health: {oldHealth} -> {Mathf.Max(0, health)}/{maxHealth}");

        if (health <= 0)
        {
            health = 0;
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        isDead = true;

        Debug.LogWarning("<color=red>[PlayerHealth] PLAYER DIED!</color> Freezing game time and displaying death screen.");


        // Show the death screen UI
        if (deathScreenUI != null)
            deathScreenUI.SetActive(true);
        // Optionally, you can also disable player controls here
        // For example, if you have a PlayerController script, you can disable it:
        // GetComponent<PlayerController>().enabled = false;
       
        // Lock movement and camera inputs immediately 
        if (playerMovement != null)
            playerMovement.DisableControllerOnDeath();

        Time.timeScale = 0f; // Pause the game
        
        StartCoroutine(RespawnCountdown());
    }

    private IEnumerator RespawnCountdown()
    {
        Debug.Log($"<color=yellow>[PlayerHealth]</color> Respawn timer started via unscaled realtime. Waiting {respawnDelay} seconds...");

        //Wait for the specified amount of seconds
        yield return new WaitForSecondsRealtime(respawnDelay);

        // Trigger the actual respawn logic
        ExecuteRespawn();
    }

    private void ExecuteRespawn()
    {
         // DEBUG: Confirms the transition to resetting the player
        Debug.Log("<color=magenta>[PlayerHealth]</color> Timer complete. Unfreezing time and resetting player status.");


        // Unfreeze time first so everything can move again
        Time.timeScale = 1f;
        // 1. Instantly hide the UI first to prevent visual lag
        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);




        // Briefly disable the CharacterController so we can move the
        // player's Transform directly without it fighting the teleport
        if (controller != null)
            controller.enabled = false;

        transform.position = startPosition;

        if (controller != null)
            controller.enabled = true;

        // Unlock movement and camera inputs once responsive
        if (playerMovement != null)
            playerMovement.EnableControllerOnRespawn();

        // Reset heatlth status varaibles 
        health = maxHealth;
        if (healthSlider != null)
            healthSlider.value = health;

        isDead = false;

        // Hide the death screen UI
        //if (deathScreenUI != null)
        //    deathScreenUI.SetActive(false);

        //health = maxHealth;
        //isDead = false;

        //if (deathScreenUI != null && deathScreenUI.activeSelf)
        //    deathScreenUI.SetActive(true);
    }
}