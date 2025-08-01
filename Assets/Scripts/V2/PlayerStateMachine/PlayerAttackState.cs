
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
           if (player.animator.GetBool(player.Idle))
           {
               player.animator.SetBool(player.Idle, false);
           }

           cooldownCoroutine = player.StartCoroutine(EnforceCooldown(player));
           
           lastMove = player.InputReader.currentMoveInput;
           lastAttack = player.InputReader.currentAttackInput;
           Debug.Log(lastAttack);
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
        if (player.animator.IsInTransition(0) && player.onAttackCoolDown ) return;

        player.isGrounded = player.gravityManager.CheckifGrounded(player);
        
        player.animator.SetBool(player.Airborne, !player.isGrounded);
    
        // choose attack based on input and any movement detected 
        if (player.IsAttacking)
        {
            switch (lastAttack)
            {
                case InputReader.AttackInputResult.Light:
                    Light(player,lastMove);
                    break;
                case InputReader.AttackInputResult.Medium:
                    Medium(player,lastMove);
                    break;
                case InputReader.AttackInputResult.Heavy:
                    break;
            }

        }
        // State swapping 
        if (!player.isGrounded) return;
        if(player.playerMove == Vector3.zero && !player.IsAttacking)
        {
            Debug.Log("going to neut");
            playerStateManager.SwitchState( PlayerStateManager.PlayerStateType.Neutral);
        }
        else if (player.IsWalking && !player.IsAttacking )
        {
            Debug.Log("going to moving");
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Walking);
        }
        else if(player.isCrouching && !player.IsAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);

        }





    }

    
    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
      
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        
    }
}
