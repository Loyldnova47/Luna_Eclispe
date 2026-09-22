using UnityEngine;

public class SlamHitBox : MonoBehaviour
{
   public float damage = 10f;

   private void OnTriggerEnter (Collider other)
   {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage (damage);
        }
   }
}
