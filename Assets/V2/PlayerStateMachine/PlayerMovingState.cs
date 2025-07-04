using UnityEngine;

[System.Serializable]
public abstract class PlayerMovingState : PlayerBaseState
{
    protected PlayerController _player; 
    protected Vector3 moveDir;
    protected Vector3 _smoothedMoveDir;
    protected Vector3 _smoothedMoveVelocity;
    protected virtual float moveSpeed => 1;
    
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        _player = player;
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //set movement direction and apply velocity 
        setMoveDir(new Vector2(player.playerMove.x, 0));
        smoothMovement();
        applyVelocity(player);
    }

    protected void applyVelocity(PlayerController player)
    {
        //change speed based on state 
        var velocity = new Vector3(_smoothedMoveDir.x * moveSpeed, player.rb.linearVelocity.y);
        player.rb.linearVelocity = velocity;    
    }

    protected void smoothMovement()
    {
        _smoothedMoveDir = Vector3.SmoothDamp(_smoothedMoveDir, moveDir, ref _smoothedMoveVelocity, 0.2f);
    }

    protected void setMoveDir(Vector3 newDir)
    {
        moveDir = newDir.normalized;
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // player.rb.linearVelocity = Vector3.zero;
        _smoothedMoveVelocity = Vector3.zero;
        _smoothedMoveDir = Vector3.zero;

    }
}
