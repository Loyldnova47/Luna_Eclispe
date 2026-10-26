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

    [Header("Instruction Box UI")] 
    public GameObject instructionBox;
    public TMP_Text instructionText;
    public TMP_Text instructionHintText;

    [Header("Name")]
    public string defaultName = "LUNA"; 

    [Header("Hint Messages")]
    public string nextHint = "Right click to continue";

    [Header("Scripts To Freeze")] // Scripts I wanna freeze so that the game completely freezes (done in inspector)
    public Behaviour[] scriptsToFreeze;

    
    public static LunaDialogueManager instance;
    public static bool dialogueIsPlaying = false;

    private string[] lines;
    private string[] instructionLines;
    private int currentLine = 0;
    private bool showingInstructions = false; 

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

    
    public void StartDialogue(string characterName, string[] newLines)
    {
        StartDialogue(characterName, newLines, null);
    }

    
    public void StartDialogue(string characterName, string[] newLines, string[] newInstructionLines)
    {
        if (dialogueIsPlaying == true)
        {
            return; 
        }

        if (newLines == null || newLines.Length == 0)
        {
            return;
        }

        lines = newLines;
        instructionLines = newInstructionLines;
        currentLine = 0;
        showingInstructions = false;
        dialogueIsPlaying = true;

        
        if (characterName == "")
        {
            characterName = defaultName;
        }

        
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
        Time.timeScale = 0f; // freezes the whole game so that nothing moves
    }

    void Update()
    {
        if (dialogueIsPlaying == false)
        {
            return;
        }

        
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            currentLine = currentLine + 1;

            if (showingInstructions == true)
            {
                
                if (currentLine >= instructionLines.Length) // Instruction Box is actuve 
                {
                    EndDialogue(); 
                }
                else
                {
                    ShowLine();
                }
            }
            else
            {
                
                if (currentLine >= lines.Length) // Luna Box is Active 
                {
                    if (HasInstructions() == true)
                    {
                        StartInstructions(); // Instructions ae right after dialogue hence freeze isn't activated yet 
                    }
                    else
                    {
                        EndDialogue(); // Uf there are no instructions after the dialogue then the Dialoue immediately ends
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

        dialogueBox.SetActive(false); 
        instructionBox.SetActive(true); 
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

            
            hintText.text = nextHint;
        }
    }

    void FreezeScripts()
    {
        frozenScripts.Clear();

        
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
        Time.timeScale = 1f; // game is unfrozen
    }

    
    void OnDestroy()
    {
        dialogueIsPlaying = false;
        Time.timeScale = 1f;
    }
}