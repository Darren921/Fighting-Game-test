using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputManger : MonoBehaviour, Controls.IPlayerActions
{
    
    private Controls controls; // Source code representation of asset.
    private Controls.PlayerActions m_Player; // Source code representation of action map.
    private Vector3 rawInputMovement;
    private Vector3 smoothedMovement;

    
    private PlayerMovement playerMovement;
    
    
    void Awake()
    {
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
        var RawInput = context.ReadValue<Vector2>();
        
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
      //  throw new System.NotImplementedException();
    }
    

    public void OnSprint(InputAction.CallbackContext context)
    {
     //   throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        playerMovement.TryJump();
    }
}
