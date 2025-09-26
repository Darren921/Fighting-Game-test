using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class PlayerHitStunState : PlayerBaseState
{ 
  
    
    
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.StartCoroutine(WaitForHitStun(player));
    }

   
   
   

    private IEnumerator WaitForHitStun(PlayerController player)
    {
        player.OnDisablePlayer();
        player.HitStun = true;
        Time.timeScale = 0f;
        Debug.Log("HitStun");
        yield return new WaitForSecondsRealtime(0.2f);
        Debug.Log("HitStun complete");
        player.OnEnablePlayer();
        Time.timeScale = 1f;
        player.HitStun = false;
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (!player.HitStun && !player.PlayerHitDetection.otherPlayer.IsActiveFrame)
        {
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.Crouching | PlayerStateManager.PlayerStateTypes.Dash | PlayerStateManager.PlayerStateTypes.Jumping | PlayerStateManager.PlayerStateTypes.Walking);
        }
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.PlayerHitDetection._hit = false;
    }
    
    
    
    public class WaitForFrames : CustomYieldInstruction
    {
        private readonly int _targetFrameCount;

        public WaitForFrames(int numberOfFrames)
        {
            _targetFrameCount = Time.frameCount + numberOfFrames;
        }

        public override bool keepWaiting => Time.frameCount < _targetFrameCount;
    }
    
}
