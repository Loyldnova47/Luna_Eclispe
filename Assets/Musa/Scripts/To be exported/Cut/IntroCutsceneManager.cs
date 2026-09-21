using UnityEngine;
using System.Collections;
using TMPro; // Required if you use text mesh pro for dialogue boxes

public class IntroCutsceneManager : MonoBehaviour
{
    public GameObject normalGameplayCamera;

    public GameObject cutsceneZoomCamera;

    public GameObject dialoguePanelUI;

    public TMP_Text dialogueText;
    public float initialZoomDuration = 2.0f;
    

    public float dialogueDisplayDuration = 3.0f;

    [TextArea(3, 5)]
    [Tooltip("Type the intro dialogue line here!")]
    public string introDialogueText = "Breach ! Breach !";

    void Start()
    {
        // Start the cinematic loop sequence immediately on boot
        StartCoroutine(CutsceneSequenceRoutine());
    }

    private IEnumerator CutsceneSequenceRoutine()
    {
        // 1. INPUT LOCK: Freeze Luna's mouse turns and keyboard input arrays instantly
        PlayerController controller = FindFirstObjectByType<PlayerController>();
        if (controller != null)
        {
            controller.DisableControllerOnDeath(); // Safely re-uses your input shutdown locks!
        }

        // 2. CAMERAS SETUP: Activate the close-up zoom view and hide normal gameplay eyes
        if (cutsceneZoomCamera != null) cutsceneZoomCamera.SetActive(true);
        if (normalGameplayCamera != null) normalGameplayCamera.SetActive(false);
        if (dialoguePanelUI != null) dialoguePanelUI.SetActive(false);

        // Hold the view on the object for your custom zoom duration
        yield return new WaitForSeconds(initialZoomDuration);

        // 3. DIALOGUE RUN: Pop open the subtitle pane box and print the line
        if (dialoguePanelUI != null) dialoguePanelUI.SetActive(true);
        if (dialogueText != null) dialogueText.text = introDialogueText;

        // Hold the text display on screen so the player can easily read it
        yield return new WaitForSeconds(dialogueDisplayDuration);

        // 4. UI CLEANUP: Hide the dialogue text box frame elements
        if (dialoguePanelUI != null) dialoguePanelUI.SetActive(false);

        // 5. CAMERA BLEND: Snap cleanly back into standard first-person viewport eyes
        if (normalGameplayCamera != null) normalGameplayCamera.SetActive(true);
        if (cutsceneZoomCamera != null) cutsceneZoomCamera.SetActive(false);

        // 6. RESTORE INPUTS: Give movement control back to Luna!
        if (controller != null)
        {
            controller.EnableControllerOnRespawn(); // Restores input mapping grids flawlessly
        }
        
        Debug.Log("<color=green>[Cutscene System]</color> Intro complete! Control handed back to player loop.");
    }
}
