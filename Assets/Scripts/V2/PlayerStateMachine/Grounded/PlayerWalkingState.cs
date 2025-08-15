using UnityEngine;

public class PlayerWalkingState : PlayerMovingState
{
    protected override float moveSpeed => _player.WalkSpeed;

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //switch states 
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.CrouchMove 
                                              | PlayerStateManager.PlayerStateTypes.Running | PlayerStateManager.PlayerStateTypes.Jumping | PlayerStateManager.PlayerStateTypes.Crouching);
        
    }
}
