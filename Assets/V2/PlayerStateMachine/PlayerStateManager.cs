using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStateManager : MonoBehaviour
{
    public enum PlayerStateType {
        Neutral,
        Crouching,
        Jumping,
        Moving,
        Attack
    }
    private readonly Dictionary<PlayerStateType, PlayerBaseState> _states = new ()
    {
        { PlayerStateType.Neutral, new PlayerNeutralState() },
        { PlayerStateType.Crouching, new PlayerCrouchingState() },
        { PlayerStateType.Jumping, new PlayerJumpingState() },
        { PlayerStateType.Moving, new PlayerMovingState() },
        { PlayerStateType.Attack, new PlayerAttackState() },  
    };
    public string CurrentStateName => currentState?.GetType().Name; 
    private PlayerBaseState currentState;
    internal  PlayerController playerController;
   


   void Awake()
   {
    
   }
   
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentState = _states[PlayerStateType.Neutral];
        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this,playerController);
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState(this,playerController);
    }

    public void SwitchState(PlayerStateType newType)
    {
        if (_states.TryGetValue(newType, out var state))
        {
            currentState?.ExitState(this,playerController);
            currentState = state;
            currentState?.EnterState(this);

        }
      
    }
}
