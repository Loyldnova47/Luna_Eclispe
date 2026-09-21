using System.Collections;
using UnityEngine;

public class SlamAttack : MonoBehaviour
{
    public GameObject rectangleLeft;
    public GameObject rectangleRight;

    [Header("Timing")]
    public float telegraphDuration = 0.8f;
    public float activeDuration = 0.2f;

    [Header("Visual Rotation")]
    public float raisedAngle = -140f;
    public float slammedAngle = 0f;
    public Vector3 rotationAxis = Vector3.right;

    [Header("Area Attack Settings")]
    public float attackRadius = 5f;          // How wide the aerial damage zone is
    public int damageAmount = 1;             // Damage dealt to the player
    public LayerMask playerLayer;            // Set this to your Player layer
    public Transform damageCenter;           // Where the attack originates (defaults to enemy position)

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;

    private Quaternion leftRestRot, rightRestRot;
    private Quaternion leftRaisedRot, rightRaisedRot;

    void Awake()
    {
        leftRestRot = rectangleLeft.transform.localRotation;
        rightRestRot = rectangleRight.transform.localRotation;

        leftRaisedRot = leftRestRot * Quaternion.AngleAxis(raisedAngle, rotationAxis);
        rightRaisedRot = rightRestRot * Quaternion.AngleAxis(raisedAngle, rotationAxis);

        rectangleLeft.transform.localRotation = leftRaisedRot;
        rectangleRight.transform.localRotation = rightRaisedRot;
        
        // If no custom damage center is set, use the enemy's feet/position
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

        Quaternion leftSlamRot = leftRestRot * Quaternion.AngleAxis(slammedAngle, rotationAxis);
        Quaternion rightSlamRot = rightRestRot * Quaternion.AngleAxis(slammedAngle, rotationAxis);

        // 1. Swing DOWN (Telegraph phase)
        float elapsed = 0f;
        while (elapsed < telegraphDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / telegraphDuration;

            rectangleLeft.transform.localRotation = Quaternion.Slerp(leftRaisedRot, leftSlamRot, t);
            rectangleRight.transform.localRotation = Quaternion.Slerp(rightRaisedRot, rightSlamRot, t);

            yield return null;
        }

        rectangleLeft.transform.localRotation = leftSlamRot;
        rectangleRight.transform.localRotation = rightSlamRot;

        // 2. IMPACT MOMENT: Deal Area Damage instantly
        DealAreaDamage();

        // Hold the slammed position briefly
        yield return new WaitForSeconds(activeDuration);

        // 3. Swing back UP to raised/idle position
        elapsed = 0f;
        float returnDuration = telegraphDuration * 0.5f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;

            rectangleLeft.transform.localRotation = Quaternion.Slerp(leftSlamRot, leftRaisedRot, t);
            rectangleRight.transform.localRotation = Quaternion.Slerp(rightSlamRot, rightRaisedRot, t);

            yield return null;
        }

        rectangleLeft.transform.localRotation = leftRaisedRot;
        rectangleRight.transform.localRotation = rightRaisedRot;

        isAttacking = false;
    }

    private void DealAreaDamage()
    {
        // Finds all colliders on the 'playerLayer' within the 'attackRadius'
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

    // Visualises the attack radius in the Unity editor scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 center = damageCenter != null ? damageCenter.position : transform.position;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
