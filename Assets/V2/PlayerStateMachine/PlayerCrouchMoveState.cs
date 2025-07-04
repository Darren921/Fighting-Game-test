using UnityEngine;

public class PlayerCrouchMoveState : PlayerMovingState
{
    protected override float moveSpeed => _player.WalkSpeed;

    
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // state swap
        if (player.isCrouching && player.playerMove.x == 0)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);
        else if (!player.isCrouching && player.IsWalking)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);
        else if (!player.isCrouching && player.playerMove.x == 0)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
        else if (player.isCrouching && (player.playerMove.x != 0 && player.IsAttacking)) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
    }
    
    
    

   
}
