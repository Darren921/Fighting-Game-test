using System;
using UnityEngine;
[System.Serializable]

public class PlayerStateManager : MonoBehaviour
{
    public string CurrentStateName => currentState?.GetType().Name;
    [SerializeReference]  public PlayerBaseState currentState;
   internal  PlayerController playerController;
   internal PlayerNeutralState NeutralState = new PlayerNeutralState();
   internal PlayerCrouchingState CrouchingState = new PlayerCrouchingState();
   internal PlayerJumpingState JumpingState = new PlayerJumpingState();
   internal PlayerMovingState MovingState = new PlayerMovingState();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentState = NeutralState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this,playerController);
  //        print(currentState);
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState(this,playerController);
    }

    public void SwitchState(PlayerBaseState newState)
    {
        currentState.ExitState(this,playerController);
        currentState = newState;
        newState.EnterState(this);
    }
}
