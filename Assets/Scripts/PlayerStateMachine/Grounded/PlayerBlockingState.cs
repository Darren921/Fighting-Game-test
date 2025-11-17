using UnityEngine;
using System.Collections;

[System.Serializable]

//Transition checks are in playerstateManager use if they match your needs, example below, whatever transtion you need place in check for transition, else bool check in update 
// playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Walking | PlayerStateManager.PlayerStateTypes.Crouching);
public class PlayerBlockingState : PlayerBaseState
{
    private PlayerStateManager.PlayerStateTypes _returnState; 
    private Coroutine _blockCoroutine;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        Debug.Log("Blocked state triggered");
        
        player.Animator.SetBool(player.blocking, true);
        
        player.rb.linearVelocity = Vector3.zero;
        
        _blockCoroutine = player.StartCoroutine(BlockDuration(playerStateManager, player));
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (player.InputReader.CurrentMoveInput != InputReader.MovementInputResult.Backward && player.InputReader.CurrentMoveInput != InputReader.MovementInputResult.DownLeft)
        {
            playerStateManager.SwitchToLastState();
        }
    }

    private IEnumerator BlockDuration(PlayerStateManager playerStateManager, PlayerController player)
    {
        yield return new WaitForSeconds(0.2f);
        playerStateManager.SwitchToLastState();
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (!player.IsGrounded)
        {
            player.GravityManager.ApplyGravity(player);
            player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, player.GravityManager.GetVelocity(), 0);
        }
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (_blockCoroutine != null) player.StopCoroutine(_blockCoroutine);
        player.Animator.SetBool(player.blocking, false);
        player.PlayerHitDetection._hit = false;
    }
}
