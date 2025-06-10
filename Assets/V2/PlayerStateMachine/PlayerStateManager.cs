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
    [SerializeReference] internal PlayerBaseState currentState;
    internal  PlayerController player;
   


   void Awake()
   {
    
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
            currentState = state;
            currentState?.EnterState(this,player);

        }
      
    }
}
