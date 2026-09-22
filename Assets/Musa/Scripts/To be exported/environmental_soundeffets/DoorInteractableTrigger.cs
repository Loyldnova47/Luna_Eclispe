using UnityEngine;
using TMPro;

public class DoorInteractableTrigger : MonoBehaviour
{
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string gamepadPrompt = "Press Triangle to Open";
    public string keyboardPrompt = "Press E to Open";
    public float interactRange = 2.5f;

    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            promptUI.SetActive(true);
            promptText.text = ControlSchemeManager.Instance.IsGamepad() ? gamepadPrompt : keyboardPrompt;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            promptUI.SetActive(false);
        }
    }
}