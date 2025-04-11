using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 moveDir;
    private Rigidbody rb;
    [Header("StandardMovement")]
    private Vector2 _smoothedMoveDir;
    [SerializeField]private float _moveSpeed;
    private Vector2 _smoothedMoveVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        smoothMovement();
        movePlayer();
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

    public void TryJump()
    {
        
    }
}
