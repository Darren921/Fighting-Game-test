using UnityEngine;

[System.Serializable]
public class PlayerMovingState : PlayerBaseState
{
    private Vector3 moveDir;
    private Vector3 _smoothedMoveDir;
    private Vector3 _smoothedMoveVelocity;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (player.playerMove == Vector3.zero)
        {
            playerStateManager.SwitchToLastState();
            return;
        }

        if (player.playerMove.x != 0 && player.isCrouching)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.CrouchMove);
        }

        if (player.IsAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
            return;
        }

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

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        setMoveDir(new Vector2(player.playerMove.x, 0));
        smoothMovement();
        applyVelocity(player);
    }

    private void applyVelocity(PlayerController player)
    {
        var speed = player.IsRunning ? player.RunSpeed : player.WalkSpeed;
        var velocity = new Vector3(_smoothedMoveDir.x * speed, player.rb.linearVelocity.y);
//        Debug.Log($"{player.name} applying velocity: {velocity}");
        player.rb.linearVelocity = velocity;    
    }

    private void smoothMovement()
    {
        _smoothedMoveDir = Vector3.SmoothDamp(_smoothedMoveDir, moveDir, ref _smoothedMoveVelocity, 0.2f);
    }

    public void setMoveDir(Vector3 newDir)
    {
        moveDir = newDir.normalized;
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.rb.linearVelocity = Vector3.zero;
        _smoothedMoveVelocity = Vector3.zero;
        _smoothedMoveDir = Vector3.zero;

    }
}
