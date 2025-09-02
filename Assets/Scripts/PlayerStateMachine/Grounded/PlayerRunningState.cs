using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunningState : PlayerMovingState
{
    protected override float moveSpeed => _player.RunSpeed;
    
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //controls the decel curve to make slow down movement more accurate 
        if (player.PlayerMove == Vector3.zero && !decelerating)
        {
            player.StartCoroutine(DecelerationCurve(player));
        }
        if (player.PlayerMove == Vector3.zero && decelerating == false)
        {
                Debug.Log("HEH");
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Neutral);
        }

        //switch states 
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

        playerStateManager.CheckForTransition( PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.CrouchMove);
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
