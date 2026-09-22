using UnityEngine;

public class MenuToggler : MonoBehaviour
{
    [SerializeField] private GameObject panelToEnable;

    
    [SerializeField] private GameObject panelToDisable;

    public void SwitchPanels()
    
    {
       if (panelToEnable != null)
       {
        panelToEnable.SetActive(true);
       }
       else
       {
          Debug.LogWarning($"[Menu Toggler] No panel assigned to enable on {gameObject.name}!");
       }

       if (panelToDisable != null)
       {
        panelToDisable.SetActive(false);
       }
    }

    public void ReverseSwitchPanels()
    {
      if (panelToDisable != null) panelToDisable.SetActive(true);
      if (panelToEnable != null) panelToEnable.SetActive(false);
    }
    
}
