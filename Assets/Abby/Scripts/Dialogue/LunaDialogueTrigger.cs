using UnityEngine;

public class LunaDialogueTrigger : MonoBehaviour
{
    [Header("Character")]
    public string characterName; // shows at the top of Luna's box

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string[] lines; // Luna's lines

    [Header("Instructions (optional)")]
    [TextArea(2, 4)]
    public string[] instructionLines; // leave empty if this trigger has no instructions

    [Header("Settings")]
    public bool onlyTriggerOnce = true;

    private bool alreadyTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // only the player should set this off
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        if (onlyTriggerOnce == true && alreadyTriggered == true)
        {
            return;
        }

        alreadyTriggered = true;
        LunaDialogueManager.instance.StartDialogue(characterName, lines, instructionLines);
    }
}