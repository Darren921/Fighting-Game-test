using UnityEngine;

public class PlayerWalkingState : PlayerMovingState
{
    protected override float moveSpeed => _player.WalkSpeed;

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //switch states 
        if (player.playerMove == Vector3.zero) playerStateManager.SwitchToLastState();

        if (player.playerMove.x != 0 && player.isCrouching) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.CrouchMove);
        
        if (player.IsAttacking) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
        
        if (player.IsRunning) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Running);

        switch (player.playerMove.y)
        {
            case > 0:
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Jumping);
                break;
            case < 0  :
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);
                break;
        }
    }
}
