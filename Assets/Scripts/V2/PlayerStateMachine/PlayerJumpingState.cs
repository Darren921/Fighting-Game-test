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
            if (player.IsAttacking)
            {
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
            }
        }
        else
        {
            switch (player.playerMove.y)
            {
                case 0:
                {
                    if (player.playerMove == Vector3.zero)
                        playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
                    else if (player.IsWalking && player.playerMove.y == 0)
                        playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);
                    break;
                }
                case < 0:
                    playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);
                    break;
                case > 0:
                    playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Jumping);
                    break;
            }

            if (!player.isGrounded)
            {
                switch (player.InputReader.currentMoveInput)
                {
                    case InputReader.MovementInputResult.Backward:
                        playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);
                        break;
                    case InputReader.MovementInputResult.Forward:
                        playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Running);
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

        player.rb.linearVelocity = new Vector3(xJumpVal, velocity, 0);
    }


    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //performing jump and applying custom gravity 

        if (!player.isGrounded)
        {
            player.gravityManager.ApplyGravity(player);
        }
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
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