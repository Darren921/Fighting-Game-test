using UnityEngine;

public class PlayerCrouchMoveState : PlayerMovingState
{
    protected override float moveSpeed => _player.WalkSpeed;

    
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // state swap
        if (player.IsCrouching && player.PlayerMove.x == 0)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Crouching);
        else if (!player.IsCrouching && player.IsWalking)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
        else if (!player.IsCrouching && player.PlayerMove.x == 0)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Neutral);
        else if (player.IsCrouching && (player.PlayerMove.x != 0 && player.IsAttacking)) playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Attack);
    }
}
