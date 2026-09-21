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
    public float mouseSensitivity = 15f;
    public float controllerSensitivity = 120.0f;

    [Header("Pause Settings UI")]
    [SerializeField] private GameObject pauseMenuUI;

    float xRotation = 0f;

    private bool isDead = false;
    private bool isPaused = false;

    [Header("Player Health System")]
    public int playerCurrentHealth = 100;
    public int playerMaxHealth = 100;

    [Header("Footstep Settings")]
    [Tooltip("Drag your walking/footstep audio clip here!")]
    public AudioClip footstepSound;
    [Tooltip("How many seconds between steps while walking.")]
    public float footstepInterval = 0.5f;
    private float footstepTimer = 0f;

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

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = controller.isGrounded;

        SetAnimations();
    }

    void FixedUpdate()
    { MoveInput(input.Movement.ReadValue<Vector2>()); }

    void LateUpdate()
    { 
      if (isDead) return;

      Vector2 lookVector = input.Look.ReadValue<Vector2>();

      var activeControl = input.Look.activeControl;
      bool isController = activeControl != null && activeControl.device is Gamepad;

      LookInput(lookVector, isController);
    }

    public void DisableControllerOnDeath()
    {
        isDead = true;
        input.Disable();
        _PlayerVelocity = Vector3.zero;
    }

    public void EnableControllerOnRespawn()
    {
        isDead = false;
        input.Enable();
        _PlayerVelocity = Vector3.zero;
        xRotation = 0f;
    }

    void MoveInput(Vector2 inputMovement)
    {
        Vector3 moveDirection = Vector3.zero;

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

        // Footstep playback system checks
        if (isGrounded && inputMovement.sqrMagnitude > 0.01f && !isPaused && !isDead)
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepInterval)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = footstepInterval;
        }
    }

    private void PlayFootstep()
    {
        if (audioSource != null && footstepSound != null)
        {
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.volume = Random.Range(0.6f, 0.8f);
            audioSource.PlayOneShot(footstepSound);
        }
    }

    void LookInput(Vector3 input, bool isController)
    {
        float currentSensitivity = isController ? controllerSensitivity : mouseSensitivity;
        
        float mouseX = input.x * Time.deltaTime * currentSensitivity;
        float mouseY = input.y * Time.deltaTime * currentSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80, 80);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * mouseX);
    }

    void OnEnable()
    { if (!isDead) input.Enable(); }

    void OnDisable()
    { if (!isDead) input.Disable(); }

    void AssignInputs()
    {
        input.Attack.performed += ctx => Attack();
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

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("<color=yellow>[PlayerController]</color> Game Paused.");
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
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
        if (currentAnimationState == newState) return;

        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.0f, 0, 0f);
    }

    void SetAnimations()
    {
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
    public float damageRegistryDelay = 0.2f;
    public float totalAttackDuration = 0.6f;
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 10;
    public float hitImpactDelay = 0.2f;
    public LayerMask attackLayer;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    bool attacking = false;
    bool readyToAttack = true;

    public void Attack()
    {
        if (isPaused || Time.timeScale == 0f) return;
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        if (audioSource != null && swordSwing != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.volume = 1.0f;
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
        yield return new WaitForSeconds(damageRegistryDelay);

        Debug.Log("<color=cyan>[Raycast Test]</color> AttackRaycast function fired!");
        Debug.DrawRay(cam.transform.position, cam.transform.forward * attackDistance, Color.green, 2.0f);

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {
            Debug.Log($"<color=green>[Raycast Test]</color> Raycast physically HIT: {hit.transform.name}");

            if (hitEffect != null)
            {
                GameObject sparks = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(sparks, 1.0f);
            }

            if (hit.transform.TryGetComponent<EnemyAI_Musa>(out EnemyAI_Musa enemy))
            {
                Debug.Log("<color=orange>[Raycast Test]</color> Custom EnemyAI_Musa component matched! Forcing Neon Glow & Camera Shake...");
                StartCoroutine(CameraShakeRoutine());
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    void HitTarget(Vector3 pos) { }

    // ---------------------------- //
    // FIRST-PERSON PLAYER FEEDBACK //
    // ---------------------------- //

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        StartCoroutine(CameraShakeRoutine());

        PlayerHealth healthScript = GetComponent<PlayerHealth>();
        if (healthScript != null)
        {
            healthScript.TakeDamage((float)amount);
        }
        else
        {
            FindFirstObjectByType<PlayerHealth>()?.TakeDamage((float)amount);
        }
    }

    private IEnumerator CameraShakeRoutine()
    {
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;
        float shakeDuration = 0.08f;  
        float shakeIntensity = 0.08f; 

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

