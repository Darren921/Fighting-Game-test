using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Front and back Movement")]
    internal Vector2 moveDir;
    private Vector2 _smoothedMoveDir;
    [SerializeField]private float _moveSpeed;
    private Vector2 _smoothedMoveVelocity;

    [Header("Jumping")] internal bool isGrounded;
    private bool isDoubleJumping;
  [SerializeField]  private int jumpHeight;
    private Rigidbody rb;
    private RaycastHit raycastHit;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, -transform.up, out raycastHit,1 ))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        smoothMovement();
        movePlayer();
        TryJump();
        rb.AddForce(Physics.gravity * ((5 - 1) * rb.mass));
        
    }

    private void movePlayer()
    {
        rb.linearVelocity = _smoothedMoveDir * _moveSpeed;
    }
    

    private void smoothMovement()
    {
        _smoothedMoveDir = Vector2.SmoothDamp(_smoothedMoveDir, moveDir, ref _smoothedMoveVelocity, 0.1f);
    }

    public  void setMoveDir(Vector2 newDir)
    {
        moveDir = newDir.normalized;
    }

    public bool TryJump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
            return true;
        }
        return false;
    }

    public void TryCrouch()
    {
       
    }
}
