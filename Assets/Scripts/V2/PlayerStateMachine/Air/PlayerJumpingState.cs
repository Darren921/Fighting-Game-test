using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    #region Standard Jump Variables

   float velocity;

    #endregion

    private float xJumpVal; // check Try jump method for changes 
    private Collider collider;
    private bool jumpTriggered;
    private InputReader.MovementInputResult enterInput;
    private int jumpCharges;
    private bool atJumpHeight;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        jumpCharges = player.characterData.jumpCharges;
        //apply jump immediately when entering state to prevent update glitches   
        collider = player.GetComponent<Collider>();
        player.Animator.SetBool(player.Jump, true);
        player.IsRunning = false;
        TryJump(player);
        jumpCharges--;
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (player.transform.localPosition.y > player.JumpHeight) atJumpHeight = true;
        // check to see if player is jumping 
        switch (player.IsGrounded)
        {
            case true:
                player.Animator.SetBool(player.Jump, false);
                break;
            case false:
                player.Animator.SetBool(player.Jump, true);
                break;
        }
        
        //Transitioning states 
        if (!player.IsGrounded)
        {
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.AirDash);
            if (jumpCharges > 0 && atJumpHeight && player.PlayerMove.y > 0 )
            {
                player.Animator.SetBool(player.Jump, true);
                jumpCharges--;
                TryJump(player);
            }
        }
        else
        {
            if(!player.AtDashHeight) return;
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Walking | PlayerStateManager.PlayerStateTypes.Crouching | PlayerStateManager.PlayerStateTypes.Jumping | PlayerStateManager.PlayerStateTypes.Walking );
            if (!player.IsGrounded)
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


    }

    private void TryJump(PlayerController player)
    {
        // jumping based off on custom  gravity to ensure the player jumps to same height each time 
        velocity = player.GravityManager.SetJumpVelocity(player);
        xJumpVal = player.InputReader.LastValidMovementInput switch
        {
            InputReader.MovementInputResult.Up => 0,
            InputReader.MovementInputResult.UpRight => 3,
            InputReader.MovementInputResult.UpLeft => -3,
            _ => xJumpVal
        };

        enterInput = player.InputReader.LastValidMovementInput;

    }


    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //performing jump and applying custom gravity 
        player.Rb.linearVelocity = new Vector3(xJumpVal, player.GravityManager.GetVelocity(), 0);
        if (!player.IsGrounded && player.gameObject.transform.localPosition.y > 0.1f )
        {
            player.GravityManager.ApplyGravity(player);
        }
        //        Debug.Log(player.gravityManager.GetVelocity());

    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // if (!player.IsAttacking)
        // {
        //     //   Debug.Log("Reseting velo");
        //     player.gravityManager.ResetVelocity();
        // }

        player.Animator.SetBool(player.Jump, false);
        xJumpVal = 0f;
        atJumpHeight = false;
        //   Debug.Log("Exiting playerJumpingState");
    }
}