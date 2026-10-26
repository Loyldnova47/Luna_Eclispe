using TMPro; // Added to fix the TextMeshProUGUI compiler error
using UnityEngine;
using UnityEngine.UI;

public class Brightness_Slider : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup blackImageCanvasGroup; // Reference to the CanvasGroup of the black image
    public Slider brightnessSlider; // Reference to the Slider component
    [SerializeField] private TextMeshProUGUI valueText; // Displays the text value

    [Header("Settings")]
    public float minBrightness = 0.1f; // Minimum brightness (fully dark)
    public float maxBrightness = 1.0f; // Maximum brightness (fully bright)
    public float defaultBrightness = 1.0f; // Default brightness value
    [SerializeField] private bool showDecimalPoints = false; // Toggle to switch text styles

    private const string PREF_KEY = "Brightness";

    private void Start()
    {
        // Load saved value or fallback to default
        float savedBrightness = PlayerPrefs.GetFloat(PREF_KEY, defaultBrightness);

        // Initialize slider position if assigned
        if (brightnessSlider != null)
        {
            brightnessSlider.minValue = minBrightness;
            brightnessSlider.maxValue = maxBrightness;
            brightnessSlider.value = savedBrightness;

            // Listen for real-time slider changes
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        // Apply initial visual state on start up
        SetBrightness(savedBrightness, savePref: false);
    }

    public void SetBrightness(float rawValue)
    {
        SetBrightness(rawValue, savePref: true);
    }

    private void SetBrightness(float rawValue, bool savePref)
    {
        // Clamp the input value safely
        float clampedValue = Mathf.Clamp(rawValue, minBrightness, maxBrightness);

        // Map brightness to alpha: 
        // At maxBrightness (1.0), alpha = 0 (transparent)
        // At minBrightness (0.1), alpha = 0.9 (dark)
        float alpha = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(minBrightness, maxBrightness, clampedValue));

        if (blackImageCanvasGroup != null)
        {
            blackImageCanvasGroup.alpha = alpha;
        }

        // Handle Text Display formatting
        UpdateTextDisplay(clampedValue);

        if (savePref)
        {
            PlayerPrefs.SetFloat(PREF_KEY, clampedValue);
            PlayerPrefs.Save();
        }
    }

    private void UpdateTextDisplay(float value)
    {
        if (valueText == null) return;

        if (showDecimalPoints)
        {
            valueText.text = value.ToString("F2"); // Displays decimal string e.g., "0.85"
        }
        else
        {
            // Converts 0.1-1.0 range into a normalized 0% - 100% percentage display
            float percentage = Mathf.InverseLerp(minBrightness, maxBrightness, value) * 100f;
            valueText.text = Mathf.RoundToInt(percentage).ToString() + "%";
        }
    }

    private void OnDestroy()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveListener(SetBrightness);
        }
    }
}
