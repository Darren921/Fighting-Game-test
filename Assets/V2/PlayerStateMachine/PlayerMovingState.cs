using UnityEngine;
[System.Serializable]

public class PlayerMovingState : PlayerBaseState
{
    private Vector2 moveDir;
    private Vector2 _smoothedMoveDir;
    private Vector2 _smoothedMoveVelocity;
    private Vector3 move;

    internal override void EnterState(PlayerStateManager playerStateManager,PlayerController player)
    {
      //  Debug.Log("Entered PlayerMovingState");
        playerStateManager.player.animator.SetBool(player.Move, true);

    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (player.playerMove == Vector2.zero)
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);

        if (player.IsAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
        }
        switch (player.playerMove.y)
        {
            case > 0:
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Jumping);
                break;
            case < 0:
        //        Debug.Log("Switched to crouch state (M.S)");
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);
                break;
        }
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        setMoveDir(new Vector2(player.playerMove.x, 0));
        smoothMovement();
        movePlayer(player);
    }


    private void movePlayer(PlayerController player)
    {
        if (player.IsWalking)
        {
            move = new Vector3(_smoothedMoveDir.x, 0, 0) * (player.WalkSpeed * Time.fixedDeltaTime);
        }
        else if (player.IsRunning)
        {
            move = new Vector3(_smoothedMoveDir.x, 0, 0) * (player.RunSpeed * Time.fixedDeltaTime);
        }
       
        player.rb.MovePosition(player.transform.position + move);

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
        move = Vector3.zero;
        _smoothedMoveDir = Vector2.zero;
        setMoveDir(new Vector2(0, 0));
        moveDir = Vector2.zero;
        _smoothedMoveVelocity = Vector2.zero;
    //    Debug.Log("Player Exit Move State");
        playerStateManager.player.animator.SetBool(player.Move, false);

    }

}
