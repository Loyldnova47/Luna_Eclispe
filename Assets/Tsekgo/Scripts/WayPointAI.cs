using UnityEngine;
using UnityEngine.AI;

public class WayPointAI : Actor
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public int healthRewardOnDeath = 2;

    public Transform[] waypoints;
    private int currentWaypointIndex;
    public float waypointTolerance = 1f;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private SlamAttack slamAttack; // NEW

    private void Awake()
    {
        GameObject Luna = GameObject.Find("Luna");
        if (Luna != null)
            player = Luna.transform;

        agent = GetComponent<NavMeshAgent>();
        slamAttack = GetComponent<SlamAttack>(); // NEW
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (waypoints == null || waypoints.Length == 0) return;

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
        if (slamAttack !=null && slamAttack.IsAttacking)
        {
            agent.SetDestination(transform.position);
            return;
        }

        agent.SetDestination (player.position);
        
    }

    public void AttackPlayer()
    {
        agent.SetDestination(transform.position);

          if (!alreadyAttacked && (slamAttack == null || !slamAttack.IsAttacking))
    {
        transform.LookAt(player); 

        if (slamAttack != null)
            slamAttack.PerformAttack();

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
}

    public void ResetAttack()
    {
        alreadyAttacked = false;
    }

    protected override void Death()
    {
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.AddHealth(healthRewardOnDeath);
        }

        base.Death();
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
                    Gizmos.DrawLine(waypoints[i].position, nextWaypoint.position);
            }
        }
    }
}