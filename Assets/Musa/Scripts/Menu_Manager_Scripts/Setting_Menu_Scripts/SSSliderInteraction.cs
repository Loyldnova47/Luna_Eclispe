using TMPro;
using UnityEngine;
using UnityEngine.Audio; // Required for AudioMixer
using UnityEngine.UI;

namespace Musa
{
    public class SSSliderInteraction : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string exposedParamName = "MasterVolume"; // Must match your exposed parameter name exactly
        [SerializeField] private bool showDecimalPoints;

        private void Reset()
        {
            slider = GetComponent<Slider>();
            valueText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (slider != null)
            {
                // Force the slider to stay between 0.0001 and 1 for safe logarithmic math
                slider.minValue = 0.0001f;
                slider.maxValue = 1f;

                // Sync the slider value to the Mixer's current runtime volume
                if (audioMixer != null && audioMixer.GetFloat(exposedParamName, out float currentDb))
                {
                    // Convert decibels back to a 0-1 linear value
                    slider.value = Mathf.Pow(10f, currentDb / 20f);
                }
                else
                {
                    slider.value = 1f; // Default fallback
                }

                UpdateTextDisplay(slider.value);
                slider.onValueChanged.AddListener(HandlerSliderValueChanged);
            }
        }

        public void HandlerSliderValueChanged(float value)
        {
            if (audioMixer != null)
            {
                // Convert linear slider 0-1 value to Logarithmic Decibels (-80dB to 0dB)
                float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
                audioMixer.SetFloat(exposedParamName, dB);
            }

            UpdateTextDisplay(value);
        }

        private void UpdateTextDisplay(float value)
        {
            if (valueText == null) return;

            if (showDecimalPoints)
            {
                valueText.text = value.ToString("F2");
            }
            else
            {
                valueText.text = Mathf.RoundToInt(value * 100f).ToString() + "%";
            }
        }

        private void OnDestroy()
        {
            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(HandlerSliderValueChanged);
            }
        }
    }
}

