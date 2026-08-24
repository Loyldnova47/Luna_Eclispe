using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;

    private Vector3 startPosition;
    private CharacterController controller;

    void Awake()
    {
        startPosition = transform.position;
        controller = GetComponent<CharacterController>();
    }

    public void AddHealth(float amount)
    {
        health += amount;

        if (health > maxHealth)
            health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Respawn();
        }
    }

    void Respawn()
    {
        // Briefly disable the CharacterController so we can move the
        // player's Transform directly without it fighting the teleport
        if (controller != null)
            controller.enabled = false;

        transform.position = startPosition;

        if (controller != null)
            controller.enabled = true;

        health = maxHealth;
    }
}