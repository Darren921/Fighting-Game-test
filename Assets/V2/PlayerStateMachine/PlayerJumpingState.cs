using UnityEngine;
public class PlayerJumpingState : PlayerBaseState
{
    
    #region Standard Jump Variables
    [Header("Jumping")]
    public bool isGrounded { get; private set; }
    private RaycastHit raycastHit;
    float velocity;
    private Collider _collider;
    #endregion
    #region Changeable Jump variables
    [SerializeField] private float jumpHeight = 5f; //Switch to player character data S.O when created  
    [SerializeField] private float raycastDistance = 1f;
    [SerializeField] private float gravScale = 5; // (Hold for now )  character data affects gravity  
    private float xJumpVal; // check Try jump method for changes 
    #endregion
    
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        _collider = playerStateManager.GetComponent<Collider>();
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //
        velocity += Physics.gravity.y * gravScale * Time.deltaTime;
        isGrounded = Physics.Raycast(player.transform.position, -player.transform.up, out raycastHit, raycastDistance) && velocity < 0;

        // Checking if the player is grounded and resetting position and velocity 
        if (isGrounded)
        {
            velocity = 0f;
        }
        
        if (player.playerMove.y > 0 && isGrounded)
        {
            TryJump(player);
        }
        //This moves the jump (do not touch )
        if (!isGrounded)
        {
            player.transform.Translate(new Vector3(xJumpVal, velocity, 0) * Time.deltaTime);
        }
        //Transitioning states 
        switch (isGrounded)
        {
            case true when player.playerMove.y == 0:
            {
                if (player.playerMove == Vector2.zero)
                    playerStateManager.SwitchState(playerStateManager.NeutralState);
                else if (player.playerMove.x != 0 && player.playerMove.y == 0)
                    playerStateManager.SwitchState(playerStateManager.MovingState);
                break;
            }
            case true when player.playerMove.y < 0:
                playerStateManager.SwitchState(playerStateManager.CrouchingState);
                break;
        }
    }

    private void TryJump(PlayerController player)
    {
        velocity = Mathf.Sqrt(jumpHeight * -2 * (Physics.gravity.y * gravScale));
        switch (player._inputReader.GetLastInput())
        {
            case InputReader.MovementInputResult.Up:
                xJumpVal = 0;
                break;
            case InputReader.MovementInputResult.UpRight:
                xJumpVal = 5f;
                break;
            case InputReader.MovementInputResult.UpLeft:
                xJumpVal = -5f;
                break;
            default:
                xJumpVal = 0;
                break;
        }
    }


    internal override void FixedUpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {

    }

    internal override void ExitState(PlayerStateManager playerStateManager,  PlayerController player)
    {
        velocity = 0f;
        xJumpVal = 0f;
        Debug.Log("Exiting playerJumpingState");
    }
}
