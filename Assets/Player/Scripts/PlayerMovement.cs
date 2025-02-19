using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour

{
    // engine vars
    private Rigidbody2D playerBody;
    // constants
    public float speed = 20f;
    public float maxSpeed = 20;
    public float upSpeed = 6f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    // ground detection
    public Vector2 groundBoxSize;
    public float groundCastDistance;
    public LayerMask groundLayer;
    public bool drawGizmos = false;

    // wall detection
    public Vector2 wallBoxSize;
    public float wallCastDistance;
    public LayerMask wallLayer;

    // wall slide and jump
    public bool isWallGripping = false;
    public bool isWallSliding = false;
    private float wallSlidingSpeed = 0.4f;
    private bool isWallJumping = false;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    public float wallJumpDuration = 2f;
    public Vector2 wallJumpPower = new Vector2(8f, 6f);
    public bool climbPower = false;

    // states
    [Serialize] private bool facingRightState = true;
    [Serialize] private bool groundedState = false;
    [Serialize] private bool walledState = false;
    [Serialize] private bool movingState = false;
    private int moveDirection = 1;

    private Animator animator;
    public void Jump()
    {
        if (IsGrounded())
        {
            playerBody.AddForce(new Vector2(0, upSpeed), ForceMode2D.Impulse);
        }

        if (climbPower)
        {
            WallJump();
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        playerBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("xVelocity", Math.Abs(playerBody.linearVelocityX));
        animator.SetFloat("yVelocity", playerBody.linearVelocityY);

        groundedState = IsGrounded();
        animator.SetBool("isJumping", !groundedState);

        walledState = IsWalled();
        animator.SetBool("isWalled", walledState);

        if (climbPower)
        {
            WallSlide();
            CheckWallJump();
        }
    }

    public void MoveCheck(int dir)
    {
        movingState = dir != 0;
        moveDirection = dir;
        Move(dir);
        FlipPlayer(dir);
    }

    public void Move(int dir)
    {
        playerBody.linearVelocity = new Vector2(dir * speed, playerBody.linearVelocityY);
    }

    public void FlipPlayer(int dir)
    {
        if (facingRightState && dir < 0 && !(isWallGripping || isWallSliding))
        {
            transform.Rotate(0f, 180f, 0f);
            facingRightState = false;
        }

        if (!facingRightState && dir > 0 && !(isWallGripping || isWallSliding))
        {
            transform.Rotate(0f, -180f, 0f);
            facingRightState = true;
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.DrawWireCube(transform.position - transform.up * groundCastDistance, groundBoxSize);
            Gizmos.DrawWireCube(transform.position - transform.right * wallCastDistance, wallBoxSize);
        }
    }
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position, groundBoxSize, 0, -transform.up, groundCastDistance, groundLayer);
    }

    public bool IsWalled()
    {
        return Physics2D.BoxCast(transform.position, wallBoxSize, 180, -transform.right, wallCastDistance, wallLayer);
    }

    void CheckWallJump()
    {
        if (isWallSliding || isWallGripping)
        {
            isWallJumping = false;
            wallJumpCounter = wallJumpTime;

            CancelInvoke(nameof(StopWallJump));
        }
        else if (wallJumpCounter > 0)
        {
            wallJumpCounter -= Time.deltaTime;
        }
    }
    void WallJump()
    {
        if (wallJumpCounter > 0f && !isWallGripping && IsWalled())
        {
            isWallJumping = true;
            wallJumpCounter = 0f;

            if (!movingState)
            {
                facingRightState = !facingRightState;
                transform.Rotate(0f, 180f, 0f);
            }

            playerBody.AddForce(wallJumpPower, ForceMode2D.Impulse);

            Invoke(nameof(StopWallJump), wallJumpDuration);
        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    void WallSlide()
    {
        animator.SetBool("isWallGripping", isWallGripping);
        animator.SetBool("isWallSliding", isWallSliding);

        if (IsWalled() && !IsGrounded() && movingState)
        {
            isWallGripping = true;
            isWallSliding = false;
            playerBody.gravityScale = 0;
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, 0);
        }
        else if (IsWalled() && !IsGrounded() && !movingState)
        {
            isWallGripping = false;
            isWallSliding = true;
            playerBody.gravityScale = 1;
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, Mathf.Clamp(playerBody.linearVelocityY, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            playerBody.gravityScale = 1;
            isWallGripping = false;
            isWallSliding = false;
            FlipPlayer(moveDirection);
        }
    }

    void FixedUpdate()
    {
        if (!movingState && !isWallJumping)
        {
            playerBody.linearVelocityX = 0;
        }

        if ((playerBody.linearVelocityX < 0 && moveDirection > 0) || (playerBody.linearVelocityX > 0 && moveDirection < 1))
        {
            playerBody.linearVelocityX = 0;
        }
    }
}

