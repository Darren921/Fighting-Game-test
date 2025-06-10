using System;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
   public float velocity {get; private set;}
   internal RaycastHit raycastHit;
   private LayerMask groundLayerMask;


   private void Awake()
   {
       groundLayerMask = LayerMask.GetMask("Ground");   
   }

  public  bool CheckifGrounded(PlayerController player)
   {
     return Physics.Raycast(player.transform.position, -player.transform.up, out raycastHit, player.raycastDistance,
         groundLayerMask) && velocity < 0;
   }

   public void ApplyGravity(PlayerController player)
   {
       velocity += Physics.gravity.y * player.gravScale * Time.deltaTime;
   }

   public float GetVelocity()
   {
       return velocity;
   }
   public void ResetVelocity()
   {
       velocity = 0;
   }

   public float SetJumpVelocity(PlayerController player)
   {
      return velocity = Mathf.Sqrt(player.jumpHeight * -2 * (Physics.gravity.y * player.gravScale)); 
   }
  
}
