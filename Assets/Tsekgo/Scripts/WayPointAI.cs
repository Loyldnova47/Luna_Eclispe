using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WayPointAI : Actor
{
    [Header("AI Navigation")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Patrolling Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex;
    public float waypointTolerance = 1f;

    [Header("Attacking Logic")]
    public float timeBetweenAttacks = 1.5f;
    private bool alreadyAttacked;
    public float attackDamage = 15f; // The damage dealt to Luna when hit!

    [Header("AI States")]
    public float sightRange = 10f;
    public float attackRange = 2f;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Material Swap Juice")]
    [Tooltip("Drag your bright, pre-made glowing red HitGlow_Material here!")]
    public Material hitGlowMaterial;
    public float flashDuration = 0.15f;

    [Header("Death Settings")]
    public float deathDuration = 0.35f; // Holds the 3rd flash frame perfectly
    private bool isDying = false;

    [Header("Player Health Reward")]
    public int healthRewardOnDeath = 10;

    private Renderer meshRenderer;
    private Material originalMaterial;
    private SlamAttack slamAttack; // NEW ATTACK COMPONENT COUPLING

    protected override void Awake()
    {
        base.Awake(); // Setup baseline max health from Actor script

        // Automatically find Luna by her name in the scene
        GameObject Luna = GameObject.Find("Luna");
        if (Luna != null)
        {
            player = Luna.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        slamAttack = GetComponent<SlamAttack>(); // Cache the new SlamAttack component

        // Cache the active 3D model skin to handle the neon flash
        meshRenderer = GetComponentInChildren<Renderer>();
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.sharedMaterial;
        }
    }

    public virtual void Start()
    {
        // Force the floating health bar slider to start completely full
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        }
    }

    private void Update()
    {
        if (isDying) return; // Freeze behavior tracking completely if dead

        // Check for sight and attack range using your overlap sphere matrices
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // State controller map loops
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (waypoints == null || waypoints.Length == 0 || agent == null || !agent.isOnNavMesh) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        agent.SetDestination(targetWaypoint.position);

        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);

        if (distanceToWaypoint < waypointTolerance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void ChasePlayer()
    {
        if (player == null || agent == null || !agent.isOnNavMesh) return;

        // If the enemy is busy performing a slam attack, freeze navigation tracking
        if (slamAttack != null && slamAttack.IsAttacking)
        {
            agent.SetDestination(transform.position);
            return;
        }

        agent.SetDestination(player.position);
    }

    public void AttackPlayer()
    {
        if (player == null || agent == null || !agent.isOnNavMesh) return;

        // Keep enemy still when attacking Luna 
        agent.SetDestination(transform.position);

        if (!alreadyAttacked && (slamAttack == null || !slamAttack.IsAttacking))
        {
            // Level out look calculations so enemy stays standing vertical
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z)); 

            // 1. TRIGGER SLAM VISUAL ANIMATIONS
            if (slamAttack != null)
            {
                slamAttack.PerformAttack();
            }

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                //playerHealth.TakeDamage(attackDamage);

                PlayerController controller = player.GetComponent<PlayerController>();
                if (controller != null)
                {
                   // controller.TakeDamage(0); 
                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // Overrides standard damage system to process visual red flashes
    public override void TakeDamage(int amount)
    {
        if (isDying) return;

        currentHealth -= amount;
        Debug.Log($"<color=orange>[WayPoint AI]</color> Took hit! Health remaining: {currentHealth}/{maxHealth}");

        // Update your floating health bar slider canvas instantly on hit
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar((float)currentHealth, (float)maxHealth);
        }

        if (currentHealth <= 0)
        {
            Death(); // Go to our delayed freeze sequence
            return;
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

        if (agent != null && agent.isOnNavMesh) agent.isStopped = true; // Stop AI navigation physics

        StopAllCoroutines();
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        Debug.Log("<color=red>[WayPoint AI]</color> Lethal hit landed! Entering delayed death sequence...");

        if (meshRenderer != null && hitGlowMaterial != null)
        {
            meshRenderer.material = hitGlowMaterial;
        }

        // REWARD LUNA: Safely call AddHealth on your player reference
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.AddHealth(healthRewardOnDeath);
            }
        }

        yield return new WaitForSeconds(deathDuration);

        if (meshRenderer != null) meshRenderer.material = originalMaterial;

        base.Death();
    }

    private void OnDestroy()
    {
        if (meshRenderer != null && originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);

                Transform nextWaypoint = waypoints[(i + 1) % waypoints.Length];
                if (nextWaypoint != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, nextWaypoint.position);
                }
            }
        }
    }
}
