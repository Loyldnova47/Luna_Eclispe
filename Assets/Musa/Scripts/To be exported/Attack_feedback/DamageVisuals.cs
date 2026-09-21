using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageVisuals : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image damageImage;
    [SerializeField] private Image healImage; // Added for healing image support

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] [Range(0f, 1f)] private float maxAlpha = 0.3f;

    private Coroutine flashCoroutine;

    private void Start()
    {
        // Automatically hide both images when the game starts
        ResetAlpha(damageImage);
        ResetAlpha(healImage);
    }

    private void ResetAlpha(Image img)
    {
        if (img != null)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
    }

    // Call this for taking damage
    public void TriggerDamageFlash()
    {
        TriggerFlash(damageImage);
    }

    // Call this for getting healed
    public void TriggerHealFlash()
    {
        TriggerFlash(healImage);
    }

    private void TriggerFlash(Image targetImage)
    {
        if (targetImage == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            // Cleanly force both to invisible if another flash overrides it mid-way
            ResetAlpha(damageImage);
            ResetAlpha(healImage);
        }
        
        // Pass the chosen image into the sequence
        flashCoroutine = StartCoroutine(FlashSequence(targetImage));
    }

    // The sequence now dynamically accepts whichever image you tell it to flash
    private IEnumerator FlashSequence(Image targetImage)
    {
        Color origColor = targetImage.color;
        float elapsed = 0f;

        float fadeInDuration = flashDuration * 0.3f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            origColor.a = Mathf.Lerp(0f, maxAlpha, elapsed / fadeInDuration);
            targetImage.color = origColor;
            yield return null;
        }

        elapsed = 0f;
        float fadeOutDuration = flashDuration * 0.7f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            origColor.a = Mathf.Lerp(maxAlpha, 0f, elapsed / fadeOutDuration);
            targetImage.color = origColor;
            yield return null;
        }

        origColor.a = 0f;
        targetImage.color = origColor;
    }
}
