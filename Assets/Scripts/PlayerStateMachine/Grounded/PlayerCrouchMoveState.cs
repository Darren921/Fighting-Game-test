using UnityEngine;

public class PlayerCrouchMoveState : PlayerMovingState
{
    protected override float moveSpeed => _player.WalkSpeed;

    
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        switch (player.IsCrouching)
        {
            // state swap
            case true when player.PlayerMove.x == 0:
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Crouching);
                break;
            case false when player.IsWalking:
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
                break;
            case false when player.PlayerMove.x == 0:
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Neutral);
                break;
            case true when (player.PlayerMove.x != 0 && player.IsAttacking):
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Attack);
                break;
        }
    }
}
