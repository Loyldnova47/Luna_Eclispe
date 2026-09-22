using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSchemeManager : MonoBehaviour
{
    public static ControlSchemeManager Instance { get; private set; }

    [SerializeField] private PlayerController playerController;

    void Awake()
    {
        Instance = this;
    }

    public bool IsGamepad()
    {
        if (playerController == null)
            return false;

        var activeControl = playerController.input.Movement.activeControl;
        return activeControl != null && activeControl.device is Gamepad;
    }
}