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

        //range takes note of where the player is 
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // attacks happen via slamming
    private SlamAttack slamAttack; 

    private void Awake()
    {
        // enemies have objective to find the player (luna)
        GameObject Luna = GameObject.Find("Luna");
        if (Luna != null)
            player = Luna.transform;

        
        agent = GetComponent<NavMeshAgent>();
        slamAttack = GetComponent<SlamAttack>(); 
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    //responsible for controlling roaming activity between way points 
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
        //player gets faced during attacks 
        transform.LookAt(player); 

        if (slamAttack != null)
            slamAttack.PerformAttack();

        //when the player is slammed, the logic resets to continue attacking between incraments of time
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

                //player gets rewarded with hp for killing enemy (glorp)
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