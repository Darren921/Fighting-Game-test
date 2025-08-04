using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    private InputReader.MovementInputResult dir;
    private Vector3 dashDir;
    private Vector3 newDashVelo;
    private float dashTime = 0.3f;
    private float dashDistance = 3f;
    private bool isDashing;
    private float jumpVelocity;
    private Coroutine dashCoroutine;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        dir  = player.DashDir;
        player.rb.linearVelocity = Vector3.zero;
      Debug.Log("PlayerDashState EnterState");
      switch (dir)
      {
          case InputReader.MovementInputResult.None or  InputReader.MovementInputResult.Forward:
              dashDir =  !player.Reversed ? new Vector3(2, 0, 0 ) : new Vector3(-1, 0, 0);
              jumpVelocity = 0;
              break;
          case InputReader.MovementInputResult.Backward:
              dashDir =  !player.Reversed ? new Vector3(-1, 0f, 0 ) : new Vector3(2, 0f, 0);
              jumpVelocity = 3 ;
              break;
      }
      
      newDashVelo = dashDir * (dashDistance / dashTime);
//      Debug.Log(newDashVelo);
      player.StartCoroutine(Dash(player));
    }

    private IEnumerator Dash(PlayerController player)
    {
        Debug.Log("PlayerDashState Dash");
        isDashing = true;
        player.rb.linearVelocity = new Vector3(newDashVelo.x, jumpVelocity, 0);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        player.Dashing = false;

    }


    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
   
        //grab the last inputs given 
        if(isDashing || player.Dashing) return;
        if (player.playerMove.x == 0) playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
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
        player.Dashing = false;
        
        Debug.Log("PlayerDashState ExitState");
    }
}
