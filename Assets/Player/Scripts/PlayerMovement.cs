using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.SocialPlatforms;

public class PlayerMovement : MonoBehaviour

{
    // engine vars
    private Rigidbody2D playerBody;
    public InputMap playerControls;
    private InputAction movement;
    private InputAction jump;

    // constants
    public float speed = 20f;
    public float maxSpeed = 20;
    public float upSpeed = 10;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    private float jumpBufferTime = 0.2f;

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
    private float wallJumpDuration = 0.4f;
    public Vector2 wallJumpPower = new Vector2(8f, 6f);
    public bool climbPower = false;

    // slopes
    public LayerMask slopeLayer;

    // ledge
    [HideInInspector] public bool ledgeDetected;
    [SerializeField] private Vector2 offset1 = new Vector2(-0.131f, -0.256f);  //-0.131, -0.256
    [SerializeField] private Vector2 offset2;  //-0.116, -0.14
    [SerializeField] private Vector2 offset3; // 0, 0.05
    [SerializeField] private Vector2 offset4; // 0.187, 0.286
    private Vector2 climbStartPos;
    private Vector2 climbMidPos1;
    private Vector2 climbMidPos2;
    private Vector2 climbEndPos;

    private bool canGrabLedge = true;
    private bool canClimb;

    // states
    private Vector2 moveDirection = Vector2.zero;
    private bool facingRightState = true;
    public bool groundedState = false;
    public bool walledState = false;
    private bool jumpPressed = false;
    private float jumpBufferCounter = 0f;

    // animator 
    private SpriteRenderer playerSprite;
    private Animator animator;

    void CheckJump()
    {
        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= jumpBufferTime;
        }

        if (jumpBufferCounter > 0 && groundedState)
        {
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, upSpeed);
            jumpBufferCounter = 0;
        }
    }

    private void CheckLedge()
    {
        if (ledgeDetected && canGrabLedge)
        {
            canGrabLedge = false;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetector>().transform.position;
            climbStartPos = ledgePosition + (facingRightState ? offset1 : new Vector2(-offset1.x, offset1.y));
            climbMidPos1 = ledgePosition + offset2;
            climbMidPos2 = ledgePosition + offset3;
            climbEndPos = ledgePosition + (facingRightState ? offset4 : new Vector2(-offset4.x, offset4.y));

            canClimb = true;
        }

        if (canClimb)
        {
            transform.position = climbStartPos;
            movement.Disable();
            playerBody.gravityScale = 0;
        }
    }
    public void LedgeEndState()
    {
        canClimb = false;
        transform.position = climbEndPos;
        playerBody.gravityScale = 1;
        movement.Enable();
        StartCoroutine(EnableLedgeGrab());
    }

    IEnumerator EnableLedgeGrab()
    {
        yield return new WaitForSeconds(0.1f);
        canGrabLedge = true;
    }

    void Awake()
    {
        playerControls = new InputMap();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        movement = playerControls.PlayerInputMap.Movement;
        jump = playerControls.PlayerInputMap.Jump;
        jump.Enable();
        movement.Enable();
    }

    private void OnDisable()
    {
        jump.Disable();
        movement.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = movement.ReadValue<Vector2>();
        jumpPressed = jump.triggered;

        // anims
        if (facingRightState && moveDirection.x < 0 && !(isWallGripping || isWallSliding))
        {
            transform.Rotate(0f, 180f, 0f);
            facingRightState = false;
        }

        if (!facingRightState && moveDirection.x > 0 && !(isWallGripping || isWallSliding))
        {
            transform.Rotate(0f, -180f, 0f);
            facingRightState = true;
        }

        animator.SetFloat("xVelocity", Math.Abs(playerBody.linearVelocityX));
        animator.SetFloat("yVelocity", playerBody.linearVelocityY);

        groundedState = IsGrounded();
        walledState = IsWalled();

        if (climbPower)
        {
            WallSlide();
            WallJump();
        }

        //CheckLedge();
        //animator.SetBool("canClimb", canClimb);
    }

    public void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.DrawWireCube(transform.position - transform.up * groundCastDistance, groundBoxSize);
            Gizmos.DrawWireCube(transform.position - transform.right * wallCastDistance, wallBoxSize);
        }
    }

    public bool IsOnSlope()
    {
        if (Physics2D.BoxCast(transform.position, groundBoxSize, 0, -transform.up, groundCastDistance, slopeLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsGrounded()
    {
        if (Physics2D.BoxCast(transform.position, groundBoxSize, 0, -transform.up, groundCastDistance, groundLayer))
        {
            animator.SetBool("isJumping", false);
            return true;
        }
        else
        {
            animator.SetBool("isJumping", true);
            return false;
        }
    }

    public bool IsWalled()
    {
        if (Physics2D.BoxCast(transform.position, wallBoxSize, 180, -transform.right, wallCastDistance, wallLayer))
        {
            animator.SetBool("isWalled", true);
            return true;
        }
        else
        {
            animator.SetBool("isWalled", false);
            return false;
        }
    }

    void WallJump()
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

        if (jumpPressed && wallJumpCounter > 0f && !isWallGripping)
        {
            isWallJumping = true;
            wallJumpCounter = 0f;

            if (moveDirection.x == 0)
            {
                facingRightState = !facingRightState;
                transform.Rotate(0f, 180f, 0f);
            }

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

        if (IsWalled() & !IsGrounded() && moveDirection.x != 0)
        {
            isWallGripping = true;
            isWallSliding = false; ;
            playerBody.gravityScale = 0;
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, 0);
        }
        else if (IsWalled() & !IsGrounded() && moveDirection.x == 0)
        {
            isWallGripping = false;
            isWallSliding = true;
            playerBody.gravityScale = 1;
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, Mathf.Clamp(playerBody.linearVelocityY, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            if (canGrabLedge)
            {
                playerBody.gravityScale = 1;
            }
            isWallGripping = false;
            isWallSliding = false;
        }
    }

    void FixedUpdate()
    {
        // jump
        if (playerBody.linearVelocityY < 0)
        {
            playerBody.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (playerBody.linearVelocityY > 0 && !jumpPressed)
        {
            playerBody.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (!isWallJumping && !canClimb)
        {
            playerBody.linearVelocity = new Vector2(moveDirection.x * speed, playerBody.linearVelocityY);
        }
        else
        {
            playerBody.linearVelocity = new Vector2(moveDirection.x * wallJumpPower.x, wallJumpPower.y);
        }

        CheckJump();
    }
}

