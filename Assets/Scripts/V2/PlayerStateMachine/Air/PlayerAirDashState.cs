using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirDashState : PlayerDashState
{
    private  int AirDashCharges; 
    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        AirDashCharges = player.characterData.AirDashCharges;
        AirDashCharges--;
        player.StartCoroutine(AirDash(player));
//      Debug.Log(newDashVelo);
     
    }

    private void SetUpDash(PlayerController player)
    {
        dir  = player.DashDir;
        Debug.Log(dir);
        Debug.Log("PlayerDashState EnterState");
        switch (dir)
        {
            case InputReader.MovementInputResult.None or  InputReader.MovementInputResult.Forward:
                dashDir =  !player.Reversed ? new Vector3(1.5f, 0, 0 ) : new Vector3(-1.5f, 0, 0);
                Debug.Log(dashDir);
                break;
            case InputReader.MovementInputResult.Backward:
                dashDir =  !player.Reversed ? new Vector3(-1.5f, 0f, 0 ) : new Vector3(1.5f, 0f, 0);
                Debug.Log(dashDir);
                break;
        }
        newDashVelo = dashDir * (dashDistance / dashTime);
    }


    private IEnumerator AirDash(PlayerController player)
    {
        Debug.Log("PlayerDashState Dash");
        isDashing = true;
        SetUpDash(player);
        player.rb.useGravity = false;
        player.rb.linearVelocity = new Vector3(newDashVelo.x, 0, 0);
        yield return new WaitForSeconds(dashTime);
        player.rb.useGravity = true;
        isDashing = false;
        player.Dashing = false;

    }
    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
  //      if(!player.isGrounded ||   isDashing || player.Dashing) return;

        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack);
        if (AirDashCharges > 0 && player.Dashing && !isDashing) 
        {
            Debug.Log("PlayerDashState Dash again");
            AirDashCharges--;
            player.gravityManager.ResetVelocity();

            player.StartCoroutine(AirDash(player));
        }
        if (player.isGrounded)
        {
            playerStateManager.CheckForTransition( PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Walking );
        }
        Debug.Log(player.gravityManager.GetVelocity());
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
//        Debug.Log(player.gravityManager.GetVelocity());

        if (!player.isGrounded && player.gameObject.transform.localPosition.y > 0.1f && !isDashing )
        {
            player.gravityManager.ApplyGravity(player);
            
            player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, player.gravityManager.GetVelocity(), 0);

        }
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.Dashing = false;
    }
}
