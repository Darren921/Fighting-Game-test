using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (player.isGrounded)
        {
            
        }
        else
        {
            
        }
        
        
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        
    }
}
