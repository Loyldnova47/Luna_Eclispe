using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LunaDialogueManager : MonoBehaviour
{
    [Header("UI")] // Luna's box
    public GameObject dialogueBox;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public TMP_Text hintText;

    [Header("Instruction Box UI")] // the second box
    public GameObject instructionBox;
    public TMP_Text instructionText;
    public TMP_Text instructionHintText;

    [Header("Name")]
    public string defaultName = "Luna"; // used when a trigger's Character Name is empty

    [Header("Hint Messages")]
    public string nextHint = "Right click to continue";

    [Header("Scripts To Freeze")] // drag the player, enemy etc. scripts in here
    public Behaviour[] scriptsToFreeze;

    // static means other scripts can check these without needing a reference
    public static LunaDialogueManager instance;
    public static bool dialogueIsPlaying = false;

    private string[] lines;
    private string[] instructionLines;
    private int currentLine = 0;
    private bool showingInstructions = false; // true once Luna is done and the second box is up

    // remembers which scripts I switched off so I only switch those back on
    private List<Behaviour> frozenScripts = new List<Behaviour>();

    void Awake()
    {
        instance = this;
        dialogueIsPlaying = false;
        Time.timeScale = 1f;
        dialogueBox.SetActive(false); // both boxes are hidden at the start of the game
        instructionBox.SetActive(false);
    }

    // old version, still works for triggers that have no instructions
    public void StartDialogue(string characterName, string[] newLines)
    {
        StartDialogue(characterName, newLines, null);
    }

    // the trigger calls this when the player walks into it
    public void StartDialogue(string characterName, string[] newLines, string[] newInstructionLines)
    {
        if (dialogueIsPlaying == true)
        {
            return; // already talking so do nothing
        }

        if (newLines == null || newLines.Length == 0)
        {
            return; // no lines were typed in so do nothing
        }

        lines = newLines;
        instructionLines = newInstructionLines;
        currentLine = 0;
        showingInstructions = false;
        dialogueIsPlaying = true;

        // use the default name if the trigger's name was left blank
        if (characterName == "")
        {
            characterName = defaultName;
        }

        // show the name, or hide the name text if there still isn't one
        if (characterName == "")
        {
            nameText.gameObject.SetActive(false);
        }
        else
        {
            nameText.gameObject.SetActive(true);
            nameText.text = characterName;
        }

        dialogueBox.SetActive(true);
        ShowLine();

        FreezeScripts();
        Time.timeScale = 0f; // freezes the whole game
    }

    void Update()
    {
        if (dialogueIsPlaying == false)
        {
            return;
        }

        // right click goes to the next line
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            currentLine = currentLine + 1;

            if (showingInstructions == true)
            {
                // we are in the instruction box
                if (currentLine >= instructionLines.Length)
                {
                    EndDialogue(); // last instruction so the game starts again
                }
                else
                {
                    ShowLine();
                }
            }
            else
            {
                // we are in Luna's box
                if (currentLine >= lines.Length)
                {
                    if (HasInstructions() == true)
                    {
                        StartInstructions(); // Luna is done but the game stays frozen
                    }
                    else
                    {
                        EndDialogue(); // no instructions so this was the end
                    }
                }
                else
                {
                    ShowLine();
                }
            }
        }
    }

    bool HasInstructions()
    {
        return instructionLines != null && instructionLines.Length > 0;
    }

    void StartInstructions()
    {
        showingInstructions = true;
        currentLine = 0;

        dialogueBox.SetActive(false); // hide Luna's box
        instructionBox.SetActive(true); // show the instruction box
        ShowLine();
    }

    void ShowLine()
    {
        if (showingInstructions == true)
        {
            instructionText.text = instructionLines[currentLine];

            instructionHintText.text = nextHint; 
        }
        else
        {
            dialogueText.text = lines[currentLine];

            // only say "close" on Luna's last line if there are no instructions after it
            hintText.text = nextHint;
        }
    }

    void FreezeScripts()
    {
        frozenScripts.Clear();

        // the scripts I dragged in the Inspector
        foreach (Behaviour script in scriptsToFreeze)
        {
            if (script != null && script.enabled == true)
            {
                script.enabled = false;
                frozenScripts.Add(script);
            }
        }
    }

    void UnfreezeScripts()
    {
        foreach (Behaviour script in frozenScripts)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        frozenScripts.Clear();
    }

    void EndDialogue()
    {
        dialogueIsPlaying = false;
        showingInstructions = false;
        dialogueBox.SetActive(false);
        instructionBox.SetActive(false);

        UnfreezeScripts();
        Time.timeScale = 1f; // game starts moving again
    }

    // just in case the scene reloads while the game is frozen
    void OnDestroy()
    {
        dialogueIsPlaying = false;
        Time.timeScale = 1f;
    }
}