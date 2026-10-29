using System.Collections;
using UnityEngine;
using TMPro;

public class RoomNames : MonoBehaviour
{
    public TMP_Text partNameText;
    public CanvasGroup canvasGroup;
    public string roomName = "Engine Room";
    public float charactersPerSecond = 20f;
    public float fadeOutDuration = 0.5f;

    [Header("Cursor")]
    public string cursorChar = "|";
    public float blinkInterval = 0.5f;

    private Coroutine routine;

    private void Start()
    {
        canvasGroup.alpha = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (routine != null) StopCoroutine(routine);
        canvasGroup.alpha = 1f;
        routine = StartCoroutine(TypeThenBlink());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeOut());
    }

    private IEnumerator TypeThenBlink()
    {
        float delay = 1f / charactersPerSecond;

        for (int i = 0; i <= roomName.Length; i++)
        {
            partNameText.text = roomName.Substring(0, i) + cursorChar;
            yield return new WaitForSeconds(delay);
        }

        bool cursorOn = true;
        while (true)
        {
            cursorOn = !cursorOn;
            partNameText.text = roomName + (cursorOn
                ? cursorChar
                : "<alpha=#00>" + cursorChar);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}