using UnityEngine;

public class LunaAnimationController : MonoBehaviour
{
    [Header("Animator Reference")]
    [SerializeField] private Animator lunaAnimator;

    [Header("Settings")]
    [SerializeField] private float walkThreshold = 0.1f;

    private Vector3 lastPosition;
    private bool isDead = false;

    void Start()
    {
        // Automatically grabs the Animator from Luna_Animations+Model if not assigned
        if (lunaAnimator == null)
        {
            lunaAnimator = GetComponentInChildren<Animator>();
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        if (isDead || lunaAnimator == null) return;

        // 1. Calculate horizontal movement speed regardless of movement script type
        Vector3 horizontalMovement = new Vector3(
            transform.position.x - lastPosition.x,
            0f,
            transform.position.z - lastPosition.z
        );
        float currentSpeed = horizontalMovement.magnitude / Time.deltaTime;
        lastPosition = transform.position;

        // 2. Toggle IsWalking and IsIdle booleans based on movement
        bool walking = currentSpeed > walkThreshold;
        lunaAnimator.SetBool("IsWalking", walking);
        lunaAnimator.SetBool("IsIdle", !walking);

        // 3. Example Attack Input (Left Mouse Click)
        if (Input.GetMouseButtonDown(0))
        {
            TriggerAttack();
        }
    }

    // Call this when the player attacks
    public void TriggerAttack()
    {
        if (isDead) return;
        lunaAnimator.SetBool("IsAttacking", true);
        
        // Automatically turns off IsAttackIn after 0.5 seconds so it can return to Idle
        CancelInvoke(nameof(ResetAttack));
        Invoke(nameof(ResetAttack), 0.5f);
    }

    private void ResetAttack()
    {
        lunaAnimator.SetBool("IsAttacking", false);
    }

    // Call this from your health script when Luna takes damage
    public void TriggerHit()
    {
        if (isDead) return;
        lunaAnimator.SetBool("IsHitting", true);

        CancelInvoke(nameof(ResetHit));
        Invoke(nameof(ResetHit), 0.4f);
    }

    private void ResetHit()
    {
        lunaAnimator.SetBool("IsHitting", false);
    }

    // Call this from your health script when Luna's health reaches 0
    public void TriggerDeath()
    {
        isDead = true;
        lunaAnimator.SetBool("IsWalking", false);
        lunaAnimator.SetBool("IsIdle", false);
        lunaAnimator.SetBool("IsAttacking", false);
        lunaAnimator.SetBool("IsHitting", false);
        lunaAnimator.SetBool("IsDying", true);
    }
}
