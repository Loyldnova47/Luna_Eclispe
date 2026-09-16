using UnityEngine;
using System.Collections;

public class PlayerActor : Actor
{
    [Header("First-Person Damage Feedback")]
    public Camera playerCamera;
    public float shakeIntensity = 0.3f;
    public float shakeDuration = 0.2f;

    protected override void Awake()
    {
        base.Awake();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerActor: No Camera assigned and no Main Camera found. Camera shake will not work.");
        }
    }

    public override void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"[Combat Log] Player took {amount} damage. Current Health: {currentHealth}/{maxHealth}");

        if (playerCamera != null && currentHealth > 0)
        {
            StopAllCoroutines();
            StartCoroutine(CameraShakeRoutine());
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    private IEnumerator CameraShakeRoutine()
    {
        Vector3 originalPosition = playerCamera.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            //elapsed += Time.deltaTime;
            float xOffset = Random.Range(-1f, 1f) * shakeIntensity;
            float yOffset = Random.Range(-1f, 1f) * shakeIntensity;
            playerCamera.transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);
            yield return null;
        }
        playerCamera.transform.localPosition = originalPosition;
    }

    protected override void Death()
    {
        Debug.Log("Player Defeated! Freeze movement and display local Game Over Menu.");
        // We override this to stop the player object from being instantly destroyed,
        // which would cause the Main Camera to snap or break completely.
    }
}
