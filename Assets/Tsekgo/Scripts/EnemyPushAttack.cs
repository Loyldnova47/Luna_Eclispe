using UnityEngine;

public class EnemyPushAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Seconds between each pushback attack")]
    [SerializeField] private float attackInterval = 3f;
    [Tooltip("How hard the player is pushed back")]
    [SerializeField] private float pushForce = 15f;
    [Tooltip("Damage dealt by the pushback")]
    [SerializeField] private float damage = 10f;
    [Tooltip("Player must be within this distance to be hit")]
    [SerializeField] private float attackRange = 3f;

    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Animation (for later)")]
    [Tooltip("If true, the timer only triggers an animation; an Animation Event must call DoPushAttack()")]
    [SerializeField] private bool useAnimationEvent = false;
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";

    private float timer;

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer < attackInterval) return;

        // Only attack when the player is close enough
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            timer = attackInterval; // stay "ready" until the player is in range
            return;
        }

        timer = 0f;

        if (useAnimationEvent && animator != null)
            animator.SetTrigger(attackTrigger);   // animation calls DoPushAttack() via event
        else
            DoPushAttack();
    }

    // Public so you can call it from an Animation Event later
    public void DoPushAttack()
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, player.position) > attackRange) return;

        // Push direction: away from the enemy, flattened so the player isn't shoved into the floor
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        dir.Normalize();

        if (player.TryGetComponent(out PlayerPushReceiver receiver))
            receiver.ApplyPush(dir, pushForce);

        if (player.TryGetComponent(out PlayerHealth health))
            health.TakeDamage(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}