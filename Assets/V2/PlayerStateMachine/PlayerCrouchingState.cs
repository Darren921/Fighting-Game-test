using UnityEngine;
public class PlayerCrouchingState : PlayerBaseState
{
    private bool isCrouching;
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        isCrouching = true;
        Debug.Log("Entering PlayerCrouchingState");
    }

    internal override void UpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
        if (playerController.playerMove.y == 0)
        {
            isCrouching = false;
        }
        if (playerController.playerMove is { y: 0, x: 0 })
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
        }

        if (!isCrouching && playerController.playerMove.x != 0)
        {
            Debug.Log("Switched to Move state (C.S)");
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Moving);
        }
          
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
    }

    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    {
        Debug.Log("Exiting PlayerCrouchingState");
    }
}
