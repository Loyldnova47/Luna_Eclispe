using UnityEngine;

public class LunaDialogueTrigger : MonoBehaviour
{
    [Header("Character")]
    public string characterName; 

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string[] lines; 

    [Header("Instructions (optional)")]
    [TextArea(2, 4)]
    public string[] instructionLines; 

    [Header("Settings")]
    public bool onlyTriggerOnce = true;

    private bool alreadyTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        
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