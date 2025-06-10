using Unity.VisualScripting;
using UnityEngine;
public class PlayerJumpingState : PlayerBaseState
{
    
    #region Standard Jump Variables
    [Header("Jumping")]
    float velocity;
    #endregion
    private float xJumpVal; // check Try jump method for changes 
    private LayerMask groundLayerMask;
    private Collider collider;

    internal override void EnterState(PlayerStateManager playerStateManager,PlayerController player)
    {
       collider = player.GetComponent<Collider>();
    }

    internal override void UpdateState(PlayerStateManager playerStateManager, PlayerController player)
    {
        player.gravityManager.ApplyGravity(player);
        //
        player.isGrounded = player.gravityManager.CheckifGrounded(player);

        // Checking if the player is grounded and resetting position and velocity 
        if ( player.isGrounded)
        {
            player.gravityManager.ResetVelocity();
            Vector3 closestPoint = player.gravityManager.raycastHit.collider.ClosestPoint(player.transform.position);
            Vector3 snappedPosition = new Vector3(player.transform.position.x, closestPoint.y + 1, player.transform.position.z);
            player.transform.position = snappedPosition;
        }
        
        if (player.playerMove.y > 0 && player.isGrounded)
        {
            TryJump(player);
        }
        //This moves the jump (do not touch )
        if (!player.isGrounded)
        {
            velocity = player.gravityManager.GetVelocity();
            player.transform.Translate(new Vector3(xJumpVal, velocity, 0) * Time.deltaTime);
        }

        if (!player.isGrounded && player.IsAttacking)
        {
            playerStateManager.SwitchState(PlayerStateManager.PlayerStateType.Attack);
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
        velocity = player.gravityManager.SetJumpVelocity(player);
        switch (player.InputReader.GetLastInput())
        {
            case InputReader.MovementInputResult.Up:
                xJumpVal = 0;
                break;
            case InputReader.MovementInputResult.UpRight:
                xJumpVal = 5;
                break;
            case InputReader.MovementInputResult.UpLeft:
                xJumpVal = -5;
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
        player.gravityManager.ResetVelocity();
        xJumpVal = 0f;
        Debug.Log("Exiting playerJumpingState");
    }
}
