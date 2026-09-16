using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput_new playerInput;
    PlayerInput_new.MainActions input;

    CharacterController controller;
    Animator animator;
    AudioSource audioSource;

    [Header("Controller")]
    public float moveSpeed = 5;
    public float gravity = -9.8f;
    public float jumpHeight = 1.2f;

    Vector3 _PlayerVelocity;

    bool isGrounded;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity;

    [Header("Pause Settings UI")]
    [SerializeField] private GameObject pauseMenuUI;

    float xRotation = 0f;

    // Added a reference to lock actions when loaded
    private bool isDead = false;
    private bool isPaused = false;

    // Player health values since player doesn't inherit from Actor
    [Header("Player Health System")]
    public int playerCurrentHealth = 100;
    public int playerMaxHealth = 100;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerInput = new PlayerInput_new();
        input = playerInput.Main;
        AssignInputs();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure the pause menu starts hidden
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (isDead) return; // If the player is dead, skip the update

        isGrounded = controller.isGrounded;

        SetAnimations();
    }

    void FixedUpdate()
    { MoveInput(input.Movement.ReadValue<Vector2>()); }

    void LateUpdate()
    { LookInput(input.Look.ReadValue<Vector2>()); }

    public void DisableControllerOnDeath()
    {
        isDead = true;
        input.Disable(); // Completely shuts off input processing
        _PlayerVelocity = Vector3.zero; // Wipes existing momentum
    }

    public void EnableControllerOnRespawn()
    {
        isDead = false;
        input.Enable(); // Re-enables input processing
        _PlayerVelocity = Vector3.zero; // Wipes existing momentum
        xRotation = 0f; // Resets camera rotation
    }

    void MoveInput(Vector2 inputMovement)
    {
        Vector3 moveDirection = Vector3.zero;

        // If you are swinging, ignore your keyboard movement
        if (!attacking)
        {
            moveDirection.x = inputMovement.x;
            moveDirection.z = inputMovement.y;
            moveDirection = transform.TransformDirection(moveDirection) * moveSpeed;
        }

        controller.Move(moveDirection * Time.deltaTime);

        // Handle Gravity
        _PlayerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && _PlayerVelocity.y < 0)
            _PlayerVelocity.y = -2f;

        controller.Move(_PlayerVelocity * Time.deltaTime);
    }

    void LookInput(Vector3 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime * sensitivity);
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime * sensitivity));
    }

    void OnEnable()
    { if (!isDead) input.Enable(); }

    void OnDisable()
    { if (!isDead) input.Disable(); }

    void Jump()
    {
        if (!isGrounded || attacking) return;

        // Adds force to the player rigidbody to jump
        _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();
        input.Attack.performed += ctx => Attack();

        // Listen for your new Escape button press action map event
        input.Pause.performed += ctx => TogglePause();
    }

    public void TogglePause()
    {
        if (isDead) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;

            if (pauseMenuUI != null)
                pauseMenuUI.SetActive(true);

            //Unlock and reveal mouse cursor options
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("<color=yellow>[PlayerController]</color> Game Paused.");
        }
        else
        {
            // Resume the game simulation status path
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        // Lock and hide mouse cursor options
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("<color=yellow>[PlayerController]</color> Game Resumed.");
    }

    // ---------- //
    // ANIMATIONS //
    // ---------- //

    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack 1";

    string currentAnimationState;

    public void ChangeAnimationState(string newState)
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.0f, 0, 0f);
    }

    void SetAnimations()
    {
        // If player is not attacking
        if (!attacking)
        {
            if (_PlayerVelocity.x == 0 && _PlayerVelocity.z == 0)
            { ChangeAnimationState(IDLE); }
            else
            { ChangeAnimationState(WALK); }
        }
    }

    // ------------------- //
    // ATTACKING BEHAVIOUR //
    // ------------------- //

    [Header("Attacking Timing")]
    [Tooltip("Delay between pressing the key and the weapon striking the target (in seconds). Set to 0 for instant hits.")]
    public float damageRegistryDelay = 0.2f;

    [Tooltip("Total duration of the attack animation (in seconds). This should match the length of the attack animation.")]
    public float totalAttackDuration = 0.6f;
    [Tooltip("Distance the attack can reach.")]
    public float attackDistance = 3f;
    [Tooltip("Delay before the attack can be performed again (in seconds).")]
    public float attackDelay = 0.4f;
    [Tooltip("Speed of the attack animation.")]
    public float attackSpeed = 1f;
    [Tooltip("Amount of damage the attack deals.")]
    public int attackDamage = 1;
    [Tooltip("Layers that the attack can hit.")]
    public LayerMask attackLayer;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    bool attacking = false;
    bool readyToAttack = true;

    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        if (audioSource != null && swordSwing != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(swordSwing);
        }

        ChangeAnimationState(ATTACK1);

        StartCoroutine(AttackTimingRoutine());
        Invoke(nameof(ResetAttack), totalAttackDuration);
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    private IEnumerator AttackTimingRoutine()
    {
        // Wait for your hit timing delay window
        yield return new WaitForSeconds(damageRegistryDelay);

        Debug.Log("<color=cyan>[Raycast Test]</color> AttackRaycast function fired!");
        Debug.DrawRay(cam.transform.position, cam.transform.forward * attackDistance, Color.green, 2.0f);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {
            Debug.Log($"<color=green>[Raycast Test]</color> Raycast physically HIT: {hit.transform.name}");
            HitTarget(hit.point);

            // MATCHED NAME: Pulls EnemyAI_Musa dynamically to align visual frames
            if (hit.transform.TryGetComponent<EnemyAI_Musa>(out EnemyAI_Musa enemy))
            {
                Debug.Log("<color=orange>[Raycast Test]</color> Custom EnemyAI_Musa component matched! Forcing Neon Glow & Camera Shake...");

                StopAllCoroutines();
                StartCoroutine(CameraShakeRoutine());

                // 2. LOGICAL SYSTEM: Process damage data through the target container
                enemy.TakeDamage(attackDamage);
            }
            else if (hit.transform.TryGetComponent<Actor>(out Actor T))
            {
                Debug.Log("<color=yellow>[Raycast Test]</color> Generic Actor component matched. Running standard damage loop.");
                T.TakeDamage(attackDamage);
            }
            else
            {
                Debug.LogWarning($"<color=red>[Raycast Test]</color> Hit {hit.transform.name}, but it lacks an EnemyAI or Actor component!");
            }
        }
        else
        {
            Debug.LogWarning("<color=yellow>[Raycast Test]</color> Raycast missed. Did not collide with any objects on the selected Layer Mask.");
        }
    }

    void HitTarget(Vector3 pos)
    {
        // Hit effect instantiation logic placeholder
    }

    // ---------------------------- //
    // FIRST-PERSON PLAYER FEEDBACK //
    // ---------------------------- //

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        playerCurrentHealth -= amount;
        Debug.Log($"<color=red>[Combat Log]</color> Player took {amount} damage! Current Health: {playerCurrentHealth}");

        if (cam != null && playerCurrentHealth > 0)
        {
            StopCoroutine(nameof(CameraShakeRoutine));


        }
    }

    private IEnumerator CameraShakeRoutine()
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;
        float shakeDuration = 0.15f;
        float shakeIntensity = 0.45f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            cam.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }
}

