using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PlayerMovingState : PlayerBaseState
{
    protected PlayerController _player; 
    protected Vector3 moveDir;
    protected Vector3 _smoothedMoveDir;
    protected Vector3 _smoothedMoveVelocity;
    protected bool decelerating;

    protected virtual float moveSpeed => 1;
    
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
//        Debug.Log("Entered " + playerStateManager.currentState);
        _player = player;
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        setMoveDir(new Vector2(player.playerMove.x, 0));
        smoothMovement();
        applyVelocity(player);
    }

    protected void applyVelocity(PlayerController player)
    {
        var velocity = new Vector3(_smoothedMoveDir.x * moveSpeed, player.rb.linearVelocity.y);
        if (Mathf.Abs(velocity.x) > 0.01f)
        {
            player.rb.linearVelocity = velocity;    
        }
    }

    protected void smoothMovement()
    {
        _smoothedMoveDir = Vector3.SmoothDamp(_smoothedMoveDir, moveDir, ref _smoothedMoveVelocity, 0.3f);
    }

    protected void setMoveDir(Vector3 newDir)
    {
        moveDir = newDir.normalized;
    }
    protected IEnumerator DecelerationCurve(PlayerController player)
    {
        if (decelerating) yield break;
        decelerating = true;

        while (player.rb.linearVelocity.magnitude > 0.1f)
        {
            var decelerationCurve = player.rb.linearVelocity.normalized * (2 * Time.deltaTime);
            Debug.Log(decelerationCurve);
            player.rb.linearVelocity -= decelerationCurve;
            yield return null;
        }
        decelerating = false;
    }

    private IEnumerator Delay(PlayerController player)
    {
        
        yield return new  WaitUntil(() => player.rb.linearVelocity.magnitude < 0.1f);
        decelerating = false;
    }
    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
      //   player.rb.linearVelocity = Vector3.zero;
        _smoothedMoveVelocity = Vector3.zero;
        _smoothedMoveDir = Vector3.zero;

    }
}
