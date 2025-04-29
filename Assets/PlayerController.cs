using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerController : MonoBehaviour, Controls.IPlayerActions
{
    
    private Controls controls; // Source code representation of asset.
    private Controls.PlayerActions m_Player; // Source code representation of action map.

    private PlayerMovement playerMovement;
    private InputReader _inputReader;

    internal  float playerMoveX;
    internal float playerMoveY;

    public enum  playerState
    {
        Neutral,
        Moving,
        Jumping,
        Crouching,
    }

    void Awake()
    {
        _inputReader = GetComponent<InputReader>();
        playerMovement = GetComponent<PlayerMovement>();
        controls = new Controls(); // Create asset object.
        m_Player = controls.Player; // Extract action map object.
        m_Player.AddCallbacks(this); // Register callback interface IPlayerActions.
    }

    void OnDestroy()
    {
        controls.Dispose(); // Destroy asset object.
    }

    void OnEnable()
    {
        m_Player.Enable(); // Enable all actions within map.
    }
    void OnDisable()
    {
        m_Player.Disable();   // Disable all actions within map.
    }



   
    

    public void OnMove(InputAction.CallbackContext context)
    {
      
    }

    public void OnMoveX(InputAction.CallbackContext context)
    {
        playerMoveX = context.ReadValue<float>();
        playerMovement.setMoveDir(new Vector2(playerMoveX, 0));
        StartCoroutine(playerMoveX > 0
            ? _inputReader.AddInput(InputReader.InputResult.Right, Time.frameCount)
            : _inputReader.AddInput(InputReader.InputResult.Left, Time.frameCount));
    }

    public void OnMoveY(InputAction.CallbackContext context)
    {
        playerMoveY = context.ReadValue<float>();
        StartCoroutine(playerMoveY > 0
            ? _inputReader.AddInput(InputReader.InputResult.Up, Time.frameCount)
            : _inputReader.AddInput(InputReader.InputResult.Down,Time.frameCount));
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
      //  throw new System.NotImplementedException();
    }
    

    public void OnSprint(InputAction.CallbackContext context)
    {
     //   throw new System.NotImplementedException();
    }

 
      


   
}
