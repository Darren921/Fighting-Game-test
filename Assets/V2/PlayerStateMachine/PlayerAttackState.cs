using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        Debug.Log("Entered PlayerPunchingState");
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        throw new System.NotImplementedException();
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        throw new System.NotImplementedException();
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        throw new System.NotImplementedException();
    }
}
