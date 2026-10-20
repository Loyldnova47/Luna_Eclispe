using System.Collections;
using UnityEngine;

public class SlamAttack : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    public string attackTrigger = "Attack";

    [Header("Timing")]
    // seconds from the start of the animation to the moment of impact (damage lands here)
    public float telegraphDuration = 0.8f;
    // seconds after impact before the enemy can move or attack again
    public float activeDuration = 0.2f;

    [Header("Area Attack Settings")]
    // attack zone
    public float attackRadius = 5f;
    // damage dealt to player (luna)
    public int damageAmount = 1;
    public LayerMask playerLayer;
    public Transform damageCenter;

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // damage center set to the enemy's position by default
        if (damageCenter == null) damageCenter = transform;
    }

    public void PerformAttack()
    {
        if (isAttacking) return;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        // play the attack animation
        if (animator != null)
            animator.SetTrigger(attackTrigger);

        // wait for the impact moment of the animation
        yield return new WaitForSeconds(telegraphDuration);

        // dealing damage within an area (not directly from the arms)
        DealAreaDamage();

        // short recovery before the enemy can act again
        yield return new WaitForSeconds(activeDuration);

        isAttacking = false;
    }

    private void DealAreaDamage()
    {
        // recognises all colliders on the player area that are within the attack area
        Collider[] hitColliders = Physics.OverlapSphere(damageCenter.position, attackRadius, playerLayer);

        foreach (Collider hit in hitColliders)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = hit.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log($"Player caught in aerial blast! Dealt {damageAmount} damage.");
            }
        }
    }

    // visualises the attack radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 center = damageCenter != null ? damageCenter.position : transform.position;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}