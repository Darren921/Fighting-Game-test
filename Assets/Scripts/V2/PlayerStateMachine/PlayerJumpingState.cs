using Unity.VisualScripting;
using UnityEngine;
public class PlayerJumpingState : PlayerBaseState
{
    
    #region Standard Jump Variables
    [Header("Jumping")]
    float velocity;
    #endregion
    private float xJumpVal; // check Try jump method for changes 
    private Collider collider;
    private bool Jumping;

    internal override void EnterState(PlayerStateManager playerStateManager,PlayerController player)
    {
       collider = player.GetComponent<Collider>();
       player.animator.SetBool(player.Jump, true);
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //check to see if player is jumping 
        if (player.playerMove.y > 0 && player.isGrounded)
        {
            Jumping = true;

        }
        // Toggling animation 
        switch (player.isGrounded )
        {
            case true:
                player.animator.SetBool(player.Jump, false);
                break;
            case false:
                player.animator.SetBool(player.Jump, true);
                break;
        } 
        
        //Transitioning states 
        if (!player.isGrounded && player.IsAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
        }
        
        if (player.isGrounded && player.playerMove.y == 0)
        {
            if (player.playerMove == Vector3.zero)
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
            else if (player.IsWalking && player.playerMove.y == 0)
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);
        }
        else if (player.isGrounded && player.playerMove.y < 0)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);
        }
    }

    private void TryJump(PlayerController player)
    {
        // jumping based off on custom  gravity to ensure the player jumps to same height each time 
        velocity = player.gravityManager.SetJumpVelocity(player);
        xJumpVal = player.InputReader.currentMoveInput switch
        {
            InputReader.MovementInputResult.Up => 0,
            InputReader.MovementInputResult.UpRight => 3,
            InputReader.MovementInputResult.UpLeft => -3,
            _ => 0
        };
        player.rb.linearVelocity = new Vector3(xJumpVal, velocity, 0);
    }


    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //performing jump and applying custom gravity 
        if (Jumping)
        {
            TryJump(player);
            Jumping = false;
        }

        if (!player.isGrounded)
        {
            player.gravityManager.ApplyGravity(player);
        }
    }

    internal override void ExitState(PlayerStateManager playerStateManager,  PlayerController player)
    {
        // only resting jump velo if not attacking 
        if (!player.IsAttacking)
        {
         //   Debug.Log("Reseting velo");
            player.gravityManager.ResetVelocity();
        }
        player.animator.SetBool(player.Jump, false);
        xJumpVal = 0f;
    //   Debug.Log("Exiting playerJumpingState");
    }
}
