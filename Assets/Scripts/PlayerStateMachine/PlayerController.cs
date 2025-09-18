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
    private readonly int Walking = Animator.StringToHash("Walking");
    private readonly int Running = Animator.StringToHash("Running");
    public readonly int Jump = Animator.StringToHash("Jumping");
    private readonly int Crouch = Animator.StringToHash("Crouching");
    private int Attacking => Animator.StringToHash("Attacking");
    public int Light => Animator.StringToHash("Light");
    public int Medium => Animator.StringToHash("Medium");
    public int left = Animator.StringToHash("Left");
    public int right = Animator.StringToHash("Right");
    public int airborne = Animator.StringToHash("Airborne");

    #endregion

    #region Class references

    private Controls _controls;
    private Controls.PlayerActions _playerActions;
    internal InputReader InputReader;
    internal CharacterSO characterData;
    internal Animator Animator;
    internal GravityManager GravityManager;
    internal HitDetection PlayerHitDetection;
    internal PlayerKnockBack PlayerKnockBack;
    
    #endregion
 
    #region Crouching and Dashing variables

    internal bool IsCrouching;
    internal bool IsDashing;
    internal bool AtDashHeight;

    #endregion

    public Action OnJump;
    public Action<InputReader.AttackInputResult> PlayerAttackAction;
    private PlayerStateManager playerStateManager;
    
    #region Attack Check Variables

    public bool IsAttacking { get; private set; }
    public bool OnAttackCoolDown { get; set; }

    #endregion

    #region Misc variables
  
    internal bool Reversed;
    internal bool HitStun;
    internal int Health;
    internal bool AtBorder;

    #endregion

    #region Move Variables

    public Vector3 PlayerMove { get; private set; }
    internal int jumpCharges { get; set; }

    internal float WalkSpeed;
    internal float RunSpeed;
    internal bool IsWalking;
    internal bool IsRunning;
    [Tooltip("This controls the Decel curve")]
    [SerializeField]internal AnimationCurve decelerationCurve;

    #endregion

    #region Jump Variables
   
    [Tooltip("Origin of the grounded Raycast, DO NOT TOUCH PLEASE")]
    [SerializeField] internal Transform raycastPos;
    
    internal float JumpHeight; 
    internal float RaycastDistance; //2.023f
    internal float GravScale; // (Hold for now )  character data affects gravity 5 
    internal float Velocity;
    internal Rigidbody rb;
    internal InputReader.MovementInputResult DashDir;
    internal bool IsGrounded;
    internal bool SuperJumpActive;
    [SerializeField] internal GameObject hitBox;
    internal bool jumpPressed;

    #endregion
   
    internal float decelerationDuration = 1;
    internal bool decelerating;
    private float elapsedTime;
    internal bool decelActive;

    private void Awake()
    {
        PlayerKnockBack = GetComponent<PlayerKnockBack>();
        PlayerHitDetection = GetComponentInChildren<HitDetection>();
        playerStateManager = GetComponent<PlayerStateManager>();
        GravityManager = GetComponent<GravityManager>();
        IsGrounded = true;
        InputReader = GetComponent<InputReader>();
        Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        RaycastDistance = 2.0231f;
        HitDetection.OnDeath += OnPlayerDeath;
    }

    public void InitializePlayer(InputDevice device)
    {
        //Setup all player controls (note if players > inputs, players aren't set up)
        _controls = new Controls();
        //creates a new set of controls for the chosen device 
        _controls.devices = new[] { device };
        if (device != null)
        {
            _playerActions = _controls.Player;
        }
        else
        {
            _playerActions = new Controls.PlayerActions();
        }
        _playerActions.Run.performed += OnRun;
        _playerActions.Run.canceled += OnRun;
        _playerActions.DashMacro.performed += OnDashMacro;
        _playerActions.Dash.performed += OnDash;
        _playerActions.Move.performed += OnMove;
        _playerActions.Move.canceled += OnMove;
        _playerActions.Light.performed += OnLight;
        _playerActions.Medium.performed += OnMedium;
        _playerActions.Heavy.performed += OnLight; // I don't need to explain this comment

        _playerActions.Jumping.performed += OnJumping;
        _playerActions.SuperJump.performed += OnSuperJump;
       
        OnEnablePlayer();
//        print(gameObject.gameObject.name);
        SetUpCharacterVariables();
    }

    public void OnEnablePlayer()
    {
        _playerActions.Enable();
    }
    public void OnDisablePlayer()
    {
        _playerActions.Disable();
    }

    private void OnPlayerDeath()
    {
        InputReader.enabled = false;
        HitDetection.OnDeath -= OnPlayerDeath;
        OnDisablePlayer();
        playerStateManager.ResetStateMachine();
    }


    private void SetUpCharacterVariables()
    {
        //All character data is added here (future ones must be added here as well)
        
        JumpHeight = characterData.jumpHeight;
        GravScale = characterData.gravScale;
        WalkSpeed = characterData.walkSpeed;
        RunSpeed = characterData.runSpeed;
        Health = characterData.health;
    }


  
    public void SetAttacking()
    {
        //This may need to change to separate ones for each attack
        // This is used at the end of each animation 
        IsAttacking = false;
        Animator.ResetTrigger(Attacking);
        Animator.ResetTrigger(Light);
        Animator.SetBool(Medium, false);
        Animator.SetBool(left, false);
        Animator.SetBool(right, false);
    }

    private void OnCollisionStay(Collision collision)
    {
        IsGrounded = GravityManager.CheckGrounded(this);
        
        //this needs to be rework (current anti head landing )
        PreventHeadLanding(collision);
    }

    private void PreventHeadLanding(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject != gameObject && !IsGrounded)
        {
            if (gameObject.GetComponent<Collider>().bounds.min.y > collision.gameObject.GetComponent<Collider>().bounds.max.y - 0.001)
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
       IsGrounded = false;
    }

    private void Update()
    {
        // sets animator booleans
        Animator.SetBool(airborne, !IsGrounded);
        switch (IsGrounded)
        {
            case true:
                IsCrouching = PlayerMove.y < 0;
                Animator.SetBool(Crouch, IsCrouching);
                Animator.SetBool(Walking, IsWalking);
                Animator.SetBool(Running, IsRunning);
                break;
            case false:
                Animator.SetBool(Crouch, false);
                Animator.SetBool(Walking, false);
                Animator.SetBool(Running, false);
                break;
        }

        if (transform.localPosition.y > JumpHeight / 2)
        {
            AtDashHeight = true;
        }
    }
     

    public void OnMove(InputAction.CallbackContext context)
    {
        //Turns off running and walking when player releases context or player stops 
        //default till running begins
        PlayerMove = context.ReadValue<Vector3>();
        if (!IsRunning && PlayerMove.x != 0)
        {
            IsWalking = true;
        }
        else if (PlayerMove.x == 0)
        {
            IsWalking = false;
        }
    }

    public void OnLight(InputAction.CallbackContext context)
    {
        PlayerAttackAction?.Invoke(InputReader.AttackInputResult.Light);
        Animator.SetTrigger(Attacking);    
        Animator.SetTrigger(Light);
        if (OnAttackCoolDown || IsAttacking) return;
        IsAttacking = true;
    }

    public void OnMedium(InputAction.CallbackContext context)
    {
        PlayerAttackAction?.Invoke(InputReader.AttackInputResult.Medium);
        Animator.SetBool(Medium, true);
        if (OnAttackCoolDown || IsAttacking) return;
        IsAttacking = true;
        Animator.SetTrigger(Attacking);    
    }

    public void OnHeavy(InputAction.CallbackContext context)
    {
        PlayerAttackAction?.Invoke(InputReader.AttackInputResult.Heavy);
        if (OnAttackCoolDown || IsAttacking) return;
        IsAttacking = true;
        Animator.SetTrigger(Attacking);    
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //convert and passes input to attack type for the input reader 
        //PlayerAttackAction?.Invoke(ReturnAttackType(context.ReadValue<float>()));
        if (OnAttackCoolDown || IsAttacking) return;
        IsAttacking = true;
        Animator.SetTrigger(Attacking);    
    }
       
        
    


    public void OnDashMacro(InputAction.CallbackContext context)
    {
        //shortcut for dash 
        if (IsRunning || IsDashing || !IsGrounded ) return;
        print("entered dash");
        IsDashing = true;
        DashDir = InputReader.currentMoveInput;
        IsRunning = false;
        IsWalking = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        switch (IsGrounded)
        {
            case false:
            {
                if (IsDashing || IsGrounded || jumpCharges <= 0) return;
                print("entered dash");
                IsDashing = true;
                DashDir = InputReader.currentMoveInput;
                IsRunning = false;
                IsWalking = false;
                break;
            }
            case true:
            {
                if (IsDashing || InputReader.currentMoveInput == InputReader.MovementInputResult.Forward ) return;
                print("entered dash");
                IsDashing = true;
                DashDir = InputReader.currentMoveInput;
                IsRunning = false;
                IsWalking = false;
                break;
            }
        }
    }
    

    public void OnJumping(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJump?.Invoke();
        }

    }

    public void OnSuperJump(InputAction.CallbackContext context)
    {

        SuperJumpActive = true;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed && InputReader.GetValidMoveInput() != InputReader.MovementInputResult.Backward && InputReader.GetValidMoveInput() != InputReader.MovementInputResult.None && IsGrounded)
        {
//            print(InputReader.currentMoveInput);
            IsRunning = true;
            IsWalking = false;
        }
       
        if (IsRunning && context.canceled)
        {
            if (PlayerMove.x != 0)
            {
                print("canceled run");
                IsRunning = false;
                IsWalking = true;
            }
            
            
        }

    }

   


    internal IEnumerator DecelerationCurve(PlayerController player)
    {
        decelerating = true;

        while (elapsedTime < decelerationDuration && player.decelActive)
        {
        //    var decelerationCurve = player.decelerationCurve.Evaluate(elapsedTime / decelerationDuration);
            player.rb.linearVelocity = Vector3.Lerp(player.rb.linearVelocity, new Vector3(0f,0,0), decelerationDuration) ;
            Debug.Log( player.rb.linearVelocity.magnitude);
            elapsedTime += Time.deltaTime;
            Debug.Log(elapsedTime);
            yield return null;
        }
        decelerating = false;
        elapsedTime = 0f;
    }
    
    /*
    private InputReader.AttackInputResult ReturnAttackType(float attackVal)
    {
        //depending on the attacks scale number, (check controls and the attacks scale #) returns the corresponding attack 
        var attackValAsInt = (int)attackVal;
        var attackResult = attackValAsInt switch
        {
            1 => InputReader.AttackInputResult.Light,
            2 => InputReader.AttackInputResult.Medium,
            3 => InputReader.AttackInputResult.Heavy,
            _ => InputReader.AttackInputResult.None,
        };

        return attackResult;
    }
    */
    
    
    
    /*public void OnRunOrDash(InputAction.CallbackContext context)
    {
        var contextHold = context.interaction as MultiTapOrHold;

        //this method switches between sprint and dashing depending on time held 
        if (IsRunning && context.canceled)
        {
            if (PlayerMove.x != 0)
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

        //This is run section activated when the button is press and held after the second press 
        if (contextHold is { Holding: true } && context.performed && IsGrounded)
        {
            if (InputReader.currentMoveInput == InputReader.MovementInputResult.Backward)
            {
                IsWalking = true;
                return;
            }

            if (!IsDashing)
            {
                print("entered run");
                IsRunning = true;
                IsWalking = false;
            }
        }
        //This is dash section activated when the button is pressed twice quickly 
        if (contextHold is not { Holding: false } || !context.performed) return;
        if (IsRunning || IsDashing || !IsGrounded || context.canceled) return;
        print("entered dash");
        IsDashing = true;
        DashDir = InputReader.currentMoveInput;
        IsRunning = false;
        IsWalking = false;
    }*/





}