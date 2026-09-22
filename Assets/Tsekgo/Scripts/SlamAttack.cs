using System.Collections;
using UnityEngine;

public class SlamAttack : MonoBehaviour
{
    //recognises the arms on either side of the enemy
    public GameObject rectangleLeft;
    public GameObject rectangleRight;

    //responsible for timing the frequency of attacks 
    [Header("Timing")]
    public float telegraphDuration = 0.8f;
    public float activeDuration = 0.2f;

    //responsible for the overall slamming movement of the arms 
    [Header("Visual Rotation")]
    public float raisedAngle = -140f;
    public float slammedAngle = 0f;
    public Vector3 rotationAxis = Vector3.right;

    [Header("Area Attack Settings")]
    // attack zone
    public float attackRadius = 5f;         
    //damage dealt to player (luna)
    public int damageAmount = 1;            
    public LayerMask playerLayer;          
    public Transform damageCenter;         
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

        Quaternion leftSlamRot = leftRestRot * Quaternion.AngleAxis(slammedAngle, rotationAxis);
        Quaternion rightSlamRot = rightRestRot * Quaternion.AngleAxis(slammedAngle, rotationAxis);

        // downward swing of arms 
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

       //dealing damage within a area (not directly from the arms)
        DealAreaDamage();

        // keep arms temporarily in slam position
        yield return new WaitForSeconds(activeDuration);

        //the swing moves back up to default position
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
