using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractDialogue : MonoBehaviour
{
    [Header("UI (the copied box)")]
    public GameObject dialogueBox;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public TMP_Text hintText;

    [Header("Content")]
    public string characterName = "LUNA";
    [TextArea(2, 4)]
    public string[] lines;
    public string hint = "Press E to close";

    private bool playerInRange = false;
    private bool open = false;
    private int currentLine = 0;

    void Start()
    {
        if (dialogueBox != null) dialogueBox.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        // Don't interfere with the frozen dialogue system
        if (LunaDialogueManager.dialogueIsPlaying) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!open) Open();
            else Next();
        }
    }

    void Open()
    {
        if (lines == null || lines.Length == 0) return;

        open = true;
        currentLine = 0;
        nameText.text = characterName;
        hintText.text = hint;
        dialogueBox.SetActive(true);
        dialogueText.text = lines[currentLine];
    }

    void Next()
    {
        currentLine++;
        if (currentLine >= lines.Length)
            Close();
        else
            dialogueText.text = lines[currentLine];
    }

    void Close()
    {
        open = false;
        dialogueBox.SetActive(false);   // hidden, not destroyed
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (open) Close();   // walking away closes it
    }
}
