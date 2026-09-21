using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour
{
    [Header("Health Data")]
    public int currentHealth;
    public int maxHealth = 30;

    [Header("UI Reference")]
    [SerializeField] protected Floating_HealthBar healthBar;

    protected virtual void Awake()
    {
       currentHealth = maxHealth;
        
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<Floating_HealthBar>();
        }
    }

    public virtual void start()
    {
      if (healthBar != null)
      {
        healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
      }
      
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took damage! Current Health: {currentHealth}");

        if (healthBar != null)
        {
          healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        }
        else

        {
        Debug.LogError($"{gameObject.name} is missing its healthBar reference!");
        }
        
        if(currentHealth <= 0)
        { 
            Death(); 
        }
    }

    protected virtual void Death()
    {
        // Death function 
        // TEMPORARY: DESTROY OBJECT
        Destroy(gameObject);
    }
}
