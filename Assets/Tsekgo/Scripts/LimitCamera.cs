using UnityEngine;

public class LimitCamera : MonoBehaviour
{
   public GameObject Player; 

   private void LateUpdate()
   {
    
        transform.rotation = Quaternion.Euler(90f, Player.transform.eulerAngles.y, 0f);

   }

}
