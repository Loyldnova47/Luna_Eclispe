using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
 
// Temporary debug script: logs which UI object is under the mouse on every left click.
public class UIClickDebug : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
 
        if (EventSystem.current == null)
        {
            Debug.Log("No EventSystem in the scene!");
            return;
        }
 
        var data = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
 
        string top = results.Count == 0 ? "NO UI" : results[0].gameObject.name;
        Debug.Log("Click -> top UI object: " + top + " | cursor lockState: " + Cursor.lockState + " | timeScale: " + Time.timeScale);
    }
}
 