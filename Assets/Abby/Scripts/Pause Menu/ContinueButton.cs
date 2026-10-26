using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public Button button;

    void Start()
    {
        // grey it out if there's no save
        button.interactable = SaveSystem.HasSave();
    }

    public void OnContinue()
    {
        SaveSystem.Continue();
    }
}