using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.rb.linearVelocity = new Vector3(15f,0,0);
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        
    }
}
