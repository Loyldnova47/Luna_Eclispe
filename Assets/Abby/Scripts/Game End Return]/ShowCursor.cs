using UnityEngine;

public class ShowCursor : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;// Cursor wasn't showing in game end scene so players couldn't click the button 
        Cursor.visible = true;
    }
}
