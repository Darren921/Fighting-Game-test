using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PlayerStateManager : MonoBehaviour
{
    [Flags]
    public enum PlayerStateTypes {
        None = 0,
        Neutral = 1 << 0,
        Crouching = 1 << 1,
        Jumping = 1 << 2,
        Walking = 1 << 3,
        Running = 1 << 4,
        Attack = 1 << 5,
        CrouchMove = 1 << 6,
        Dash = 1 << 7,
        AirDash = 1 << 8,
        HitStun = 1 << 9,
    }

    private  Dictionary<PlayerStateTypes, PlayerBaseState> _states;
    public string CurrentStateName => currentState?.GetType().Name; 

    [SerializeReference] internal PlayerBaseState currentState;
    [SerializeReference] internal PlayerBaseState lastState;
    PlayerController player;
  

   void Awake()
   {
       //This dictionary makes it that each state is available and not duped 
    _states = new ()
    {
        { PlayerStateTypes.Neutral, new PlayerNeutralState() },
        { PlayerStateTypes.Crouching, new PlayerCrouchingState() },
        { PlayerStateTypes.Jumping, new PlayerJumpingState() },
        { PlayerStateTypes.Walking, new PlayerWalkingState() },
        { PlayerStateTypes.Running, new PlayerRunningState()}, /////
        { PlayerStateTypes.Attack, new PlayerAttackState() },
        { PlayerStateTypes.CrouchMove , new PlayerCrouchMoveState()},
        { PlayerStateTypes.Dash , new PlayerDashState() },
        { PlayerStateTypes.AirDash , new PlayerAirDashState() },
        { PlayerStateTypes.HitStun , new PlayerHitStunState()}
        
    };
   }
   
    void Start()
    {
        player = GetComponent<PlayerController>();
        currentState = _states[PlayerStateTypes.Neutral];
        currentState.EnterState(this,player);
    }

    void Update()
    {
        currentState.UpdateState(this,player);
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState(this,player);
    }

    public void SwitchState(PlayerStateTypes newType)
    {
        if (_states.TryGetValue(newType, out var state))
        {
            currentState?.ExitState(this,player);
            lastState = currentState;
            currentState = state;
            currentState?.EnterState(this,player);

        }
    }

    public void CheckForTransition(PlayerStateTypes transitionType)
    {
        if (transitionType.HasFlag(PlayerStateTypes.Neutral))
            if (player.PlayerMove == Vector3.zero && player.IsGrounded)
            {
//                print($"Fired from {currentState}");
                SwitchState(PlayerStateTypes.Neutral);
            }
        
        if (transitionType.HasFlag(PlayerStateTypes.AirDash)) if (player.IsDashing && player.AtDashHeight) SwitchState(PlayerStateTypes.AirDash);
        
        if(transitionType.HasFlag(PlayerStateTypes.Jumping)) if (player.PlayerMove.y > 0  ) SwitchState(PlayerStateTypes.Jumping);
        
        if(transitionType.HasFlag(PlayerStateTypes.Walking)) if (player.PlayerMove.x != 0) SwitchState(PlayerStateTypes.Walking);
        
        if(transitionType.HasFlag(PlayerStateTypes.Running)) if (player.IsRunning  && !player.IsDashing ) SwitchState(PlayerStateTypes.Running);

        if(transitionType.HasFlag(PlayerStateTypes.Dash)) if (player.IsDashing  && !player.IsRunning ) SwitchState(PlayerStateTypes.Dash);

        if (transitionType.HasFlag(PlayerStateTypes.Attack)) if (player.IsAttacking && !player.OnAttackCoolDown) SwitchState(PlayerStateTypes.Attack);
       
        if (transitionType.HasFlag(PlayerStateTypes.CrouchMove))if (player.IsCrouching && player.PlayerMove.x != 0 && !player.IsAttacking) SwitchState(PlayerStateTypes.CrouchMove);
        

        if (transitionType.HasFlag(PlayerStateTypes.Crouching)) if (player.PlayerMove.y < 0 && player.IsGrounded) SwitchState(PlayerStateTypes.Crouching);
        
    }

    public void SwitchToLastState()
    {
        currentState?.ExitState(this,player);
        var temp = currentState;
        currentState = lastState;
        currentState?.EnterState(this,player);
        
    }
    
}
