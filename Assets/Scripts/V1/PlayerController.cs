using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    [SerializeField] internal CharacterSO characterData;
    internal Animator animator;
    internal GravityManager gravityManager;
    internal hitDetection playerHitDetection;

    #endregion

    #region Crouching and Dashing variables

    internal bool isCrouching;
    
    
    internal bool Dashing;
    internal bool AtDashHeight;

    #endregion
    
    #region Attack Check Variables

    public bool IsAttacking { get; private set; }
    public bool onAttackCoolDown { get; set; }

    #endregion

    #region Misc variables
  
    internal bool Reversed;
    internal bool hitStun;
    internal int Health;

    #endregion

    #region Move Variables

    public Vector3 playerMove { get; private set; }
    internal float WalkSpeed;
    internal float RunSpeed;
    internal bool IsWalking;
    internal bool IsRunning;

    #endregion

    #region Jump Variables

    [SerializeField] internal Transform raycastPos;
    internal float jumpHeight; //Switch to player character data S.O when created 5 
    internal float raycastDistance; //2.023f
    internal float gravScale; // (Hold for now )  character data affects gravity 5 
    internal float velocity;
    internal Rigidbody rb;
    internal InputReader.MovementInputResult DashDir;
    internal bool isGrounded;
    [SerializeField] internal GameObject Hitbox;

    #endregion


  
    void Awake()
    {
        playerHitDetection = GetComponentInChildren<hitDetection>();
        gravityManager = GetComponent<GravityManager>();
        isGrounded = true;
        InputReader = GetComponent<InputReader>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        raycastDistance = 2.0231f;
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
        m_Player.DashMacro.performed += OnDashMacro;
        m_Player.AirDash.performed += OnAirDash;
        m_Player.Move.performed += OnMove;
        m_Player.Move.canceled += OnMove;
        m_Player.Attack.performed += OnAttack;

        print(gameObject.gameObject.name);

        OnEnablePlayer();
        SetUpCharacterVariables();
    }

    public void OnEnablePlayer()
    {
        m_Player.Enable();
        hitDetection.OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        OnDisablePlayer();
    }


    private void SetUpCharacterVariables()
    {
        //All character data is added here (future ones must be added here as well)
        jumpHeight = characterData.jumpHeight;
        gravScale = characterData.gravScale;
        WalkSpeed = characterData.walkSpeed;
        RunSpeed = characterData.runSpeed;
        Health = characterData.health;
    }


    public void OnDisablePlayer()
    {
        m_Player.Disable();
    }

    void OnDisable() => OnDisablePlayer();
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
        isGrounded = gravityManager.CheckifGrounded(this);
        if (collision.gameObject.CompareTag("Player") && collision.gameObject != gameObject && !isGrounded)
        {
            // print(gameObject.GetComponent<Collider>().bounds.min.y >
            //       collision.gameObject.GetComponent<Collider>().bounds.max.y);
            // print(gameObject.transform.position.x < collision.gameObject.GetComponent<Collider>().bounds.center.x);
            // print(gameObject.GetComponent<Collider>().bounds.min.y);
            // print(collision.gameObject.GetComponent<Collider>().bounds.max.y);
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

    private void OnCollisionExit(Collision other)
    {
       isGrounded = false;
    }

    private void Update()
    {
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

        if (transform.localPosition.y < jumpHeight / 2)
        {
            AtDashHeight = true;
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
//        print("Entered Attack");
//        print("Attacking");
        //    InputReader.AddInput(attackVal,InputReader.attackBuffer);
        InputReader.AddAttackInput(ReturnAttackType(context.ReadValue<float>()));
        if (onAttackCoolDown || IsAttacking) return;
        IsAttacking = true;
        animator.SetBool(Attacking, true);
        //convert and passes input to attack type for the input reader 
//        print(attackVal);
    }

    public void OnDashMacro(InputAction.CallbackContext context)
    {
        if (IsRunning || Dashing || !isGrounded ) return;
        print("entered dash");
        Dashing = true;
        DashDir = InputReader.currentMoveInput;
        IsRunning = false;
        IsWalking = false;
    }
    public void OnAirDash(InputAction.CallbackContext context)
    {
        if (IsRunning || Dashing || isGrounded ) return;
        print("entered dash");
        Dashing = true;
        DashDir = InputReader.LastValidMovementInput;
        IsRunning = false;
        IsWalking = false;
    }
    public void OnRunOrDash(InputAction.CallbackContext context)
    {
        var contextHold = context.interaction as MultiTapOrHold;

        if (IsRunning && context.canceled)
        {
            if (playerMove.x != 0)
            {
                print("canceled run");
                IsRunning = false;
                IsWalking = true;
            }
            else
            {
                IsRunning = false;
            }

            return;
        }


        if (contextHold is { holding: true } && context.performed && isGrounded)
        {
            if (InputReader.LastValidMovementInput == InputReader.MovementInputResult.Backward)
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

        if (contextHold is not { holding: false } || !context.performed) return;
        if (IsRunning || Dashing || !isGrounded || context.canceled) return;
        print("entered dash");
        Dashing = true;
        DashDir = InputReader.LastValidMovementInput;
        IsRunning = false;
        IsWalking = false;
    }

   


   
    private InputReader.AttackInputResult ReturnAttackType(float attackVal)
    {
        //depending on the attacks scale number, (check controls and the attacks scale #) returns the corresponding attack 
        var attackValAsInt = (int)attackVal;
//        print(attackValAsInt);
        var attackResult = attackValAsInt switch
        {
            1 => InputReader.AttackInputResult.Light,
            2 => InputReader.AttackInputResult.Medium,
            3 => InputReader.AttackInputResult.Heavy,
            _ => InputReader.AttackInputResult.None,
        };

        return attackResult;
    }
}