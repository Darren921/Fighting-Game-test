using System;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public float velocity { get; private set; }
    internal RaycastHit raycastHit;
    private LayerMask _groundLayerMask;
    private LayerMask _playerLayerMask;


    private void Awake()
    {
        _groundLayerMask = LayerMask.GetMask("Ground");
        _playerLayerMask = LayerMask.GetMask("Player");
    }

    public bool CheckifGrounded(PlayerController player)
    {
        var grounded = Physics.Raycast(player.raycastPos.position, -player.transform.up, out raycastHit,
            player.raycastDistance, _groundLayerMask);
//       print(Vector3.Distance(player.raycastPos.position, raycastHit.point)); 
        //   print(raycastHit);
        Debug.DrawRay(player.raycastPos.position, -player.transform.up * player.raycastDistance, Color.red);
        return grounded;
    }

    public bool CheckIfPlayer(PlayerController player)
    {
        var playerBelow = Physics.Raycast(player.raycastPos.position, -player.transform.up, out raycastHit,
            player.raycastDistance, _playerLayerMask);
        if (playerBelow)
        {
            print(player.rb.linearVelocity);
        }

        return playerBelow;
    }

    public void ApplyGravity(PlayerController player)
    {
        velocity += Physics.gravity.y   * player.gravScale * Time.fixedDeltaTime;
//        print("Applying gravity");
    }

    public float GetVelocity()
    {
        return velocity;
    }

    public void ResetVelocity()
    {
        velocity = 0;
    }

    public float SetJumpVelocity(PlayerController player)
    {
        var targetVelocity = Mathf.Sqrt(player.jumpHeight * -2 * (Physics.gravity.y * player.gravScale));
//        print(player.jumpHeight * -2 * (Physics.gravity.y * player.gravScale));
        return velocity = targetVelocity;
    }
}