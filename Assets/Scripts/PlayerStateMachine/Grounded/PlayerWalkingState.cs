using UnityEngine;

[System.Serializable]

public class PlayerWalkingState : PlayerMovingState
{
    protected override float MoveSpeed => Player.WalkSpeed;
    private float temp;
    protected override void ApplyVelocity(PlayerController player)
    {
        var velocity = new Vector3(player.PlayerMove.x * MoveSpeed, player.rb.linearVelocity.y);
        player.rb.linearVelocity = velocity;
        if (!player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Walking")) return;
        temp = player.rb.linearVelocity.x switch
        {
            < 0 when player.Reversed => 1,
            > 0 when player.Reversed => 0,
            _ => player.rb.linearVelocity.x switch
            {
                > 0 => 1,
                < 0 => 0,
            }
        };
        player.Animator.SetFloat("WalkDir", temp);


    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //switch states 
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.CrouchMove 
                                              | PlayerStateManager.PlayerStateTypes.Running | PlayerStateManager.PlayerStateTypes.Jumping | PlayerStateManager.PlayerStateTypes.Crouching | PlayerStateManager.PlayerStateTypes.Dash);
        
    }
}
