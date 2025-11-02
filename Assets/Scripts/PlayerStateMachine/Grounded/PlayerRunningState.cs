using UnityEngine;

[System.Serializable]

public class PlayerRunningState : PlayerMovingState
{
    protected override float MoveSpeed => Player.RunSpeed;
    
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.IsRunning = true;
        //controls the decel curve to make slow down movement more accurate 
        if (player.PlayerMove == Vector3.zero && !player.DashMarcoActive )
        {
            if (!player.Decelerating && !player.DecelActive)
            {
                player.DecelActive = true;
                player.Decelerating = true;
                player.StartCoroutine(player.DecelerationCurve(player));
            }
        }

        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack);
     
        
        if (player.InputReader.CurrentMoveInput == InputReader.MovementInputResult.Backward )
        {
//            Debug.Log("BACK");
            player.IsRunning = false;
            player.IsWalking = true;
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
        }
        switch (player.PlayerMove.y)
        {
            case > 0:
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Jumping);
                break;
            case < 0  :
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Crouching);
                break;
        }
        
        //switch states 
        if(player.Decelerating || !player.DecelActive ) return;
        
      
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral );
        
        if (player.IsWalking) playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);

    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        SetMoveDir(!player.DashMarcoActive ? new Vector2(player.PlayerMove.x, 0) :  !player.Reversed ? new Vector2(1, 0) : new Vector2(-1, 0));
        SmoothMovement();
        ApplyVelocity(player);
    }

    protected override void ApplyVelocity(PlayerController player)
    {
        var velocity =  new Vector3(SmoothedMoveDir.x * MoveSpeed, player.rb.linearVelocity.y) ;
        player.rb.linearVelocity = velocity;    
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {

        player.IsRunning = false;
      Debug.Log(player.rb.linearVelocity);
      player.DecelActive = false;
    }
}
