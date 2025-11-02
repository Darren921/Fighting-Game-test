using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Controls.IPlayerActions
{


    #region Animator Hashed variables

    internal readonly int Idle = Animator.StringToHash("Idle");
    private readonly int Walking = Animator.StringToHash("Walking");
    private readonly int Running = Animator.StringToHash("Running");
    internal readonly int Jump = Animator.StringToHash("Jumping");
    private readonly int Crouch = Animator.StringToHash("Crouching");
    internal int Attacking => Animator.StringToHash("Attacking");
    private int Light => Animator.StringToHash("Light");
    private int Medium => Animator.StringToHash("Medium");
    internal int left = Animator.StringToHash("Left");
    internal int right = Animator.StringToHash("Right");
    internal int airborne = Animator.StringToHash("Airborne");
    internal int blocking = Animator.StringToHash("Blocking");//NEW, FOR BLOCKING
    private int StartUp = Animator.StringToHash("StartUp");
    private int Active = Animator.StringToHash("Active");
    private int Recovery = Animator.StringToHash("Recovery");
    #endregion

    #region Class references

    private Controls _controls;
    private Controls.PlayerActions _playerActions;
    internal InputReader InputReader;
   [SerializeField] internal CharacterSO CharacterData;
    internal Animator Animator;
    internal GravityManager GravityManager;
    internal HitDetection PlayerHitDetection;
    internal PlayerKnockBack PlayerKnockBack;
    public PlayerStateManager _playerStateManager;

    #endregion
    

    #region Crouching and Dashing variables

    [SerializeReference]  internal bool IsCrouching;
    [SerializeField]  internal bool IsDashing;
    [SerializeField]  internal bool AtDashHeight;

    #endregion

    #region PlayerActions
    public Action OnJump;
    public  Action<InputReader.AttackType> PlayerAttackAction;
    #endregion
    
    #region Attack Check Variables

    [field: SerializeField] public bool IsAttacking { get; private set; }
    [field: SerializeField]  public bool OnAttackCoolDown { get; set; }
    [field: SerializeField]  public bool IsActiveFrame{get; private set;}
    
    public bool IsBeingAttacked;//NEW, FOR BLOCKING, idk where else to put this

    #endregion
    
    #region Move Variables
    [field: SerializeField]public Vector3 PlayerMove { get; private set; }
    [SerializeField] internal float WalkSpeed;
    [SerializeField] internal float RunSpeed;
    [SerializeField] internal bool IsWalking;
    [SerializeField] internal bool IsRunning;
    #endregion

    #region Jump Variables

    [Tooltip("Origin of the grounded Raycast, DO NOT TOUCH PLEASE")] 
    [SerializeField] internal Transform raycastPos;
    [SerializeField] internal int JumpCharges;
    [SerializeField] internal float JumpHeight;
    internal float RaycastDistance; //2.023f
    internal float GravScale; // (Hold for now )  character data affects gravity 5 
    [SerializeField] internal float Velocity;
    internal Rigidbody rb;
     [SerializeField] internal InputReader.MovementInputResult DashDir;
     [SerializeField] internal bool IsGrounded;
     [SerializeField] internal bool SuperJumpActive;
     [SerializeField] internal GameObject hitBox;
     [SerializeField] internal bool JumpPressed;

    #endregion

    #region Decelerating Variables
    [SerializeField]  private float DecelerationDuration = 0.7f;
    [SerializeField] internal bool Decelerating;
    [SerializeField]   private float _elapsedTime;
    [SerializeField]  internal bool DecelActive;
    #endregion
  
    #region Misc variables

    [SerializeField] internal bool Reversed;
    [SerializeField]  internal bool HitStun;
    [SerializeField]  internal float Health;
    [SerializeField]  internal bool AtBorder;
    [SerializeField]  internal bool DashMarcoActive;
    [SerializeField] private float MinDashHeight;

    #endregion

    private void Awake()
    {
        MinDashHeight = 1.487012f;
        PlayerKnockBack = GetComponent<PlayerKnockBack>();
        PlayerHitDetection = GetComponentInChildren<HitDetection>();
        _playerStateManager = GetComponent<PlayerStateManager>();
        GravityManager = GetComponent<GravityManager>();
        IsGrounded = true;
        InputReader = GetComponent<InputReader>();
        Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        RaycastDistance = 2.006f;
        HitDetection.OnDeath += OnPlayerDeath;
    }

    public void InitializePlayer(InputDevice device)
    {
        //Setup all player controls (note if players > inputs, players aren't set up)
        _controls = new Controls();
        //creates a new set of controls for the chosen device 
        _controls.devices = new[] { device };
        _playerActions = device != null ? _controls.Player : new Controls.PlayerActions();
        _playerActions.Run.performed += OnRun;
        _playerActions.Run.canceled += OnRun;
        _playerActions.DashMacro.performed += OnDashMacro;
        _playerActions.DashMacro.canceled += OnDashMacro;
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
        PauseManager.Instance?.RegisterPlayer(this);
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
        StopAllCoroutines();
        _playerStateManager.ResetStateMachine();
        _playerActions.RemoveCallbacks(this);
        HitDetection.OnDeath -= OnPlayerDeath;
        _playerActions.Disable();
        PauseManager.Instance?.UnregisterPlayer(this);
    }

    private void OnDestroy()
    {
        _playerActions.RemoveCallbacks(this);
        HitDetection.OnDeath -= OnPlayerDeath;
        _playerActions.Disable();
        PauseManager.Instance?.UnregisterPlayer(this);

    }


    private void SetUpCharacterVariables()
    {
        //All character data is added here (future ones must be added here as well)

        JumpHeight = CharacterData.jumpHeight;
        GravScale = CharacterData.gravScale;
        WalkSpeed = CharacterData.walkSpeed;
        RunSpeed = CharacterData.runSpeed;
        Health = CharacterData.health;
    }


    public void ResetAttackingTrigger()
    {
        //This may need to change to separate ones for each attack
        // This is used at the end of each animation 
        IsAttacking = false;
        Animator?.ResetTrigger(StartUp);
        Animator?.ResetTrigger(Attacking);
        Animator?.ResetTrigger(Light);
        Animator?.ResetTrigger(Medium);
        Animator?.SetBool(left, false);
        Animator?.SetBool(right, false);
        Animator?.SetBool(Active,false);
    }

    public void SetUpStartupFrame()
    {
        Animator?.SetBool(StartUp,true);
    }

    public void SetUpActiveFrame()
    {
        Animator?.SetBool(Active,true);
        Animator?.SetBool(StartUp,false);

    }

    public void SetUpRecoveryFrame()
    {
        Animator?.SetBool(Recovery,true);
        Animator?.SetBool(Active,false);

    }

    public void ResetRecoveryFrame()
    {
        Animator?.SetBool(Recovery,false);
    }
    private void OnCollisionStay(Collision collision)
    {
        IsGrounded = GravityManager.CheckGrounded(this);

        //this needs to be rework (current anti head landing )
      //  PreventHeadLanding(collision);
    }

    /*private void PreventHeadLanding(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject != gameObject && !IsGrounded)
        {
            if (gameObject.GetComponent<Collider>().bounds.min.y >
                collision.gameObject.GetComponent<Collider>().bounds.max.y - 0.001)
            {
                if (gameObject.transform.position.x < collision.gameObject.GetComponent<Collider>().bounds.center.x)
                {
                    rb.AddForce(-5, 1.5f, 0);
                }
                else
                {
                    rb.AddForce(5, 1.5f, 0);
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
    }*/

    private void OnCollisionExit(Collision other)
    {
        IsGrounded = false;
    }

    private void Update()
    {
        // sets animator booleans
        Animator?.SetBool(airborne, !IsGrounded);
        switch (IsGrounded)
        {
            case true:
                IsCrouching = PlayerMove.y < 0;
               Animator?.SetBool(Crouch, IsCrouching);
               Animator?.SetBool(Walking, IsWalking);
               Animator?.SetBool(Running, IsRunning);
                break;
            case false:
               Animator?.SetBool(Crouch, false);
               Animator?.SetBool(Walking, false);
               Animator?.SetBool(Running, false);
                break;
        }
        if (Animator is not null) IsActiveFrame = Animator.GetBool(Active);
//        print(GravityManager.RaycastHit.distance);
//        print(JumpHeight / 2);
        if (!IsGrounded && transform.localPosition.y > MinDashHeight)
        {
//            Debug.Log(transform.transform.localPosition.y  );
//            Debug.Log(JumpHeight / 2);
            AtDashHeight = true;
        }
        else
        {
            AtDashHeight = false;
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        //Turns off running and walking when player releases context or player stops F
        //default till running begins
        PlayerMove = context.ReadValue<Vector3>();
        if (!IsRunning &&
            _playerStateManager.currentState !=
            _playerStateManager.States[PlayerStateManager.PlayerStateTypes.Running] && PlayerMove.x != 0)
        {
//            print("walking");
            IsWalking = true;
        }
        else if (PlayerMove.x == 0)
        {
            //           print("stop walking");
            IsWalking = false;
        }
    }

    public void OnLight(InputAction.CallbackContext context)
    {
        PlayerAttackAction?.Invoke(InputReader.AttackType.Light);
     
        if (OnAttackCoolDown || IsAttacking || !context.performed) return;
        IsAttacking = true;
       Animator?.SetTrigger(Attacking);
       Animator?.SetTrigger(Light);
    }

    public void OnMedium(InputAction.CallbackContext context)
    {
        PlayerAttackAction?.Invoke(InputReader.AttackType.Medium);
        if (OnAttackCoolDown || IsAttacking || !context.performed) return;
        IsAttacking = true;
        Animator?.SetTrigger(Attacking);
        Animator?.SetTrigger(Medium);
    }

    public void OnHeavy(InputAction.CallbackContext context)
    {
        PlayerAttackAction?.Invoke(InputReader.AttackType.Heavy);
        if (OnAttackCoolDown || IsAttacking || !context.performed) return;
        IsAttacking = true;
        Animator?.SetTrigger(Attacking);
    }
    

    public void OnDashMacro(InputAction.CallbackContext context)
    {
        //shortcut for dash 
        print("entered dash Marco");
        DashMarcoActive = true;
        switch (context.performed)
        {
            case true when InputReader.CurrentMoveInput is not (InputReader.MovementInputResult.Forward
                or InputReader.MovementInputResult.None) && IsGrounded:
                print("dash back");
                PerformDash();
                break;
            case true when !IsGrounded:
                print("air dash");
                PerformDash();
                break;
            case true:
            {
                print("sprinting");
                IsRunning = true;
                IsWalking = false;
                if (!context.canceled) return;
                print("sprint cancel ");
                IsRunning = false;
                break;
            }
        }

        if (!context.canceled) return;
        DashMarcoActive = false;
    }

    private void PerformDash()
    {
        if(!IsDashing)
        IsDashing = true;
        DashDir = InputReader.CurrentMoveInput;
        IsRunning = false;
        IsWalking = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        switch (IsGrounded)
        {
            case false:
            {
                if (IsDashing || IsGrounded || JumpCharges <= 0 || !AtDashHeight) return;
                print("entered dash");
                PerformDash();
                break;
            }
            case true:
            {
                if (IsDashing || InputReader.CurrentMoveInput == InputReader.MovementInputResult.Forward) return;
                print("entered dash");
                PerformDash();
                break;
            }
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed && InputReader.GetValidMoveInput() is not (InputReader.MovementInputResult.Backward or InputReader.MovementInputResult.None or InputReader.MovementInputResult.Down) && IsGrounded && !IsRunning)
        {
//            print(InputReader.CurrentMoveInput);
            IsRunning = true;
            IsWalking = false;
        }
        if (!IsRunning || !context.canceled || PlayerMove.x == 0 || _playerStateManager.currentState == _playerStateManager.States[PlayerStateManager.PlayerStateTypes.Running]) return;
        print("canceled run");
        IsRunning = false;
        IsWalking = true;
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
    
    

  

    internal IEnumerator DecelerationCurve(PlayerController player)
    {
        Decelerating = true;

        while (_elapsedTime < DecelerationDuration && player.DecelActive)
        {
            //    var decelerationCurve = player.decelerationCurve.Evaluate(elapsedTime / decelerationDuration);
            player.rb.linearVelocity =
                Vector3.Lerp(player.rb.linearVelocity, new Vector3(0f, 0, 0), DecelerationDuration);
//          Debug.Log( player.rb.linearVelocity.magnitude);
            _elapsedTime += Time.deltaTime;
//            Debug.Log(elapsedTime);
            yield return null;
        }

        Decelerating = false;
        _elapsedTime = 0f;
    }
}