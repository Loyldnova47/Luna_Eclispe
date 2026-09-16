using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;

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
