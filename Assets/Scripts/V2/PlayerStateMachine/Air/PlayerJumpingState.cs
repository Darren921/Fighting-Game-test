using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    #region Standard Jump Variables

    [Header("Jumping")] float velocity;

    #endregion

    private float xJumpVal; // check Try jump method for changes 
    private Collider collider;
    private bool jumpTriggered;
    private InputReader.MovementInputResult enterInput;
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        collider = player.GetComponent<Collider>();
        player.animator.SetBool(player.Jump, true);
        player.IsRunning = false;
        TryJump(player);
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // check to see if player is jumping 


        
        switch (player.isGrounded)
        {
            case true:
                player.animator.SetBool(player.Jump, false);
                break;
            case false:
                player.animator.SetBool(player.Jump, true);
                break;
        }

        if (!player.isGrounded)
        {
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.AirDash);
            
        }
        else
        {
            if(!player.AtDashHeight) return;
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Walking | PlayerStateManager.PlayerStateTypes.Crouching | PlayerStateManager.PlayerStateTypes.Jumping | PlayerStateManager.PlayerStateTypes.Walking );
            if (!player.isGrounded)
            {
                switch (player.InputReader.currentMoveInput)
                {
                    case InputReader.MovementInputResult.Backward:
                        playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
                        break;
                    case InputReader.MovementInputResult.Forward:
                        playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Running);
                        break;
                }
            }
        }


        //Transitioning states 
    }

    private void TryJump(PlayerController player)
    {
        // jumping based off on custom  gravity to ensure the player jumps to same height each time 
        velocity = player.gravityManager.SetJumpVelocity(player);
        switch (player.InputReader.LastValidMovementInput)
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
        }

        enterInput = player.InputReader.LastValidMovementInput;

    }


    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //performing jump and applying custom gravity 

        player.rb.linearVelocity = new Vector3(xJumpVal, player.gravityManager.GetVelocity(), 0);
//        Debug.Log(player.gravityManager.GetVelocity());

        if (!player.isGrounded && player.gameObject.transform.localPosition.y > 0.1f )
        {
            player.gravityManager.ApplyGravity(player);
        }
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // only resting jump velo if not attacking 
        // if (!player.IsAttacking)
        // {
        //     //   Debug.Log("Reseting velo");
        //     player.gravityManager.ResetVelocity();
        // }

        player.animator.SetBool(player.Jump, false);
        xJumpVal = 0f;
        //   Debug.Log("Exiting playerJumpingState");
    }
}