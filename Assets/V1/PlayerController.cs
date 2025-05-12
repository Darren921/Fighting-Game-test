
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    private Controls.PlayerActions m_Player;
    internal InputReader _inputReader;
    internal PlayerStateManager stateManager;
    private float playerMoveX;
    private float playerMoveY;
    public  Vector2 playerMove {get; private set;}
    internal float verticalVelocity;
    [SerializeField] internal float _moveSpeed;

    public bool isAttacking{get; private set;}
    public bool isPunching {get; private set;}
    public bool isKicking {get; private set;}
    public bool isSlashing {get; private set;}

    

    void Awake()
    {
        _inputReader = GetComponent<InputReader>();
        controls = new Controls();
        m_Player = controls.Player;
        stateManager = GetComponent<PlayerStateManager>();
    }

    void OnDestroy() => controls.Dispose();

    void OnEnable()
    {
        m_Player.Enable();
        m_Player.MoveX.performed += OnMoveX;
        m_Player.MoveX.canceled += OnMoveX;
        m_Player.MoveY.performed += OnMoveY;
        m_Player.MoveY.canceled += OnMoveY;
        // m_Player.Move.performed += OnMove;
        // m_Player.Move.performed += OnMove;

        
        m_Player.Punch.performed += OnPunch;
        m_Player.Punch.canceled += OnPunch;
        m_Player.Kick.performed += OnKick;
        m_Player.Kick.canceled += OnKick;

        
    }


    void OnDisable() => m_Player.Disable();

    private void Update()
    {
        playerMove = new Vector2(playerMoveX, playerMoveY);
        isAttacking = isKicking || isSlashing || isPunching;
    } 
    public void OnMove(InputAction.CallbackContext context)
    {
        // playerMove = context.ReadValue<Vector2>();
        // print(playerMove);    
        //
        // StartCoroutine(playerMove.x > 0 ?
        //     _inputReader.AddInput(InputReader.InputResult.Right, Time.frameCount) :
        //     _inputReader.AddInput(InputReader.InputResult.Left, Time.frameCount)); 
        //
        // StartCoroutine(playerMove.y > 0 ?
        //     _inputReader.AddInput(InputReader.InputResult.Up, Time.frameCount) :
        //     _inputReader.AddInput(InputReader.InputResult.Down, Time.frameCount));
    }

    public void OnMoveX(InputAction.CallbackContext context)
    {
        playerMoveX = context.ReadValue<float>();
        StartCoroutine(playerMoveX > 0 ?
            _inputReader.AddMovementInput(InputReader.MovementInputResult.Right, Time.frameCount) :
            _inputReader.AddMovementInput(InputReader.MovementInputResult.Left, Time.frameCount));
    }

    public void OnMoveY(InputAction.CallbackContext context)
    {
        playerMoveY = context.ReadValue<float>();
        StartCoroutine(playerMoveY > 0 ?
            _inputReader.AddMovementInput(InputReader.MovementInputResult.Up, Time.frameCount) :
            _inputReader.AddMovementInput(InputReader.MovementInputResult.Down, Time.frameCount));
    }

    public void OnPunch(InputAction.CallbackContext context)
    {
         isPunching = context.ReadValueAsButton();
         if (context.performed)
             StartCoroutine(_inputReader.AddAttackInput(InputReader.AttackInputResult.Punch, Time.frameCount));
    }
    
    public void OnKick(InputAction.CallbackContext context)
    {
        isKicking = context.ReadValueAsButton();
        if (context.performed)
            StartCoroutine(_inputReader.AddAttackInput(InputReader.AttackInputResult.Kick, Time.frameCount));
    }

    public void OnSlash(InputAction.CallbackContext context)
    {
        isSlashing = context.ReadValueAsButton();
        if (context.performed)
            StartCoroutine(_inputReader.AddAttackInput(InputReader.AttackInputResult.Slash, Time.frameCount));
    }

  
}
