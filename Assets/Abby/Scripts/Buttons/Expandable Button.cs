using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExpandableButton : MonoBehaviour
{
    [SerializeField] private Button mainButton;
    [SerializeField] private GameObject[] subButtons;
    [SerializeField] private Key activationKey = Key.Q; // Insert Activation key In inspector

    private bool isExpanded = false;

    void Start()
    {
        mainButton.onClick.AddListener(ToggleSubButtons);
        SetSubButtons(false);
    }

    void Update()
    {
        if (Keyboard.current[activationKey].wasPressedThisFrame)
        {
            ToggleSubButtons();
        }
    }

    void ToggleSubButtons()
    {
        isExpanded = !isExpanded;
        SetSubButtons(isExpanded);// if the key is pressed the subButtons will either show/hide. 
    }

    void SetSubButtons(bool state)
    {
        foreach (var btn in subButtons)
            btn.SetActive(state);
    }
}