using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerMovingState
{
    protected InputReader.MovementInputResult Dir;
    protected Vector3 DashDir;
    protected Vector3 NewDashVelo;
    protected const float DashTime = 0.3f;
    protected const float DashDistance = 3f;
    protected bool IsDashing;
    private float _jumpVelocity;
    private Coroutine _dashCoroutine;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        SetUpDash(player);
        player.StartCoroutine(Dash(player));
    }

    protected virtual void SetUpDash(PlayerController player)
    {
        Dir  = player.DashDir;
        Debug.Log(Dir);
        Debug.Log("PlayerDashState EnterState");
        switch (Dir)
        {
            case InputReader.MovementInputResult.None or  InputReader.MovementInputResult.Forward:
                DashDir =  !player.Reversed ? new Vector3(2, 0, 0 ) : new Vector3(-1, 0, 0);
                _jumpVelocity = 0;
                break;
            case InputReader.MovementInputResult.Backward:
                DashDir =  !player.Reversed ? new Vector3(-1, 0f, 0 ) : new Vector3(2, 0f, 0);
                _jumpVelocity = 5 ;
                break;
        }
      
        NewDashVelo = DashDir * (DashDistance / DashTime);
    }

    private IEnumerator Dash(PlayerController player)
    {
        Debug.Log("PlayerDashState Dash");
        IsDashing = true;
        player.rb.linearVelocity = new Vector3(NewDashVelo.x, _jumpVelocity, 0);
        Debug.Log(player.rb.linearVelocity);
        yield return new WaitForSeconds(DashTime);
        IsDashing = false;
        player.IsDashing = false;

    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
    }

    protected override void ApplyVelocity(PlayerController player)
    {
    }


    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //grab the last inputs given 
        if (IsDashing || player.IsDashing ) return;

        Debug.Log("HEH"); 
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Attack | PlayerStateManager.PlayerStateTypes.Walking);

    }

    

    internal override void ExitState(PlayerStateManager playerStateManager, PlayerController player)
    {
        if (_dashCoroutine != null)
        {
            player.StopCoroutine(_dashCoroutine);
            _dashCoroutine = null;
        }
        player.IsDashing = false;
        
        Debug.Log("PlayerDashState ExitState");
    }
}
