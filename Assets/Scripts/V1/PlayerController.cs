using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour, Controls.IPlayerActions
{
    #region Animator Hashed variables

    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Walking = Animator.StringToHash("Walking");
    public readonly int Running = Animator.StringToHash("Running");
    public readonly int Jump = Animator.StringToHash("Jumping");
    public readonly int Crouch = Animator.StringToHash("Crouching");
    public int Attacking => Animator.StringToHash("Attacking");
    public int Light => Animator.StringToHash("Light");
    public int Medium => Animator.StringToHash("Medium");
    public int Left = Animator.StringToHash("Left");
    public int Right = Animator.StringToHash("Right");
    public int Airborne = Animator.StringToHash("Airborne");

    #endregion

    #region Class references

    private Controls controls;
    private Controls.PlayerActions m_Player;
    internal InputReader InputReader;
    internal PlayerStateManager stateManager;
    [SerializeField] private CharacterSO characterData;
    internal Animator animator;
    internal GravityManager gravityManager;
    internal Collider playerCollider;

    #endregion

    private Coroutine AttackCheck;

    public Vector3 playerMove { get; private set; }
    internal bool isGrounded;
    internal bool isCrouching;
    internal bool Dashing;
    [SerializeField] internal Transform raycastPos;

    #region Attack Check Variables

    public bool IsAttacking { get; private set; }
    public bool IsPunching { get; private set; }
    public bool IsKicking { get; private set; }
    public bool IsSlashing { get; internal set; }

    public bool IsHeavySlashing { get; private set; }
    public bool onAttackCoolDown { get; set; }
    private int dashCooldownAmount { get; set; }

    #endregion

    internal Renderer playerRenderer;
    internal bool playerBelow;
    internal bool Reversed;

    #region Changeable Move Variables

    internal float WalkSpeed;
    internal float RunSpeed;
    internal bool IsWalking;
    internal bool IsRunning;

    #endregion

    #region Changeable Jump Variables

    internal float jumpHeight; //Switch to player character data S.O when created 5 
    internal float raycastDistance = 2.023f; // 1
    internal float gravScale; // (Hold for now )  character data affects gravity 5 
    internal float velocity;
    internal Rigidbody rb;
    internal InputReader.MovementInputResult DashDir;
    private bool onDashCoolDown;
    
    #endregion

    void Awake()
    {
        gravityManager = GetComponent<GravityManager>();
        isGrounded = true;
        playerRenderer = gameObject.GetComponent<Renderer>();
        InputReader = GetComponent<InputReader>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<PlayerStateManager>();
        playerCollider = GetComponent<Collider>();
    }

    public void InitializePlayer(InputDevice device)
    {
        //Setup all player controls (note if players > inputs, players aren't set up)
        controls = new Controls();
        //creates a new set of controls for the chosen device 
        controls.devices = new[] { device };
        m_Player = controls.Player;
        m_Player.RunOrDash.performed += OnRunOrDash;
        m_Player.RunOrDash.canceled += OnRunOrDash;
        m_Player.Move.performed += OnMove;
        m_Player.Move.canceled += OnMove;
        m_Player.Attack.performed += OnAttack;
       

        m_Player.Enable();
        SetUpCharacterVariables();
    }

    private void SetUpCharacterVariables()
    {
        //All character data is added here (future ones must be added here as well)
        jumpHeight = characterData.jumpHeight;
        gravScale = characterData.gravScale;
        WalkSpeed = characterData.walkSpeed;
        RunSpeed = characterData.runSpeed;
    }


//    void OnDisable() => m_Player.Disable();
    public void SetAttacking()
    {
        //This may need to change to separate ones for each attack
        // This is used at the end of each animation 
        IsAttacking = false;
        animator.SetBool(Attacking, false);
        animator.SetBool(Light, false);
        animator.SetBool(Medium, false);
        animator.SetBool(Left, false);
        animator.SetBool(Right, false);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject != gameObject && !isGrounded)
        {
            print(gameObject.GetComponent<Collider>().bounds.min.y >
                  collision.gameObject.GetComponent<Collider>().bounds.max.y);
            print(gameObject.transform.position.x < collision.gameObject.GetComponent<Collider>().bounds.center.x);
            print(gameObject.GetComponent<Collider>().bounds.min.y);
            print(collision.gameObject.GetComponent<Collider>().bounds.max.y);
            if (gameObject.GetComponent<Collider>().bounds.min.y >
                collision.gameObject.GetComponent<Collider>().bounds.max.y - 0.001)
            {
                if (gameObject.transform.position.x < collision.gameObject.GetComponent<Collider>().bounds.center.x)
                {
                    rb.AddForce(-13, 1.5f, 0);
                }
                else
                {
                    rb.AddForce(13, 1.5f, 0);
                }
            }
            else
            {
                if (gameObject.transform.position.x < collision.gameObject.GetComponent<Collider>().bounds.center.x)
                {
                    rb.AddForce(-1, -5f, 0, ForceMode.VelocityChange);
                }
                else
                {
                    rb.AddForce(1, -5f, 0, ForceMode.VelocityChange);
                }
            }
        }
    }

    private void Update()
    {
        isGrounded = gravityManager.CheckifGrounded(this);
        animator.SetBool(Airborne, !isGrounded);
        if (isGrounded)
        {
//            print(IsRunning);
            isCrouching = playerMove.y < 0;
            animator.SetBool(Crouch, isCrouching);
            animator.SetBool(Walking, IsWalking);
            animator.SetBool(Running, IsRunning);
        }

        if (!isGrounded)
        {
            animator.SetBool(Crouch, false);
            animator.SetBool(Walking, false);
            animator.SetBool(Running, false);
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        //     print("Move called");
        playerMove = context.ReadValue<Vector3>();

        //Turns off running and walking when player releases context or player stops 
        //default till running begins
        if (!IsRunning && playerMove.x != 0)
        {
            IsWalking = true;
        }
        else if (playerMove.x == 0)
        {
            IsWalking = false;
        }
        
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        print("Entered Attack");
//        print("Attacking");

        var attackVal = ReturnAttackType(context.ReadValue<float>());
        //    InputReader.AddInput(attackVal,InputReader.attackBuffer);
        InputReader.AddAttackInput(attackVal);
        if (onAttackCoolDown || IsAttacking) return;
        IsAttacking = true;
        animator.SetBool(Attacking, true);
        //convert and passes input to attack type for the input reader 
        //   print(AttackCheck);
        print(attackVal);
    }

    public void OnDashMacro(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnRunOrDash(InputAction.CallbackContext context)
    {
        var contextHold = context.interaction as MultiTapOrHold;
        
        if (IsRunning && context.canceled )
        {
            
            if (playerMove.x != 0)
            {
                print("canceled run");
                IsRunning = false;
                IsWalking = true;
            }
            if ( playerMove.x == 0)
            {
                IsRunning = false;
            }
            return;
        }
        
        
        if (contextHold is { holding: true } && context.performed && isGrounded)
        {
            if(InputReader.LastValidMovementInput == InputReader.MovementInputResult.Backward)
            {
                IsWalking = true;
                return;
            }
          
            if (!Dashing)
            {
                print("entered run");
                IsRunning = true;
                IsWalking = false;
            }
        }
        if (!contextHold.holding && context.performed)
        {
            if(IsRunning || Dashing || !isGrounded || context.canceled) return;
            print("entered dash");
            Dashing = true;
            // StartCoroutine(EnforceDashCoolDown());
            DashDir = InputReader.LastValidMovementInput;
            IsRunning = false;
            IsWalking = false;
        }
        
      
    }
    

    //
    // private IEnumerator EnforceDashCoolDown()
    // {
    //     onDashCoolDown = true;
    //     yield return new WaitForSeconds(1f);
    //     onDashCoolDown = false;
    //
    // }

    private InputReader.AttackInputResult ReturnAttackType(float attackVal)
    {
        //depending on the attacks scale number, (check controls and the attacks scale #) returns the corresponding attack 
        var attackValAsInt = (int)attackVal;
        print(attackValAsInt);
        var attackResult = attackValAsInt switch
        {
            1 => InputReader.AttackInputResult.Light,
            2 => InputReader.AttackInputResult.Medium,
            3 => InputReader.AttackInputResult.Heavy,
        };

        return attackResult;
    }
}