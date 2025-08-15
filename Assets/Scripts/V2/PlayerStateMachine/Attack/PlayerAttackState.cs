
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerAttackState : PlayerBaseState
{
  

    // for future combo uses (look up table )
    private Dictionary<( InputReader.MovementInputResult, InputReader.AttackInputResult), InputReader.AttackInputResult> attackMoveActions = new()
    {
            { (InputReader.MovementInputResult.None,InputReader.AttackInputResult.Light), InputReader.AttackInputResult.Light },
            { (InputReader.MovementInputResult.Forward,InputReader.AttackInputResult.Light), InputReader.AttackInputResult.LightLeft },
            { (InputReader.MovementInputResult.Backward,InputReader.AttackInputResult.Light), InputReader.AttackInputResult.LightLeft },

            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.Medium), InputReader.AttackInputResult.Medium },
            { (InputReader.MovementInputResult.Forward,InputReader.AttackInputResult.Medium), InputReader.AttackInputResult.MediumLeft },
            { (InputReader.MovementInputResult.Backward,InputReader.AttackInputResult.Medium), InputReader.AttackInputResult.MediumRight },

            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.Heavy), InputReader.AttackInputResult.Heavy },

    };
    
    
    private Coroutine cooldownCoroutine;
    private InputReader.AttackInputResult lastAttack;
    private InputReader.MovementInputResult lastMove;

    
    internal override void EnterState(PlayerStateManager playerStateManager,PlayerController player)
    {
        lastMove = player.InputReader.currentMoveInput;
        lastAttack = player.InputReader.currentAttackInput;
           if (player.animator.GetBool(player.Idle))
           {
               player.animator.SetBool(player.Idle, false);
           }
          
     
//         Debug.Log(lastMove);
//        Debug.Log(lastAttack);
    }
    private void Light(PlayerController player, InputReader.MovementInputResult move)
    {           
        // Set the attack animation, and if players isn't in animation, select the correct attack based on movement
        player.animator.SetBool(player.Light,true);
        switch (move)
        {
            case InputReader.MovementInputResult.Backward or InputReader.MovementInputResult.UpRight or InputReader.MovementInputResult.DownRight:
                player.animator.SetBool(player.Right, true);
                break; 
            case InputReader.MovementInputResult.Forward or InputReader.MovementInputResult.UpLeft or InputReader.MovementInputResult.DownLeft:
                player.animator.SetBool(player.Left, true);
                break;
            case InputReader.MovementInputResult.None:
                break;
        }
    }

    private IEnumerator EnforceCooldown(PlayerController player)
    {
        player.onAttackCoolDown = true;
        yield return new WaitUntil(() => player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && player.IsAttacking == false);
        player.onAttackCoolDown = false;
        
    }

    private void Medium(PlayerController player, InputReader.MovementInputResult move)
    {
        // Set the attack animation, and if players isn't in animation, select the correct attack based on movement
        player.animator.SetBool(player.Medium, true);
        switch (move)
        {
            case InputReader.MovementInputResult.Backward or InputReader.MovementInputResult.UpRight
                or InputReader.MovementInputResult.DownRight:
                player.animator.SetBool(player.Right, true);
                break;
            case InputReader.MovementInputResult.Forward or InputReader.MovementInputResult.UpLeft
                or InputReader.MovementInputResult.DownLeft:
                player.animator.SetBool(player.Left, true);
                break;
            case InputReader.MovementInputResult.None:
                break;
        }
    }
    
    private void Heavy(PlayerController player, InputReader.MovementInputResult move)
    {
        
        Debug.Log("Heavy");
        
    }

  

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        // if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !player.animator.IsInTransition(0) && player.IsAttacking)
        // {
        //     player.SetAttacking(); 
        // }

        player.isGrounded = player.gravityManager.CheckifGrounded(player);
      
        player.animator.SetBool(player.Airborne, !player.isGrounded);
    
        // choose attack based on input and any movement detected 

        if (player.IsAttacking && !player.onAttackCoolDown)
        {
            PerformAttack(player);
        }

        // State swapping 
        if (!player.isGrounded || player.IsAttacking ) return;
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral);
        if (player.IsWalking  )
        {
            Debug.Log("going to moving");
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Walking);
        }
        else if(player.isCrouching)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Crouching);

        }

//        Debug.Log(player.gravityManager.GetVelocity());




    }

    private void PerformAttack(PlayerController player)
    {
        if (player.IsAttacking && !player.onAttackCoolDown)
        {
            enforcedCooldown = true;
            switch (lastAttack )
            {
                case InputReader.AttackInputResult.Light:
                    Light(player,lastMove);
//                    Debug.Log(lastMove.ToString());
                    break;
                case InputReader.AttackInputResult.Medium:
                    Medium(player,lastMove);
                    //        Debug.Log(lastMove.ToString());
                    break;
                case InputReader.AttackInputResult.Heavy:
                    break;
                case InputReader.AttackInputResult.None:
                    Debug.LogError("Attack dectection Failed ");
                    break;
            }
            cooldownCoroutine = player.StartCoroutine(EnforceCooldown(player));
        }
    }

    private bool enforcedCooldown;


    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {

        if (!player.isGrounded && player.transform.localPosition.y > 0.1f)
        {
            player.gravityManager.ApplyGravity(player);
        }
        player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, player.gravityManager.GetVelocity(), 0);
//        Debug.Log(player.gravityManager.GetVelocity());
        
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        enforcedCooldown = false;
    }
}
