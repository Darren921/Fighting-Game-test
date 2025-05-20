
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;
    private Controls.PlayerActions m_Player;
    internal InputReader _inputReader;
    internal PlayerStateManager stateManager;
   [SerializeField] private CharacterSO characterData;
    private Coroutine AttackCheck; 
    public Vector2 playerMove {get; private set;}
    internal float verticalVelocity;

    #region Attack Check Variables  
    public bool isAttacking{get; internal set;}
    public bool isPunching {get; private set;}
    public bool isKicking {get; private set;}
    public bool isSlashing {get; private set;}
    
    public bool isHeavySlashing {get; private set;}

 
    #endregion
    
    #region Changeable Move Variables
    internal float _moveSpeed;

    #endregion

    internal bool isGrounded;

    #region Changeable Jump Variables

    internal float jumpHeight; //Switch to player character data S.O when created 5 
    internal float raycastDistance = 1;  // 1
    internal float gravScale; // (Hold for now )  character data affects gravity 5 
    #endregion
    
    void Awake()
    {
        _inputReader = GetComponent<InputReader>();
        controls = new Controls();
        m_Player = controls.Player;
        stateManager = GetComponent<PlayerStateManager>();
        SetUpCharacterVariables();
    }

    private void SetUpCharacterVariables()
    {
        jumpHeight = characterData.jumpHeight;
        gravScale = characterData.gravScale;
        _moveSpeed = characterData.moveSpeed;
    }

    void OnDestroy() => controls.Dispose();

    void OnEnable()
    {
        m_Player.Enable();
        // m_Player.MoveX.performed += OnMoveX;
        // m_Player.MoveX.canceled += OnMoveX;
        // m_Player.MoveY.performed += OnMoveY;
        // m_Player.MoveY.canceled += OnMoveY;
        m_Player.Move.performed += OnMove;
        m_Player.Move.canceled += OnMove;


        m_Player.Attack.performed += OnAttack;


    }


    void OnDisable() => m_Player.Disable();

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
            1 => InputReader.AttackInputResult.Punch,
            2 => InputReader.AttackInputResult.Kick,
            3 => InputReader.AttackInputResult.Slash,
            _ => InputReader.AttackInputResult.None
        };

        return attackResult ;
    }
}
