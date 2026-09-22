using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Screen Panels")]
    [Tooltip("The main container group holding the default buttons (Continue, Settings, etc.)")]
    [SerializeField] private GameObject mainButtonsContainer;

    [Tooltip("The sub-panel holding your audio/visual settings sliders.")]
    [SerializeField] private GameObject settingsPanel;

    [Tooltip("The sub-panel displaying your keyboard/controller layout guide maps.")]
    [SerializeField] private GameObject controlsPanel;

    [Tooltip("The main full-screen menu screen overlay canvas.")]
    [SerializeField] private GameObject mainMenuCanvas;

    private float nextClickTime = 0f;
    private const float CLICK_COOLDOWN = 0.25f; // Fast protection shield duration window

    void Start()
    {
        // Force the main screen layout to initialize completely cleanly on boot
        ShowMainMenuHome();
    }

    // Safeguard verification layer to stop double-click/triple-click anomalies
    private bool IsClickAllowed()
    {
        if (Time.unscaledTime >= nextClickTime)
        {
            nextClickTime = Time.unscaledTime + CLICK_COOLDOWN;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Swaps the view flat into the actual gameplay world loop!
    /// </summary>
    public void ClickStartGame()
    {
        if (!IsClickAllowed()) return;

        Debug.Log("<color=cyan>[Main Menu]</color> Launching gameplay scene sequence...");
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
    }

    /// <summary>
    /// Open the Settings pane panel layout and block out home menu options.
    /// </summary>
    public void OpenSettingsMenu()
    {
        if (!IsClickAllowed()) return;

        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (mainButtonsContainer != null) mainButtonsContainer.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    /// <summary>
    /// Open the Controls instructions pane block out home menu options.
    /// </summary>
    public void OpenControlsMenu()
    {
        if (!IsClickAllowed()) return;

        if (controlsPanel != null) controlsPanel.SetActive(true);
        if (mainButtonsContainer != null) mainButtonsContainer.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Safely backs out of sub-menus and restores your center button stack view!
    /// </summary>
    public void ShowMainMenuHome()
    {
        // FIX: Cleaned out the broken sprite tracking line completely!
        // Now it just safely skips the click cooldown check right at frame zero when starting.
        if (Time.unscaledTime > 0.05f)
        {
            if (!IsClickAllowed()) return;
        }

        if (mainButtonsContainer != null) mainButtonsContainer.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    /// <summary>
    /// Clean execution application close logic for your Quit button asset
    /// </summary>
    public void ClickQuitGame()
    {
        if (!IsClickAllowed()) return;

        Debug.Log("<color=red>[Main Menu]</color> Shutting down game application compiler thread...");
        Application.Quit();
    }
}
