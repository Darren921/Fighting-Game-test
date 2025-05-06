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
        
        
        m_Player.Punch.performed += OnPunch;
        m_Player.Punch.canceled += OnPunch;
        m_Player.Kick.performed += OnKick;
        m_Player.Kick.canceled += OnKick;

        
    }

    void OnDisable() => m_Player.Disable();

    private void Update() => playerMove = new Vector2(playerMoveX, playerMoveY);

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMove = context.ReadValue<Vector2>();
        StartCoroutine(playerMoveX > 0 ?
            _inputReader.AddInput(InputReader.InputResult.Right, Time.frameCount) :
            _inputReader.AddInput(InputReader.InputResult.Left, Time.frameCount)); 
        
        StartCoroutine(playerMoveY > 0 ?
            _inputReader.AddInput(InputReader.InputResult.Up, Time.frameCount) :
            _inputReader.AddInput(InputReader.InputResult.Down, Time.frameCount));
    }

    public void OnMoveX(InputAction.CallbackContext context)
    {
        playerMoveX = context.ReadValue<float>();
        StartCoroutine(playerMoveX > 0 ?
            _inputReader.AddInput(InputReader.InputResult.Right, Time.frameCount) :
            _inputReader.AddInput(InputReader.InputResult.Left, Time.frameCount));
    }

    public void OnMoveY(InputAction.CallbackContext context)
    {
        playerMoveY = context.ReadValue<float>();
        StartCoroutine(playerMoveY > 0 ?
            _inputReader.AddInput(InputReader.InputResult.Up, Time.frameCount) :
            _inputReader.AddInput(InputReader.InputResult.Down, Time.frameCount));
    }

    public void OnPunch(InputAction.CallbackContext context)
    {
        StartCoroutine(_inputReader.AddInput(InputReader.InputResult.Punch, Time.frameCount));
    }

    public void OnKick(InputAction.CallbackContext context)
    {
        StartCoroutine(_inputReader.AddInput(InputReader.InputResult.Punch, Time.frameCount));
    }

    public void OnSlash(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

  
}
