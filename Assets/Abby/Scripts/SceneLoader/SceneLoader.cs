using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Key nextSceneKey = Key.Space;// Storing Spacebar key for now, whilst keeping editable incase of groupmembers wanting to change key binding.

    InputAction nextAction;

    void Awake()
    {
        nextAction = new InputAction(type: InputActionType.Button, binding: $"<Keyboard>/{nextSceneKey}");// struggling to set a keybinding to the right click on mouse: $"<Mouse>/rightButton" Doesn't work. New input system is also adding on issues recogizing the click as loadtonextscene when clicked instead of next dialogue when "right click is placed in system. I decided to code this due to input system conflicts.  
        nextAction.performed += OnNextActionPerformed;
    }

    void OnEnable()
    {
        nextAction.Enable();
    }

    void OnDisable()
    {
        nextAction.Disable();
    }

    void OnNextActionPerformed(InputAction.CallbackContext ctx)
    {
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;

        if (next >= SceneManager.sceneCountInBuildSettings)
            next = 0;

        SceneManager.LoadScene(next);
    }
}