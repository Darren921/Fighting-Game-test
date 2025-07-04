using UnityEngine;

public class PlayerRunningState : PlayerMovingState
{
    protected override float moveSpeed => _player.RunSpeed;


    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //switch states 
        if (player.playerMove == Vector3.zero) playerStateManager.SwitchToLastState();

        if (player.playerMove.x != 0 && player.isCrouching) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.CrouchMove);
        
        if (player.IsAttacking) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
        
        if (player.IsWalking) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);

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
