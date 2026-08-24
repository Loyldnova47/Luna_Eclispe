using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("Doors")] // Categorizing left and Right Door 
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Movement")] // Positioning of Doors and their movement speed 
    public Vector3 leftOpenOffset = new Vector3(-81.1f, 1.52f, -16.35f);
    public Vector3 rightOpenOffset = new Vector3(-76.64f, 1.52f, -16.35f);
    public float slideSpeed = 3f;
    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftTargetPos;
    private Vector3 rightTargetPos;

    private bool isOpen = false;
    private bool playerInRange = false;


    private Interact controls;

    private Interact currentInteract;
    void Awake()
    {
        controls = new Interact();
        controls.DoorInteraction.Button.performed += OnInteractPerformed;
    }

    void OnEnable()
    {
        controls.DoorInteraction.Enable();
    }

    void OnDisable()
    {
        controls.DoorInteraction.Disable();
    }


    void OnDestroy()
    {
        controls.DoorInteraction.Button.performed -= OnInteractPerformed;
    }
    void Start()
    {
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        leftTargetPos = leftClosedPos;
        rightTargetPos = rightClosedPos;
    }
    void Update()
    {

        leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftTargetPos, slideSpeed * Time.deltaTime);
        rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightTargetPos, slideSpeed * Time.deltaTime);
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (playerInRange)
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            leftTargetPos = leftClosedPos + leftOpenOffset;
            rightTargetPos = rightClosedPos + rightOpenOffset;
        }
        else
        {
            leftTargetPos = leftClosedPos;
            rightTargetPos = rightClosedPos;
        }
    }
    void OnTriggerEnter(Collider other)// If player is active withing space of collider, Collider Should open/close with doors. 
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}