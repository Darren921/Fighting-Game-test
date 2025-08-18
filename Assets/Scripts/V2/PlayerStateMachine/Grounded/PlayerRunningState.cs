using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunningState : PlayerMovingState
{
    protected override float moveSpeed => _player.RunSpeed;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        base.EnterState(playerStateManager, player);
        // player.rb.linearVelocity = Vector3.zero;
        // _smoothedMoveDir = Vector3.zero;
        // _smoothedMoveVelocity = Vector3.zero;
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //switch states 
        if (player.PlayerMove == Vector3.zero && !decelerating)
        {
            player.StartCoroutine(DecelerationCurve(player));
        }
        if (player.PlayerMove == Vector3.zero && decelerating == false)
        {
                Debug.Log("HEH");
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Neutral);
        }

        if(decelerating) return;
        if (player.InputReader.currentMoveInput == InputReader.MovementInputResult.Backward && player.IsGrounded)
        {
            player.IsRunning = false;
            player.IsWalking = true;
            playerStateManager.SwitchToLastState();
        }
        else if (player.InputReader.currentMoveInput == InputReader.MovementInputResult.Backward && !player.IsGrounded)
        {
            player.IsRunning = false;
            player.IsWalking = true;
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
        }

        playerStateManager.CheckForTransition( PlayerStateManager.PlayerStateTypes.Attack);
        if (player.PlayerMove.x != 0 && player.IsCrouching) playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.CrouchMove);
        
        if (player.IsWalking) playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);

        switch (player.PlayerMove.y)
        {
            case > 0:
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Jumping);
                break;
            case < 0  :
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Crouching);
                break;
        }
    }

   

 

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {

//        Debug.Log(player.rb.linearVelocity);
    }
}
