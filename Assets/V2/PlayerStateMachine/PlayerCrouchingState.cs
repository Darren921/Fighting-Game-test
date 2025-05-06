using UnityEngine;

public class PlayerCrouchingState : PlayerBaseState
{
    private bool isCrouching;
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        Debug.Log("Entering PlayerCrouchingState");
    }

    internal override void UpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
        if (playerController.playerMove.y == 0)
        {
            playerStateManager.SwitchState(playerStateManager.NeutralState);
        }
        if (isCrouching && playerController.playerMove.x != 0)
            playerStateManager.SwitchState(playerStateManager.MovingState);
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController playerController)
    {
    }

    internal override void ExitState(PlayerStateManager playerStateManager,PlayerController player)
    {
        Debug.Log("Exiting PlayerCrouchingState");
    }
}
