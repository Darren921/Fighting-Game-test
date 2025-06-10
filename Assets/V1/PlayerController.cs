
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Controls.IPlayerActions
{
    public   int Attacking => Animator.StringToHash("Attacking");
    public int Light => Animator.StringToHash("Light");
    public int Medium => Animator.StringToHash("Med");
    public int Move = Animator.StringToHash("Move");
    public int Left = Animator.StringToHash("Left");
    public  int Right = Animator.StringToHash("Right");
    public  int Airborne = Animator.StringToHash("Airborne");
    private Controls controls;
    private Controls.PlayerActions m_Player;
    internal InputReader InputReader;
    internal PlayerStateManager stateManager;
   [SerializeField] private CharacterSO characterData;
    private Coroutine AttackCheck; 
    public Vector2 playerMove {get; private set;}
    internal Animator animator;
    internal bool isGrounded;
    
    #region Attack Check Variables  
    public bool IsAttacking{get; private set;}
    public bool IsPunching {get; private set;}
    public bool IsKicking {get; private set;}
    public bool IsSlashing {get; private set;}
    
    public bool IsHeavySlashing {get; private set;}
    #endregion

    internal bool Reversed;
    
    #region Changeable Move Variables
    internal float WalkSpeed ;
    internal float RunSpeed ;
    internal bool IsWalking;
    internal bool IsRunning;
    #endregion
    InputAction _runAction;

    #region Changeable Jump Variables

    internal float jumpHeight; //Switch to player character data S.O when created 5 
    internal float raycastDistance = 1;  // 1
    internal float gravScale; // (Hold for now )  character data affects gravity 5 
    internal  float velocity;
    internal Rigidbody rb;

    #endregion
    internal bool isCrouching;

    internal GravityManager gravityManager;
    void Awake()
    {
        gravityManager = GetComponent<GravityManager>();
        isGrounded = true;
        InputReader = GetComponent<InputReader>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<PlayerStateManager>();
      
    }
    
    public void InitializePlayer(InputDevice device)
    {

        controls = new Controls();
        controls.devices = new[] { device };
        m_Player = controls.Player;
        m_Player.Move.performed += OnMove; 
        m_Player.Move.canceled += OnMove;
        m_Player.Attack.performed += OnAttack;
        m_Player.Run.performed += OnRun;
        m_Player.Run.canceled += OnRun;
        m_Player.Enable();
        SetUpCharacterVariables();
    }
    private void SetUpCharacterVariables()
    {
        jumpHeight = characterData.jumpHeight;
        gravScale = characterData.gravScale;
        WalkSpeed = characterData.walkSpeed;
        RunSpeed =  characterData.runSpeed;
    }
    
    void OnDisable() => m_Player.Disable();
    public void SetAttacking()
    {
       IsAttacking = false;
       animator.SetBool(Attacking, false);
       animator.SetBool(Light , false);
       animator.SetBool(Medium , false);
       animator.SetBool(Left , false);
       animator.SetBool(Right , false);
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

        if (context.canceled || playerMove == Vector2.zero)
        {
            IsWalking = false;
            IsRunning = false;
            return;
        }

        if (!IsRunning)
        {
            IsWalking = true;
        }
     
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        print("Attacking");
        var attackVal = ReturnAttackType(context.ReadValue<float>());
        AttackCheck = StartCoroutine(InputReader.AddAttackInput(attackVal, Time.frameCount));
        IsAttacking = true;
        
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        IsRunning = true;
        IsWalking = false;


    }

    private InputReader.AttackInputResult ReturnAttackType(float attackVal)
    {
        var attackValAsInt = (int) attackVal;
        print(attackValAsInt);
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
