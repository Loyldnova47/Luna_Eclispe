using UnityEngine;

public class PlayerPushReceiver : MonoBehaviour
{
    [Tooltip("How quickly the push fades out (higher = shorter slide)")]
    [SerializeField] private float pushDecay = 6f;

    private CharacterController controller;
    private Rigidbody rb;
    private Vector3 pushVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyPush(Vector3 direction, float force)
    {
        if (rb != null && !rb.isKinematic)
            rb.AddForce(direction * force, ForceMode.VelocityChange);
        else
            pushVelocity += direction * force;
    }

    private void Update()
    {
        if (controller == null) return;

        if (pushVelocity.sqrMagnitude < 0.01f)
        {
            pushVelocity = Vector3.zero;
            return;
        }

        controller.Move(pushVelocity * Time.deltaTime);
        pushVelocity = Vector3.Lerp(pushVelocity, Vector3.zero, pushDecay * Time.deltaTime);
    }
}