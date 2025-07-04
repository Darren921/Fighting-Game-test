using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNeutralState : PlayerBaseState
{
    private static readonly int Neutral = Animator.StringToHash("Neutral");
    private Coroutine idleCoroutine;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player )
    {
        idleCoroutine = player.StartCoroutine(CheckIfIdle(player));
      
    }

    internal override void UpdateState(PlayerStateManager playerStateManager,PlayerController player)
    {

        if (player.IsAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
        }
        
        if (player.IsWalking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);
        }
        if (player.IsRunning)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Running);
        }

        if (player.isBackDashing)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Dash);
        }

    }
     
    private IEnumerator CheckIfIdle(PlayerController player)
    {
        //Idle state starts animations (TBA)
        yield return new WaitForSeconds(1f);
        Debug.Log("Idle");
        player.animator.SetBool(Neutral,true);
        player.IsRunning = false;
        player.IsWalking = false;
    } 

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController player)
    {
        if (!player.isGrounded)
        {
            player.gravityManager.ApplyGravity(player);
            
            player.rb.linearVelocity  = new Vector3(player.rb.linearVelocity.x,player.gravityManager.GetVelocity(),0);
        }
      
    }

    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    {
        if (idleCoroutine != null)
        {
            player.StopCoroutine(idleCoroutine);
            idleCoroutine = null;
            player.animator.SetBool(Neutral,false);

        }

        Debug.Log("Exit PlayerNeutralState");
    }
}
