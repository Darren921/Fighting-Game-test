using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttackState : PlayerBaseState
{
    // for future combo uses (look up table )
  


    private Coroutine cooldownCoroutine;
    private InputReader.AttackInputResult lastAttack;
    private InputReader.MovementInputResult lastMove;


    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (player.Animator.GetBool(player.Idle))
        {
            player.Animator.SetBool(player.Idle, false);
        }
//        Debug.Log(player.Rb.linearVelocity); 
        

      
//           Debug.Log(lastMove); 
//           Debug.Log(lastAttack);
    }


    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !player.animator.IsInTransition(0) && player.IsAttacking)
        // {
        //     player.SetAttacking(); 
        // }

        player.IsGrounded = player.GravityManager.CheckGrounded(player);

        player.Animator.SetBool(player.airborne, !player.IsGrounded);


        if (player.IsAttacking && !player.OnAttackCoolDown)
        {
            Debug.Log("attacking");
            PerformAttack(player);
        }

        // State swapping 
        if (!player.IsGrounded || player.IsAttacking) return;
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral);
        if (player.IsWalking)
        {
            Debug.Log("going to moving");
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
        }
        else if (player.IsCrouching)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Crouching);
        }


//        Debug.Log(player.gravityManager.GetVelocity());
    }

    private void PerformAttack(PlayerController player)
    {
        lastAttack = player.InputReader.currentAttackInput;
        lastMove = player.InputReader.currentMoveInput;
        Debug.Log(lastMove);
        if (player.IsAttacking && !player.OnAttackCoolDown)
        {
            // choose attack based on input 
            switch (lastAttack)
            {
                case InputReader.AttackInputResult.Light or InputReader.AttackInputResult.LightLeft or InputReader.AttackInputResult.LightRight:
                    Light(player, lastMove);
//                    Debug.Log(lastMove.ToString());
                    break;
                case InputReader.AttackInputResult.Medium or InputReader.AttackInputResult.MediumLeft or InputReader.AttackInputResult.MediumRight:
                    Medium(player, lastMove);
                    //        Debug.Log(lastMove.ToString());
                    break;
                case InputReader.AttackInputResult.Heavy:
                    Light(player, lastMove);
                    break;
                case InputReader.AttackInputResult.None:
                    Debug.LogError("Attack dectection Failed ");
                    break;
            }

            cooldownCoroutine = player.StartCoroutine(EnforceCooldown(player));
        }
    }


    private void Light(PlayerController player, InputReader.MovementInputResult move)
    {
        // Set the attack animation, and if players isn't in animation, select the correct attack based on movement
        player.Animator.SetBool(player.Light, true);
        switch (move)
        {
            case InputReader.MovementInputResult.Backward or InputReader.MovementInputResult.UpRight
                or InputReader.MovementInputResult.DownRight:
                player.Animator.SetBool(player.right, true);
                break;
            case InputReader.MovementInputResult.Forward or InputReader.MovementInputResult.UpLeft
                or InputReader.MovementInputResult.DownLeft:
                player.Animator.SetBool(player.left, true);
                break;
            case InputReader.MovementInputResult.None:
                break;
        }
    }

    private IEnumerator EnforceCooldown(PlayerController player)
    {
        player.OnAttackCoolDown = true;
        yield return new WaitUntil(() => player.IsAttacking == false);
        player.OnAttackCoolDown = false;
    }

    private void Medium(PlayerController player, InputReader.MovementInputResult move)
    {
        // Set the attack animation, and if players isn't in animation, select the correct attack based on movement
        player.Animator.SetBool(player.Medium, true);
        switch (move)
        {
            case InputReader.MovementInputResult.Backward or InputReader.MovementInputResult.UpRight
                or InputReader.MovementInputResult.DownRight:
                player.Animator.SetBool(player.right, true);
                break;
            case InputReader.MovementInputResult.Forward or InputReader.MovementInputResult.UpLeft
                or InputReader.MovementInputResult.DownLeft:
                player.Animator.SetBool(player.left, true);
                break;
            case InputReader.MovementInputResult.None:
                break;
        }
    }

    private void Heavy(PlayerController player, InputReader.MovementInputResult move)
    {
        Debug.Log("Heavy");
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //applying the custom gravity when player is airborne 
        if (!player.IsGrounded && player.transform.localPosition.y > 0.1f)
        {
            player.GravityManager.ApplyGravity(player);
        }

        player.Rb.linearVelocity = new Vector3( player.Rb.linearVelocity.x, player.GravityManager.GetVelocity(), 0);
        

//        Debug.Log(player.gravityManager.GetVelocity());
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.SetAttacking();
    }
}