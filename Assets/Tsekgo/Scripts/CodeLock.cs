using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CodeLock : MonoBehaviour
{
    [Header("Code")]
    [SerializeField] private string correctCode = "1234";

    [Header("UI")]
    [SerializeField] private GameObject keypadPanel;   // the Canvas panel, disabled by default
    [SerializeField] private TMP_Text display;         // shows entered digits
    [SerializeField] private bool maskInput = false;   // show * instead of digits

    [Header("Player")]
    [Tooltip("Drag your mouse-look and movement scripts here. They're disabled while the keypad is open.")]
    [SerializeField] private MonoBehaviour[] disableWhileOpen;

    [Header("Events")]
    public UnityEvent onUnlocked;   // Door subscribes to this automatically
    public UnityEvent onWrongCode;  // optional: play a buzz sound, etc.

    public bool IsUnlocked { get; private set; }
    public bool IsOpen { get; private set; }

    private string entered = "";
    private bool busy;   // true while showing OK/ERROR feedback

    private void Start()
    {
        if (keypadPanel != null) keypadPanel.SetActive(false);
        UpdateDisplay();
    }

    private void Update()
    {
        if (!IsOpen || Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
        else if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            Submit();
        else if (Keyboard.current.backspaceKey.wasPressedThisFrame)
            Backspace();
    }

    private void OnDisable()
    {
        if (Keyboard.current != null) Keyboard.current.onTextInput -= OnTextInput;
    }


    public void Open()
    {
        if (IsUnlocked || IsOpen) return;

        IsOpen = true;
        entered = "";
        UpdateDisplay();
        keypadPanel.SetActive(true);

        SetPlayerControl(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Keyboard.current != null) Keyboard.current.onTextInput += OnTextInput;
    }

    public void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;
        keypadPanel.SetActive(false);

        SetPlayerControl(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Keyboard.current != null) Keyboard.current.onTextInput -= OnTextInput;
    }

    // Laptop keyboard digits (top row and numpad both arrive as text)
    private void OnTextInput(char c)
    {
        if (c >= '0' && c <= '9')
            PressDigit(c - '0');
    }


    public void PressDigit(int digit)
    {
        Debug.Log("PressDigit " + digit);
        if (busy || entered.Length >= correctCode.Length) return;
        entered += digit.ToString();
        UpdateDisplay();
    }

    public void Clear()
    {
        if (busy) return;
        entered = "";
        UpdateDisplay();
    }

    public void Backspace()
    {
        if (busy || entered.Length == 0) return;
        entered = entered.Substring(0, entered.Length - 1);
        UpdateDisplay();
    }

    public void Submit()
    {
        if (busy) return;

        if (entered == correctCode)
            StartCoroutine(CorrectRoutine());
        else
            StartCoroutine(WrongRoutine());
    }



    private IEnumerator CorrectRoutine()
    {
        busy = true;
        IsUnlocked = true;
        display.text = "OK";
        yield return new WaitForSeconds(0.6f);

        Close();
        onUnlocked.Invoke();
        busy = false;
    }

    private IEnumerator WrongRoutine()
    {
        busy = true;
        display.text = "ERROR";
        onWrongCode.Invoke();
        yield return new WaitForSeconds(0.8f);

        entered = "";
        UpdateDisplay();
        busy = false;
    }

    private void UpdateDisplay()
    {
        if (display == null) return;
        display.text = maskInput ? new string('*', entered.Length) : entered;
    }

    private void SetPlayerControl(bool enabled)
    {
        foreach (var script in disableWhileOpen)
            if (script != null) script.enabled = enabled;
    }
}