using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAttackState : PlayerBaseState
{
    private Dictionary<( InputReader.MovementInputResult, InputReader.AttackInputResult), InputReader.AttackInputResult> attackMoveActions = new()
    {
            { (InputReader.MovementInputResult.None,InputReader.AttackInputResult.Punch), InputReader.AttackInputResult.Punch },
            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.Kick), InputReader.AttackInputResult.Kick },
            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.Slash), InputReader.AttackInputResult.Slash },
            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.HeavySlash), InputReader.AttackInputResult.HeavySlash },

    };
    
    bool isOnCooldown ;
    
    
    
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
       isOnCooldown = false;
    }

    private void Slash(PlayerController player)
    {
        
        Debug.Log("Slash");
        
    }

    private void Kick(PlayerController player)
    {
        Debug.Log("Kick");

    }

    private void Punch(PlayerController player)
    {
        Debug.Log("Punch");
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        var inputReader = playerStateManager.playerController._inputReader;
        var Lastmove = inputReader.GetLastInput();
        var Lastattack = inputReader.GetLastAttackInput();
     
        if (  attackMoveActions.TryGetValue((Lastmove, Lastattack), out var action))
        {
    
            switch (action)
            {
                case InputReader.AttackInputResult.Punch:
                    Punch(player);
                    break;
                case InputReader.AttackInputResult.Kick:
                    Kick(player);
                    break;
                case InputReader.AttackInputResult.Slash:
                    Slash(player);
                    break;
                case InputReader.AttackInputResult.HeavySlash:
                    break;
                case InputReader.AttackInputResult.None:
                    Debug.Log("No action");
                    break;
            }
        }
            if (player.playerMove == Vector2.zero)
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
            else
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Moving);
     
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
       
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.isAttacking = false;
    }
}
