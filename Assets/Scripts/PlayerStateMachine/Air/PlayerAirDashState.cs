using System.Collections;
using UnityEngine;
[System.Serializable]
public class PlayerAirDashState : PlayerDashState
{
    [field: SerializeField] private int _airDashCharges;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        _airDashCharges = player.CharacterData.airDashCharges;
        _airDashCharges--;
        player.StartCoroutine(AirDash(player));

//      Debug.Log(newDashVelo);
    }

    protected override void SetUpDash(PlayerController player)
    {
        Dir = player.DashDir;
        //   Debug.Log(dir);
        //    Debug.Log("PlayerDashState EnterState");
        switch (Dir)
        {
            case InputReader.MovementInputResult.None or InputReader.MovementInputResult.Forward:
                DashDir = !player.Reversed ? new Vector3(2f, 0, 0) : new Vector3(-2f, 0, 0);
                //          Debug.Log(dashDir);
                break;
            case InputReader.MovementInputResult.Backward:
                DashDir = !player.Reversed ? new Vector3(-2f, 0f, 0) : new Vector3(2f, 0f, 0);
                //          Debug.Log(dashDir);
                break;
        }

        NewDashVelo = DashDir * (DashDistance / DashTime);
    }


    private IEnumerator AirDash(PlayerController player)
    {
        //    Debug.Log("PlayerDashState Dash");
        IsDashing = true;
        SetUpDash(player);
        player.rb.useGravity = false;
        player.rb.linearVelocity = new Vector3(NewDashVelo.x, 0, 0);
        Debug.Log( player.rb.linearVelocity);
        yield return new WaitForSeconds(DashTime);
        player.GravityManager.ResetVelocity();
        player.rb.useGravity = true;
        IsDashing = false;
        player.IsDashing = false;
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //      if(!player.isGrounded ||   isDashing || player.Dashing) return;

        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Attack);
        if (_airDashCharges > 0 && player.IsDashing && !IsDashing && player.AtDashHeight)
        {
            //  Debug.Log("PlayerDashState Dash again");
            _airDashCharges--;
            player.GravityManager.ResetVelocity();

            player.StartCoroutine(AirDash(player));
        }

        if (player.IsGrounded)
        {
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral |
                                                  PlayerStateManager.PlayerStateTypes.Walking);
        }
        // Debug.Log(player.GravityManager.GetVelocity());
    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
//        Debug.Log(player.gravityManager.GetVelocity());

        if (!player.IsGrounded && player.gameObject.transform.localPosition.y > 0.1f && !IsDashing)
        {
            player.GravityManager.ApplyGravity(player);
            player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, player.GravityManager.GetVelocity(), 0);
        }
    }

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.IsDashing = false;
    }
}