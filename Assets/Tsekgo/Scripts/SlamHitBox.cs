using UnityEngine;

public class SlamHitBox : MonoBehaviour
{
   public float damage = 10f;

   private void OnTriggerEnter (Collider other)
   {
        //the gameobject with the tag "player" takes damage when slammed
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage (damage);
        }
   }
}
