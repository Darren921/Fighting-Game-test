using Unity.VisualScripting;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
      Debug.Log("PlayerDashState EnterState");

    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
   
        //grab the last inputs given 
        var dir  = player.DashDir;
        var newDashVelo = Vector3.zero;
        if (dir == InputReader.MovementInputResult.Forward)
        {
         newDashVelo =  !player.Reversed ? new Vector3(10, 0, 0 ) : new Vector3(-10, 0, 0);
        }
        else if (dir == InputReader.MovementInputResult.Backward)
        {
            newDashVelo =  !player.Reversed ? new Vector3(-10, 0, 0 ) : new Vector3(10, 0, 0);
        }
         
        Debug.Log(newDashVelo);
        player.rb.linearVelocity = newDashVelo * 2;
        if (player.playerMove.x == 0) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
        if (player.playerMove.x > 0) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);
        if (player.isCrouching) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);
        if (player.isGrounded && player.playerMove.y > 0) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Jumping);
        
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.Dashing = false;
    }
}
