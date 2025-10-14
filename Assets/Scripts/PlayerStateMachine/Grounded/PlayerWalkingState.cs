using UnityEngine;

public class PlayerWalkingState : PlayerMovingState
{
    protected override float MoveSpeed => Player.WalkSpeed;
    protected override void ApplyVelocity(PlayerController player)
    {
        var velocity = new Vector3(player.PlayerMove.x * MoveSpeed, player.rb.linearVelocity.y);
        player.rb.linearVelocity = velocity;    
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //switch states 
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.CrouchMove 
                                              | PlayerStateManager.PlayerStateTypes.Running | PlayerStateManager.PlayerStateTypes.Jumping | PlayerStateManager.PlayerStateTypes.Crouching | PlayerStateManager.PlayerStateTypes.Dash);
        
    }
}
