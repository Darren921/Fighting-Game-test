using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerStateManager : MonoBehaviour
{
    public enum PlayerStateType {
        Neutral,
        Crouching,
        Jumping,
        Moving,
        Attack,
        CrouchMove,
        Dash
    }

    private  Dictionary<PlayerStateType, PlayerBaseState> _states;
    public string CurrentStateName => currentState?.GetType().Name; 
    [SerializeReference] internal PlayerBaseState currentState;
    [SerializeReference] internal PlayerBaseState previousState;
    internal  PlayerController player;
   


   void Awake()
   {
    _states = new ()
    {
        { PlayerStateType.Neutral, new PlayerNeutralState() },
        { PlayerStateType.Crouching, new PlayerCrouchingState() },
        { PlayerStateType.Jumping, new PlayerJumpingState() },
        { PlayerStateType.Moving, new PlayerMovingState() },
        { PlayerStateType.Attack, new PlayerAttackState() },
        { PlayerStateType.CrouchMove , new PlayerCrouchMoveState()},
        { PlayerStateType.Dash , new PlayerDashState() }
    };
   }
   
    void Start()
    {
        player = GetComponent<PlayerController>();
        currentState = _states[PlayerStateType.Neutral];
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

    public void SwitchState(PlayerStateType newType)
    {
        if (_states.TryGetValue(newType, out var state))
        {
            currentState?.ExitState(this,player);
            previousState = currentState;
            currentState = state;
            currentState?.EnterState(this,player);

        }
      
    }

    public void SwitchToLastState()
    {
        currentState?.ExitState(this,player);
        var temp = currentState;
        currentState = previousState;
        currentState?.EnterState(this,player);
        
    }
    
}
