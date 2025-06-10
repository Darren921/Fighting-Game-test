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
        
        if (player.playerMove !=  Vector2.zero)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Moving);
        }

       
    }

    private IEnumerator CheckIfIdle(PlayerController player)
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("Idle");
        player.animator.SetBool(Neutral,true);
    } 

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
       
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
