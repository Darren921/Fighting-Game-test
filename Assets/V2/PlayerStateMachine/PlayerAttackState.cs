
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerAttackState : PlayerBaseState
{
   

    private Dictionary<( InputReader.MovementInputResult, InputReader.AttackInputResult), InputReader.AttackInputResult> attackMoveActions = new()
    {
            { (InputReader.MovementInputResult.None,InputReader.AttackInputResult.Light), InputReader.AttackInputResult.Light },
            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.Medium), InputReader.AttackInputResult.Medium },
            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.Heavy), InputReader.AttackInputResult.Heavy },

    };
    
    
    
    
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        playerStateManager.player.animator.SetTrigger(playerStateManager.player.Attacking);
    }

    private void Slash(PlayerController player)
    {
        
        Debug.Log("Slash");
        
    }

    private void Kick(PlayerController player)
    {
        Debug.Log("Kick");
        player.animator.SetBool(player.Kick,true);

    }

    private void Punch(PlayerController player)
    {
        Debug.Log("Punch");
        player.animator.SetBool(player.Punch,true);
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        var inputReader = playerStateManager.player._inputReader;
        var lastmove = inputReader.GetLastInput();
        var lastattack = inputReader.GetLastAttackInput();
     
        if (  attackMoveActions.TryGetValue((lastmove, lastattack), out var action))
        {
    
            switch (action)
            {
                case InputReader.AttackInputResult.Light:
                    Punch(player);
                    break;
                case InputReader.AttackInputResult.Medium:
                    Kick(player);
                    break;
                case InputReader.AttackInputResult.Heavy:
                    Slash(player);
                    break;
                case InputReader.AttackInputResult.None:
                    Debug.Log("No action");
                    break;
            }
        }
        if(player.playerMove == Vector2.zero && !player.isAttacking)
        {
            Debug.Log("going to neut");
            playerStateManager.SwitchState( PlayerStateManager.PlayerStateType.Neutral);
        }
        else if (player.playerMove != Vector2.zero && !player.isAttacking)
        {
            Debug.Log("going to moving");
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Moving);
        }

        

       
           
    }

    
    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
       
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
    

    }
}
