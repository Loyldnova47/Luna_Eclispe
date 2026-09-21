using UnityEngine;
using System.Collections;
using TMPro;

public class IntroCutsceneManager : MonoBehaviour
{
    [Header("Camera References")]
    [Tooltip("Drag your standard gameplay first-person camera object here.")]
    public GameObject normalGameplayCamera;

    [Tooltip("Drag a second camera pre-positioned right in front of your zoom object here.")]
    public GameObject cutsceneZoomCamera;

    [Header("Dialogue UI Integration")]
    [Tooltip("Drag your dialogue UI text container panel here.")]
    public GameObject dialoguePanelUI;
    [Tooltip("Drag your TextMeshPro text object here to print lines.")]
    public TMP_Text dialogueText;

    [Header("Cutscene Timing")]
    [Tooltip("How long the camera stays zoomed on the object before the first line pops up.")]
    public float initialZoomDuration = 2.0f;
    
    [Tooltip("How many seconds EACH text line stays on screen before automatically switching.")]
    public float timePerDialogueLine = 3.5f;

    [Header("Interactive Dialogue Lines")]
    [TextArea(3, 5)]
    [Tooltip("Add your three distinct dialogue lines here!")]
    public string[] dialogueLines = new string[]
    {
        "Luna... look closely at this ancient terminal marker loop. Something feels out of context here...",
        "The encrypted data core arrays appear to be completely corrupted by a source override.",
        "We need to find the main security mainframe buffer before the system locks us out completely."
    };

    private int currentLineIndex = 0;

    void Start()
    {
        // Start the cinematic loop sequence automatically on boot
        StartCoroutine(CutsceneSequenceRoutine());
    }

    private IEnumerator CutsceneSequenceRoutine()
    {
        // 1. INPUT LOCK: Shut down Luna's mouse turns and keyboard input arrays instantly
        PlayerController controller = FindFirstObjectByType<PlayerController>();
        if (controller != null)
        {
            controller.DisableControllerOnDeath(); // Re-uses your absolute input locks!
        }

        // 2. CAMERAS SETUP: Activate close-up zoom view and hide normal gameplay eyes
        if (cutsceneZoomCamera != null) cutsceneZoomCamera.SetActive(true);
        if (normalGameplayCamera != null) normalGameplayCamera.SetActive(false);
        if (dialoguePanelUI != null) dialoguePanelUI.SetActive(false);

        // Hold the view on the object for your initial zoom window
        yield return new WaitForSeconds(initialZoomDuration);

        // 3. FRAME FREEZE: Freeze game simulation ticks before displaying text
        Time.timeScale = 0f; 
        Debug.Log("<color=yellow>[Cutscene System]</color> Game time frozen. Beginning automated text transitions.");

        if (dialoguePanelUI != null) dialoguePanelUI.SetActive(true);

        // 4. AUTOMATED DIALOGUE ITERATION LOOP
        currentLineIndex = 0;
        while (currentLineIndex < dialogueLines.Length)
        {
            // Print the current sentence line string to your UI container frame
            if (dialogueText != null) 
            {
                dialogueText.text = dialogueLines[currentLineIndex];
            }

            // TIMER DELAY: Holds the text line visible on screen for your custom duration.
            // Uses WaitForSecondsRealtime so it continues tracking smoothly while engine time scale is 0!
            yield return new WaitForSecondsRealtime(timePerDialogueLine);

            currentLineIndex++;
        }

        // 5. CLEANUP & UNFREEZE: Hide text panel and restore standard engine speed clocks
        if (dialoguePanelUI != null) dialoguePanelUI.SetActive(false);
        
        Time.timeScale = 1f; 

        // 6. CAMERA RESET: Snap cleanly back into standard first-person viewport eyes
        if (normalGameplayCamera != null) normalGameplayCamera.SetActive(true);
        if (cutsceneZoomCamera != null) cutsceneZoomCamera.SetActive(false);

        // 7. RESTORE INPUTS: Give movement and looking control right back to Luna!
        if (controller != null)
        {
            controller.EnableControllerOnRespawn(); 
        }
        
        Debug.Log("<color=green>[Cutscene System]</color> Automated intro complete! Player controls online.");
    }
}
