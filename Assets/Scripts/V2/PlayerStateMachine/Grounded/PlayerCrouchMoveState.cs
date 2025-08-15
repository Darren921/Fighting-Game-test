using UnityEngine;

public class PlayerCrouchMoveState : PlayerMovingState
{
    protected override float moveSpeed => _player.WalkSpeed;

    
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // state swap
        if (player.isCrouching && player.playerMove.x == 0)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Crouching);
        else if (!player.isCrouching && player.IsWalking)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
        else if (!player.isCrouching && player.playerMove.x == 0)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Neutral);
        else if (player.isCrouching && (player.playerMove.x != 0 && player.IsAttacking)) playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Attack);
    }
}
