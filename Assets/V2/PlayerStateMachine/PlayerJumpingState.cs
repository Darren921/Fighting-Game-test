using UnityEngine;
public class PlayerJumpingState : PlayerBaseState
{
    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float raycastDistance = 1f;

    public bool isGrounded { get; private set; }
    private RaycastHit raycastHit;
    
    
    
    [SerializeField] private float gravScale = 5;
    float velocity;
    private float xJumpVal;
    private Collider _collider;
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        _collider = playerStateManager.GetComponent<Collider>();
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        velocity += Physics.gravity.y * gravScale * Time.deltaTime;
        isGrounded = Physics.Raycast(player.transform.position, -player.transform.up, out raycastHit, raycastDistance) && velocity < 0;

        if (isGrounded)
        {
            velocity = 0f;
            Vector2 surface = Physics.ClosestPoint(player.transform.position, _collider, raycastHit.point, player.transform.rotation);
            player.transform.position = new Vector3(player.transform.position.x, surface.y, player.transform.position.z);
        }
        
        if (player.playerMove.y > 0 && isGrounded)
        {
            TryJump(player);
        }
        
        //this needs to be changed to implement x changes as well, switch case?
        
        player.transform.Translate(new Vector3(xJumpVal,velocity,0) * Time.deltaTime); 

        if (isGrounded && player.playerMove.y == 0)
        {
            if (player.playerMove == Vector2.zero)
                playerStateManager.SwitchState(playerStateManager.NeutralState);
            else if (player.playerMove.x != 0 && player.playerMove.y == 0)
                playerStateManager.SwitchState(playerStateManager.MovingState);
        
        }
        else if (isGrounded && player.playerMove.y < 0)
        {
            playerStateManager.SwitchState(playerStateManager.CrouchingState);
        }
    }

    private void TryJump(PlayerController player)
    {
        velocity = Mathf.Sqrt(jumpHeight * -2 * (Physics.gravity.y * gravScale));
        switch (player._inputReader.GetLastInput())
        {
            case InputReader.InputResult.Up:
                xJumpVal = 0;
                break;
            case InputReader.InputResult.UpRight:
                xJumpVal = 5f;
                break;
            case InputReader.InputResult.UpLeft:
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
        player.transform.position = player.transform.position;
        Debug.Log("Exiting playerJumpingState");
    }
}
