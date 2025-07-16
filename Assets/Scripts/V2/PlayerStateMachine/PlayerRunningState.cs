using UnityEngine;

public class PlayerRunningState : PlayerMovingState
{
    protected override float moveSpeed => _player.RunSpeed;


    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        base.EnterState(playerStateManager, player);
        player.rb.linearVelocity = Vector3.zero;
        _smoothedMoveDir = Vector3.zero;
        _smoothedMoveVelocity = Vector3.zero;
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //switch states 
        if (player.playerMove == Vector3.zero)
        {
            if (player.playerMove == Vector3.zero && _smoothedMoveDir.magnitude < 0.9f) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
            
        }

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

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {

        Debug.Log(player.rb.linearVelocity);
    }
}
