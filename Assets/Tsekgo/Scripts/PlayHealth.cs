using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;

    public void AddHealth(float amount)
    {
        health += amount;

        if (health > maxHealth)
            health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health < 0)
            health = 0;
    }
}