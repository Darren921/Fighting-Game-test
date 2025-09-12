using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerStateManager;

public class PlayerNeutralState : PlayerBaseState
{
   
    private Coroutine idleCoroutine;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player )
    {
        idleCoroutine = player.StartCoroutine(CheckIfIdle(player));
        player.rb.linearVelocity = Vector3.zero;
//        Debug.Log("Entered PlayerNeutralState");
    }

    internal override void UpdateState(PlayerStateManager playerStateManager,PlayerController player)
    {
        playerStateManager.CheckForTransition(PlayerStateTypes.Attack | PlayerStateTypes.Jumping | PlayerStateTypes.Crouching | PlayerStateTypes.Walking | PlayerStateTypes.Running | PlayerStateTypes.Dash);
        if(player.SuperJumpActive) playerStateManager.SwitchState (PlayerStateTypes.Jumping);
    }
     
    private IEnumerator CheckIfIdle(PlayerController player)
    {
        //Idle state starts animations (TBA)
        yield return new WaitForSeconds(3f);
    //    Debug.Log("Idle");
        player.Animator.SetBool(player.Idle,true);
    } 

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController player)
    {
        if (!player.IsGrounded)
        {
            player.GravityManager.ApplyGravity(player);
            
            player.rb.linearVelocity  = new Vector3(player.rb.linearVelocity.x,player.GravityManager.GetVelocity() * 0.25f,0);
        }
      
    }

    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    {
        if (idleCoroutine != null)
        {
            player.StopCoroutine(idleCoroutine);
            idleCoroutine = null;
            player.Animator.SetBool(player.Idle,false);

        }        




//        Debug.Log("Exit PlayerNeutralState");
    }
}
