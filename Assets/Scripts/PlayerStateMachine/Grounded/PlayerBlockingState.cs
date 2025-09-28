using UnityEngine;

public class PlayerBlockingState : PlayerBaseState
{
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        throw new System.NotImplementedException();
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
    
    
    //Transition checks are in playerstateManager use if they match your needs, example below, whatever transtion you need place in check for transition, else bool check in update 
    // playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Walking | PlayerStateManager.PlayerStateTypes.Crouching);

}
