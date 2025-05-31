
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Controls.IPlayerActions
{
    public int Attacking => Animator.StringToHash("Attacking");
    public int Light => Animator.StringToHash("Light");
    public int Kick => Animator.StringToHash("Kick");
    public int Move = Animator.StringToHash("Move");

    private Controls controls;
    private Controls.PlayerActions m_Player;
    internal InputReader _inputReader;
    internal PlayerStateManager stateManager;
   [SerializeField] private CharacterSO characterData;
    private Coroutine AttackCheck; 
    public Vector2 playerMove {get; private set;}
    internal Animator animator;
    internal bool isGrounded;
    
    #region Attack Check Variables  
    public bool isAttacking{get; internal set;}
    public bool isPunching {get; private set;}
    public bool isKicking {get; private set;}
    public bool isSlashing {get; private set;}
    
    public bool isHeavySlashing {get; private set;}
    #endregion

    internal bool reversed;
    
    #region Changeable Move Variables
    internal float _moveSpeed;
    #endregion
    

    #region Changeable Jump Variables

    internal float jumpHeight; //Switch to player character data S.O when created 5 
    internal float raycastDistance = 1;  // 1
    internal float gravScale; // (Hold for now )  character data affects gravity 5 
    internal  float velocity;
    internal Rigidbody rb;

    #endregion
    
    void Awake()
    {
        _inputReader = GetComponent<InputReader>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<PlayerStateManager>();
      
    }
    
    public void InitializePlayer(InputDevice device)
    {
        controls = new Controls();
        controls.devices = new[] { device };
        m_Player = controls.Player;

        m_Player.Enable();
        m_Player.SetCallbacks(this);

        SetUpCharacterVariables();
    }
    private void SetUpCharacterVariables()
    {
        jumpHeight = characterData.jumpHeight;
        gravScale = characterData.gravScale;
        _moveSpeed = characterData.moveSpeed;
    }
    
    void OnDisable() => m_Player.Disable();
    public void SetAttacking()
    {
       isAttacking = false;
       animator.SetBool(Attacking, false);
       animator.SetBool(Light , false);
       animator.SetBool(Kick , false);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        
    }    

    private void Update()
    {
        
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        playerMove = context.ReadValue<Vector2>();
        // print(playerMove);    
    }

    public void OnMoveX(InputAction.CallbackContext context)
    {
        // playerMoveX = context.ReadValue<float>();
    }

    public void OnMoveY(InputAction.CallbackContext context)
    {
        // playerMoveY = context.ReadValue<float>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        var attackVal = ReturnAttackType(context.ReadValue<float>());
        AttackCheck = StartCoroutine(_inputReader.AddAttackInput(attackVal, Time.frameCount));
        isAttacking = true;
        
    }

    private InputReader.AttackInputResult ReturnAttackType(float attackVal)
    {
        var attackValAsInt = (int) attackVal;
        var attackResult = attackValAsInt switch
        {
            1 => InputReader.AttackInputResult.Light,
            2 => InputReader.AttackInputResult.Medium,
            3 => InputReader.AttackInputResult.Heavy,
            _ => InputReader.AttackInputResult.None
        };

        return attackResult ;
    }
}
