using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerNeutralState : PlayerBaseState
{
    
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
    }

    internal override void UpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
        if (playerController.playerMove !=  Vector2.zero)
        {
            playerStateManager.SwitchState(playerStateManager.MovingState);
        }
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
       
    }

    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    {
        Debug.Log("Exit PlayerNeutralState");
    }
}
