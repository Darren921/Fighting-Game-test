using System;
using System.Collections;
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
    private bool doubleJumpReady ;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        
        jumpCharges = player.characterData.jumpCharges;
        //apply jump immediately when entering state to prevent update glitches   
        collider = player.GetComponent<Collider>();
        player.Animator.SetBool(player.Jump, true);
        player.IsRunning = false;
        TryJump(player);
        jumpCharges--;
        player.OnJump += HandleJumpInput; 

    }

    private void HandleJumpInput()
    {
        jumpTriggered = true;
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        atJumpHeight = player.transform.localPosition.y > player.JumpHeight;
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

        doubleJumpReady = player.PlayerMove == Vector3.zero && atJumpHeight && jumpCharges > 0 &&
                          !player.SuperJumpActive;
    
        //Transitioning states 
        if (!player.IsGrounded)
        {
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.AirDash);
            if (jumpCharges > 0 && atJumpHeight && doubleJumpReady && jumpTriggered)
            {
                player.Animator.SetBool(player.Jump, true);
                doubleJumpReady = false;
                jumpCharges--;
                TryJump(player);
                jumpTriggered =  false;
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
        Debug.Log(player.InputReader.currentMoveInput);
        xJumpVal = player.InputReader.currentMoveInput switch
        {
            InputReader.MovementInputResult.Up => 0,
            InputReader.MovementInputResult.Forward => !player.Reversed ? 3 : -3,
            InputReader.MovementInputResult.Backward => !player.Reversed ? -3 : 3,
            InputReader.MovementInputResult.UpRight => 3,
            InputReader.MovementInputResult.UpLeft => -3,
            _ => xJumpVal
        };

        enterInput = player.InputReader.currentMoveInput;

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
        jumpTriggered =  false;
        //   Debug.Log("Exiting playerJumpingState");
        player.OnJump -= HandleJumpInput;
        player.SuperJumpActive = false;
    }
}