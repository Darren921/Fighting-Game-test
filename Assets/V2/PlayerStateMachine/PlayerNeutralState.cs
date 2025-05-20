using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNeutralState : PlayerBaseState
{
    
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        
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

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
       
    }

    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    {
        Debug.Log("Exit PlayerNeutralState");
    }
}
