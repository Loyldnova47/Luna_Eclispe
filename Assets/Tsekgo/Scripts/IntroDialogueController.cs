using UnityEngine;
using UnityEngine.UI;
using TMPro; // remove if you're using legacy Text instead

public class IntroDialogueController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText; // or Text dialogueText;
    [SerializeField] private TMP_Text hintText;      // NEW: the "right click to..." prompt

    [Header("Dialogue Content")]
    [TextArea(2, 4)]
    [SerializeField] private string[] lines;

    [Header("Hint Text")]
    [SerializeField] private string advanceHint = "Right click for next line";
    [SerializeField] private string startHint = "Right click to start playing";

    [Header("Game Start")]
    [SerializeField] private GameObject[] objectsToEnableOnStart;
    [SerializeField] private GameObject[] objectsToDisableOnStart;

    private int currentLine = 0;
    private bool dialogueActive = false;

    private void Start()
    {
        StartDialogue();
    }

    private void StartDialogue()
    {
        dialogueActive = true;
        currentLine = 0;

        dialoguePanel.SetActive(true);
        ShowLine();

        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetMouseButtonDown(1))
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        currentLine++;

        if (currentLine >= lines.Length)
        {
            EndDialogueAndStartGame();
        }
        else
        {
            ShowLine();
        }
    }

    private void ShowLine()
    {
        dialogueText.text = lines[currentLine];

        // NEW: swap hint text depending on whether this is the last line
        bool isLastLine = (currentLine == lines.Length - 1);
        hintText.text = isLastLine ? startHint : advanceHint;
    }

    private void EndDialogueAndStartGame()
    {
        dialogueActive = false;
        dialoguePanel.SetActive(false);

        Time.timeScale = 1f;

        foreach (var obj in objectsToDisableOnStart)
            obj.SetActive(false);

        foreach (var obj in objectsToEnableOnStart)
            obj.SetActive(true);
    }
}