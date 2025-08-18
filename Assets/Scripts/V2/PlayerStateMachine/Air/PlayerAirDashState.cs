using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirDashState : PlayerDashState
{
    private  int _airDashCharges; 
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        _airDashCharges = player.characterData.airDashCharges;
        _airDashCharges--;
        player.StartCoroutine(AirDash(player));
//      Debug.Log(newDashVelo);
     
    }

    private void SetUpDash(PlayerController player)
    {
        dir  = player.DashDir;
     //   Debug.Log(dir);
    //    Debug.Log("PlayerDashState EnterState");
        switch (dir)
        {
            case InputReader.MovementInputResult.None or  InputReader.MovementInputResult.Forward:
                dashDir =  !player.Reversed ? new Vector3(1.5f, 0, 0 ) : new Vector3(-1.5f, 0, 0);
      //          Debug.Log(dashDir);
                break;
            case InputReader.MovementInputResult.Backward:
                dashDir =  !player.Reversed ? new Vector3(-1.5f, 0f, 0 ) : new Vector3(1.5f, 0f, 0);
      //          Debug.Log(dashDir);
                break;
        }
        newDashVelo = dashDir * (dashDistance / dashTime);
    }


    private IEnumerator AirDash(PlayerController player)
    {
    //    Debug.Log("PlayerDashState Dash");
        isDashing = true;
        SetUpDash(player);
        player.Rb.useGravity = false;
        player.Rb.linearVelocity = new Vector3(newDashVelo.x, 0, 0);
        yield return new WaitForSeconds(dashTime);
        player.GravityManager.ResetVelocity();
        player.Rb.useGravity = true;
        isDashing = false;
        player.IsDashing = false;

    }
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
  //      if(!player.isGrounded ||   isDashing || player.Dashing) return;

        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack);
        if (_airDashCharges > 0 && player.IsDashing && !isDashing) 
        {
          //  Debug.Log("PlayerDashState Dash again");
            _airDashCharges--;
            player.GravityManager.ResetVelocity();

            player.StartCoroutine(AirDash(player));
        }
        if (player.IsGrounded)
        {
            playerStateManager.CheckForTransition( PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Walking );
        }
       // Debug.Log(player.GravityManager.GetVelocity());
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
//        Debug.Log(player.gravityManager.GetVelocity());

        if (!player.IsGrounded && player.gameObject.transform.localPosition.y > 0.1f && !isDashing )
        {
            player.GravityManager.ApplyGravity(player);
            player.Rb.linearVelocity = new Vector3(player.Rb.linearVelocity.x, player.GravityManager.GetVelocity(), 0);

        }
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.IsDashing = false;
    }
}
