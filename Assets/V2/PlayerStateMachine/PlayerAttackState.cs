
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerAttackState : PlayerBaseState
{
  


    private Dictionary<( InputReader.MovementInputResult, InputReader.AttackInputResult), InputReader.AttackInputResult> attackMoveActions = new()
    {
            { (InputReader.MovementInputResult.None,InputReader.AttackInputResult.Light), InputReader.AttackInputResult.Light },
            { (InputReader.MovementInputResult.Left,InputReader.AttackInputResult.Light), InputReader.AttackInputResult.LightLeft },
            { (InputReader.MovementInputResult.Right,InputReader.AttackInputResult.Light), InputReader.AttackInputResult.LightLeft },

            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.Medium), InputReader.AttackInputResult.Medium },
            { (InputReader.MovementInputResult.Left,InputReader.AttackInputResult.Medium), InputReader.AttackInputResult.MediumLeft },
            { (InputReader.MovementInputResult.Right,InputReader.AttackInputResult.Medium), InputReader.AttackInputResult.MediumRight },

            { ( InputReader.MovementInputResult.None,InputReader.AttackInputResult.Heavy), InputReader.AttackInputResult.Heavy },

    };
    bool Airborne  = false;
    
    
    
    
    internal override void EnterState(PlayerStateManager playerStateManager,PlayerController player)
    {
        playerStateManager.player.animator.SetBool(playerStateManager.player.Attacking, true);
    }
    private void Light(PlayerController player, InputReader.MovementInputResult move)
    {           
        Debug.Log("light");
        player.animator.SetBool(player.Light,true);
        if  (!player.animator.IsInTransition(0))
        {
           Debug.Log(move);
            switch (move)
            {
                case InputReader.MovementInputResult.Right or InputReader.MovementInputResult.UpRight:
                    player.animator.SetBool(player.Right, true);
                    break; 
                case InputReader.MovementInputResult.Left or InputReader.MovementInputResult.UpLeft:
                    player.animator.SetBool(player.Left, true);
                    break;
                case InputReader.MovementInputResult.None:
                    break;
            }
        }
    }
  

    private void Medium(PlayerController player, InputReader.MovementInputResult move)
    {
        Debug.Log("light");

        player.animator.SetBool(player.Medium,true);
        if  (!player.animator.IsInTransition(0))
        {
            switch (move)
            {
                case InputReader.MovementInputResult.Right:
                    player.animator.SetBool(player.Right, true);
                    break;
                case InputReader.MovementInputResult.Left:
                    player.animator.SetBool(player.Left, true);
                    break;
                case InputReader.MovementInputResult.None:
                    break;
            }
        }

    }
    
    private void Heavy(PlayerController player, InputReader.MovementInputResult move)
    {
        
        Debug.Log("Heavy");
        
    }

  

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.isGrounded = player.gravityManager.CheckifGrounded(player);

        if (!player.isGrounded)
        {
            player.animator.SetBool(player.Airborne,true);
            player.gravityManager.ApplyGravity(player);
            
            player.transform.Translate(new Vector3(0,player.gravityManager.GetVelocity(), 0) * Time.deltaTime);

        }
        else
        {
            player.animator.SetBool(player.Airborne,false);
            player.gravityManager.ResetVelocity();
            Vector3 closestPoint = player.gravityManager.raycastHit.collider.ClosestPoint(player.transform.position);
            Vector3 snappedPosition = new Vector3(player.transform.position.x, closestPoint.y + 1, player.transform.position.z);
            player.transform.position = snappedPosition;
        }

       
         
        
        var inputReader = playerStateManager.player.InputReader;
        var lastmove = inputReader.GetLastInput();
        var lastattack = inputReader.GetLastAttackInput();
        Debug.Log(lastmove.ToString());
        Debug.Log(lastattack.ToString());

        /*if ( attackMoveActions.TryGetValue((lastmove, lastattack), out var action))
        {
    
            switch (action)
            {
                case InputReader.AttackInputResult.Light:
                    Light(player);
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
                default:
                    Debug.Log(action.ToString());
                    break;
            }
        }*/

        switch (lastattack)
        {
            case InputReader.AttackInputResult.Light:
                Light(player,lastmove);
                break;
            case InputReader.AttackInputResult.Medium:
                Medium(player,lastmove);
                break;
            case InputReader.AttackInputResult.Heavy:
                break;
        }
        if(player.playerMove == Vector2.zero && !player.IsAttacking)
        {
            Debug.Log("going to neut");
            playerStateManager.SwitchState( PlayerStateManager.PlayerStateType.Neutral);
        }
        else if (player.playerMove != Vector2.zero && !player.IsAttacking)
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
