using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNeutralState : PlayerBaseState
{
    private static readonly int Neutral = Animator.StringToHash("Neutral");

    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        
        playerStateManager.player.StartCoroutine(CheckIfIdle(playerStateManager.player)) ;

    }

    internal override void UpdateState(PlayerStateManager playerStateManager,PlayerController player)
    {
        
        if (player.playerMove !=  Vector2.zero)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Moving);
        }

        if (player.isAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
        }
    }

    private IEnumerator CheckIfIdle(PlayerController player)
    {
        yield return new WaitForSeconds(3f);
        player.animator.SetBool(Neutral,true);
    } 

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
       
    }

    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    {
        player.animator.SetBool(Neutral,false);

        player.StopAllCoroutines();

        Debug.Log("Exit PlayerNeutralState");
    }
}
