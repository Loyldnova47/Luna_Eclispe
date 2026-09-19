using UnityEngine;
using UnityEngine.InputSystem;

public class ExitGame : MonoBehaviour
{
    [SerializeField] Key quitKey = Key.Escape;// Escape to Quit. Pausing is going to need to be implemented into Next Prototype or It's going to need to be a button. Consult with group. 

    InputAction quitAction;

    void Awake()
    {
        quitAction = new InputAction(type: InputActionType.Button, binding: $"<Keyboard>/{quitKey}");
        quitAction.performed += OnQuitPressed;
    }

    void OnEnable()
    {
        quitAction.Enable();
    }

    void OnDisable()
    {
        quitAction.Disable();
    }

    void OnQuitPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("quitting game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}