using UnityEngine;
[System.Serializable]

public class PlayerMovingState : PlayerBaseState
{
    internal Vector2 moveDir;
    private Vector2 _smoothedMoveDir;
    private Vector2 _smoothedMoveVelocity;

    internal override void EnterState(PlayerStateManager playerStateManager) { }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
     
        if (player.playerMove == Vector2.zero)
            playerStateManager.SwitchState(playerStateManager.NeutralState);

        switch (player.playerMove.y)
        {
            case > 0:
                playerStateManager.SwitchState(playerStateManager.JumpingState);
                break;
            case < 0:
                playerStateManager.SwitchState(playerStateManager.CrouchingState);
                break;
        }
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        smoothMovement();
        movePlayer(player);
        setMoveDir(new Vector2(player.playerMove.x, 0));
    }


    private void movePlayer(PlayerController player)
    {
        Vector3 move = new Vector3(_smoothedMoveDir.x, 0, 0) * (player._moveSpeed * Time.fixedDeltaTime);
        player.transform.position += move;
    }

    private void smoothMovement()
    {
        _smoothedMoveDir = Vector2.SmoothDamp(_smoothedMoveDir, moveDir, ref _smoothedMoveVelocity, 0.1f);
    }

    public void setMoveDir(Vector2 newDir)
    {
        moveDir = newDir.normalized;
    }
    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    { 
        Debug.Log("Player Exit Move State");
    }

}
