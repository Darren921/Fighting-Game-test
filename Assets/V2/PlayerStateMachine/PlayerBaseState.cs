[System.Serializable]
public abstract class PlayerBaseState
{
 internal  abstract void EnterState(PlayerStateManager playerStateManager);
 internal abstract void UpdateState(PlayerStateManager playerStateManager,PlayerController player);

 internal abstract void FixedUpdateState(PlayerStateManager playerStateManager,PlayerController player);
 
 internal abstract void ExitState(PlayerStateManager playerStateManager,PlayerController player);
 
}
