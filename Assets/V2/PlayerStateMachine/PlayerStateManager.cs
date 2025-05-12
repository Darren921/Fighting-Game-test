using System;
using UnityEngine;
public class PlayerStateManager : MonoBehaviour
{
    public string CurrentStateName => currentState?.GetType().Name; 
    private PlayerBaseState currentState;
    internal  PlayerController playerController;
   
   internal PlayerNeutralState NeutralState = new();
   internal PlayerCrouchingState CrouchingState = new();
   internal PlayerJumpingState JumpingState = new();
   internal PlayerMovingState MovingState = new();
   
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentState = NeutralState;
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

    public void SwitchState(PlayerBaseState newState)
    {
        currentState?.ExitState(this,playerController);
        currentState = newState;
        newState?.EnterState(this);
    }
}
