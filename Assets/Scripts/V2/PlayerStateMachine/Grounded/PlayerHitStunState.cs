using System;
using System.Collections;
using UnityEngine;

public class PlayerHitStunState : PlayerBaseState
{

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
      
        player.StartCoroutine(WaitForHitStun(player));
        player.StartCoroutine(PerformHit(player));

    }

    private IEnumerator PerformHit(PlayerController player)
    {
        yield return new WaitUntil(() => !player.hitStun);
        switch (player.playerHitDetection.otherPlayer.InputReader.LastValidAttackInput)
        {
            case InputReader.AttackInputResult.Light:
                player.rb.linearVelocity = new Vector3(10, 0, 0);
                break;
            case InputReader.AttackInputResult.LightLeft:
                player.rb.linearVelocity = new Vector3(10, 0, 0);
                break;
            case InputReader.AttackInputResult.LightRight:
                player.rb.linearVelocity = new Vector3(10, 0, 0);
                break;
            case InputReader.AttackInputResult.Medium:
                player.rb.linearVelocity = new Vector3(20, 0, 0);
                break;
            case InputReader.AttackInputResult.MediumLeft:
                player.rb.linearVelocity = new Vector3(20, 0, 0);
                break;
            case InputReader.AttackInputResult.MediumRight:
                player.rb.linearVelocity = new Vector3(20, 0, 0);
                break;
            case InputReader.AttackInputResult.Heavy:
                break;
            case InputReader.AttackInputResult.HeavyLeft:
                break;
            case InputReader.AttackInputResult.HeavyRight:
                break;
        }

    }

    private IEnumerator WaitForHitStun(PlayerController player)
    {
        player.OnDisablePlayer();
        player.hitStun = true;
        Time.timeScale = 0f;
        yield return new WaitForFrames(3);
        player.OnEnablePlayer();
        Time.timeScale = 1f;
        player.hitStun = false;
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (!player.hitStun)
        {
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.Crouching | PlayerStateManager.PlayerStateTypes.Dash | PlayerStateManager.PlayerStateTypes.Jumping | PlayerStateManager.PlayerStateTypes.Walking);
        }
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
    }
    
    
    
    public class WaitForFrames : CustomYieldInstruction
    {
        private int _targetFrameCount;

        public WaitForFrames(int numberOfFrames)
        {
            _targetFrameCount = Time.frameCount + numberOfFrames;
        }

        public override bool keepWaiting
        {
            get
            {
                return Time.frameCount < _targetFrameCount;
            }
        }
    }
    
}
