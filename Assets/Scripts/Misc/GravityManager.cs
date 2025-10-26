using System;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    private float Velocity { get; set; }
    internal RaycastHit Hit;
    private LayerMask _groundLayerMask;
    private LayerMask _playerLayerMask;


    private void Awake()
    {
        _groundLayerMask = LayerMask.GetMask("Ground");
        _playerLayerMask = LayerMask.GetMask("Player");
    }

    public bool CheckGrounded(PlayerController player)
    {
        //checks if the player is grounded and updates the related bool 
        var grounded = Physics.Raycast(player.raycastPos.position, -player.transform.up, out Hit,player.RaycastDistance, _groundLayerMask); 
//        Debug.Log(Hit.distance);
  //      Debug.DrawRay(player.raycastPos.position, -player.transform.up * player.RaycastDistance, Color.red);
        return grounded;
    }

    // public bool CheckIfPlayer(PlayerController player)
    // {
    //     var playerBelow = Physics.Raycast(player.raycastPos.position, -player.transform.up, out RaycastHit,
    //         player.RaycastDistance, _playerLayerMask);
    //     if (playerBelow)
    //     {
    //         print(player.Rb.linearVelocity);
    //     }
    //
    //     return playerBelow;
    // }

    public void ApplyGravity(PlayerController player)
    {
        //Applies the custom gravity based on grav scale
        Velocity += Physics.gravity.y   * player.GravScale * Time.fixedDeltaTime;
    }

    public float GetVelocity()
    {
        //gets the current velocity value 
        return Velocity;
    }

    public void ResetVelocity()
    {
        //resets the gravity's velocity 
        Velocity = 0; 
    }

    public float SetJumpVelocity(PlayerController player)
    {
        //uses formula in order to get a constant jump height 
        var  targetVelocity = !player.SuperJumpActive ? Mathf.Sqrt(player.JumpHeight * -2 * (Physics.gravity.y  * player.GravScale)) : Mathf.Sqrt((player.JumpHeight * 2 ) *  -2 * (Physics.gravity.y * player.GravScale));
        return Velocity = targetVelocity;
    }
}