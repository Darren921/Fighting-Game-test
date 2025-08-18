using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDashState : PlayerMovingState
{
    protected InputReader.MovementInputResult dir;
    protected Vector3 dashDir;
    protected Vector3 newDashVelo;
    protected float dashTime = 0.3f;
    protected float dashDistance = 3f;
    protected bool isDashing;
    private float jumpVelocity;
    private Coroutine dashCoroutine;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        dir  = player.DashDir;
        Debug.Log(dir);
        player.Rb.linearVelocity = Vector3.zero;
      Debug.Log("PlayerDashState EnterState");
      switch (dir)
      {
          case InputReader.MovementInputResult.None or  InputReader.MovementInputResult.Forward:
              dashDir =  !player.Reversed ? new Vector3(2, 0, 0 ) : new Vector3(-1, 0, 0);
              jumpVelocity = 0;
              break;
          case InputReader.MovementInputResult.Backward:
              dashDir =  !player.Reversed ? new Vector3(-1, 0f, 0 ) : new Vector3(2, 0f, 0);
              jumpVelocity = 5 ;
              break;
      }
      
      newDashVelo = dashDir * (dashDistance / dashTime);
      player.StartCoroutine(Dash(player));
    }

    private IEnumerator Dash(PlayerController player)
    {
        Debug.Log("PlayerDashState Dash");
        isDashing = true;
        player.Rb.linearVelocity = new Vector3(newDashVelo.x, jumpVelocity, 0);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        player.IsDashing = false;

    }


    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //grab the last inputs given 
        if (isDashing || player.IsDashing) return;
        player.StartCoroutine(DecelerationCurve(player));
        if (player.PlayerMove == Vector3.zero && decelerating == false)
        { 
            Debug.Log("HEH");
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateTypes.Neutral);
        }
        
        if(decelerating)return;
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Walking);

    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
       
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (dashCoroutine != null)
        {
            player.StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }
        player.IsDashing = false;
        
        Debug.Log("PlayerDashState ExitState");
    }
}
