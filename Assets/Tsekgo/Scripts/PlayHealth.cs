using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;

    //Respawn point
    private RespawnScript respawn;
    private CharacterController controller;

    void Awake()
    {
        //Note start position for respawn point
        respawn = GameObject.FindGameObjectWithTag("Respawn").GetComponent<RespawnScript>();
        controller = GetComponent<CharacterController>();
    }

    public void AddHealth(float amount)
    {   //Health gets added when Luna kills an enemy
        health += amount;
        //Add health until the max health 
        if (health > maxHealth)
            health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("TakeDamage called, damage: " + damage + ", current health: " + health);
        health -= damage;

        if (health <= 0)
        {   //When health is zero, Luna respawns
            Debug.Log("Health reached zero, calling Respawn()");
            health = 0;
            Respawn();
        }
    }

    void Respawn()
    {
        // Temporarily disable the CharacterController so the charactercontroller can change w/o conflict
        if (controller != null)
            controller.enabled = false;

    //Move to the start
        transform.position = respawn.respawnPoint.transform.position;

        if (controller != null)
            controller.enabled = true;

        //Health resets
        health = maxHealth;
    }
}