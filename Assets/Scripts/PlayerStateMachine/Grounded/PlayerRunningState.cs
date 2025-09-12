using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunningState : PlayerMovingState
{
    protected override float moveSpeed => _player.RunSpeed;
    
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //controls the decel curve to make slow down movement more accurate 
        if (player.PlayerMove == Vector3.zero )
        {
            if (!player.decelerating)
            {
                player.decelerating = true;
                player.StartCoroutine(player.DecelerationCurve(player));
            }
        }

        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack);
     
        
        if (player.InputReader.currentMoveInput == InputReader.MovementInputResult.Backward )
        {
//            Debug.Log("BACK");
            player.IsRunning = false;
            player.IsWalking = true;
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
        }

        //switch states 
        if(player.decelerating ) return;
        
        if (player.PlayerMove == Vector3.zero && player.rb.linearVelocity.magnitude < 0.1f)
        {
//            Debug.Log("HEH");
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Neutral);
        }
        playerStateManager.CheckForTransition(  PlayerStateManager.PlayerStateTypes.CrouchMove );
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

        player.IsRunning = false;
      Debug.Log(player.rb.linearVelocity);
    }
}
