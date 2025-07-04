using UnityEngine;
[System.Serializable]
public class PlayerCrouchingState : PlayerBaseState
{
    internal override void EnterState(PlayerStateManager playerStateManager,PlayerController player)
    {
        Debug.Log("Entering PlayerCrouchingState");
    }

    internal override void UpdateState(PlayerStateManager playerStateManager,PlayerController player)
    {
    
        // swap states 
        if (player.playerMove is { y: 0, x: 0 })
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
        }
        
        if (!player.isCrouching && player.IsWalking)
        {
            Debug.Log("Switched to Move state (C.S)");
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);
        }

        else if (player.isCrouching && player.playerMove.x != 0)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.CrouchMove);
        }

        if (player.isCrouching && player.IsAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
        }
          
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
    }

    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    {
        Debug.Log("Exiting PlayerCrouchingState");
    }
}
