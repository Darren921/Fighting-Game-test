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
        //
       
        // Checking if the player is grounded and resetting position and velocity 
        
        if (player.playerMove.y > 0 && player.isGrounded)
        {
            Jumping = true;

        } 

        if (player.isGrounded)
        {
            player.animator.SetBool(player.Jump, false);
        }
        else if (!player.isGrounded)
        {
            player.animator.SetBool(player.Jump, true);

        } 
        if (!player.isGrounded && player.IsAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
        }
        
        //Transitioning states 
      
        
        if (player.isGrounded && player.playerMove.y == 0)
        {
            if (player.playerMove == Vector3.zero)
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
            else if (player.playerMove.x != 0 && player.playerMove.y == 0)
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Moving);
        }
        else if (player.isGrounded && player.playerMove.y < 0)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);
        }
    }

    private void TryJump(PlayerController player)
    {
        velocity = player.gravityManager.SetJumpVelocity(player);
        switch (player.InputReader.GetLastInput())
        {
            case InputReader.MovementInputResult.Up:
                xJumpVal = 0;
                break;
            case InputReader.MovementInputResult.UpRight:
                xJumpVal = 3;
                break;
            case InputReader.MovementInputResult.UpLeft:
                xJumpVal = -3;
                break;
            default:
                xJumpVal = 0;
                break;
        }
        player.rb.linearVelocity = new Vector3(xJumpVal, velocity, 0);
    }


    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
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
        if (!player.IsAttacking)
        {
            Debug.Log("Reseting velo");
            player.gravityManager.ResetVelocity();
        }
        player.animator.SetBool(player.Jump, false);
        xJumpVal = 0f;
       Debug.Log("Exiting playerJumpingState");
    }
}
