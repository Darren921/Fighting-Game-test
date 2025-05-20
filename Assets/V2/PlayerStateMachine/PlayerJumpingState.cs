using UnityEngine;
public class PlayerJumpingState : PlayerBaseState
{
    
    #region Standard Jump Variables
    [Header("Jumping")]
    private RaycastHit raycastHit;
    float velocity;
    private Collider _collider;
    #endregion
    private float xJumpVal; // check Try jump method for changes 

    
    internal override void EnterState(PlayerStateManager playerStateManager)
    {
        _collider = playerStateManager.GetComponent<Collider>();
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        //
        velocity += Physics.gravity.y * player.gravScale * Time.deltaTime;
        player.isGrounded = Physics.Raycast(player.transform.position, -player.transform.up, out raycastHit, player.raycastDistance) && velocity < 0;

        // Checking if the player is grounded and resetting position and velocity 
        if (  player.isGrounded)
        {
            velocity = 0f;
        }
        
        if (player.playerMove.y > 0 && player.isGrounded)
        {
            TryJump(player);
        }
        //This moves the jump (do not touch )
        if (!player.isGrounded)
        {
            player.transform.Translate(new Vector3(xJumpVal, velocity, 0) * Time.deltaTime);
        }
        //Transitioning states 
        switch (player.isGrounded)
        {
            case true when player.playerMove.y == 0:
            {
                if (player.playerMove == Vector2.zero)
                    playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Neutral);
                else if (player.playerMove.x != 0 && player.playerMove.y == 0)
                    playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Moving);
                break;
            }
            case true when player.playerMove.y < 0:
                playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Crouching);
                break;
            
            
        }
    }

    private void TryJump(PlayerController player)
    {
        velocity = Mathf.Sqrt(player.jumpHeight * -2 * (Physics.gravity.y * player.gravScale));
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
