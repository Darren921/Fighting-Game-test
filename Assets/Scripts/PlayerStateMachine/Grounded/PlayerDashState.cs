using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDashState : PlayerMovingState
{
    protected InputReader.MovementInputResult Dir;
    protected Vector3 DashDir;
    protected Vector3 NewDashVelo;
    protected float dashTime = 0.3f;
    protected float dashDistance = 3f;
    protected bool IsDashing;
    private float _jumpVelocity;
    private Coroutine _dashCoroutine;

    internal override void EnterState(PlayerStateManager playerStateManager, PlayerController player)
    {
        Dir  = player.InputReader.currentMoveInput;
        Debug.Log(Dir);
        player.Rb.linearVelocity = Vector3.zero;
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
      
      NewDashVelo = DashDir * (dashDistance / dashTime);
      player.StartCoroutine(Dash(player));
    }

    private IEnumerator Dash(PlayerController player)
    {
        Debug.Log("PlayerDashState Dash");
        IsDashing = true;
        player.Rb.linearVelocity = new Vector3(NewDashVelo.x, _jumpVelocity, 0);
        yield return new WaitForSeconds(dashTime);
        IsDashing = false;
        player.IsDashing = false;

    }


    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {

        //grab the last inputs given 
        if (IsDashing || player.IsDashing) return;
        player.StartCoroutine(DecelerationCurve(player));

        if (player.PlayerMove == Vector3.zero && decelerating == false)
        { 
            Debug.Log("HEH");
            playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Neutral | PlayerStateManager.PlayerStateTypes.Attack);
        }
        
        if(decelerating)return;
        playerStateManager.CheckForTransition(PlayerStateManager.PlayerStateTypes.Walking );

    }

    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
       
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
