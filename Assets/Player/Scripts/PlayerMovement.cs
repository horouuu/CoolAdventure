using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;
    public bool drawGizmos = false;

    // enemy detection


    // states
    private Vector2 moveDirection = Vector2.zero;
    private bool facingRightState = true;
    private bool groundedState = false;
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
        if (facingRightState && moveDirection.x < 0)
        {
            transform.Rotate(0f, 180f, 0f);
            facingRightState = false;
        }

        if (!facingRightState && moveDirection.x > 0)
        {
            transform.Rotate(0f, -180f, 0f);
            facingRightState = true;
        }
    }

    public void OnDrawGizmos()
    {
        if (drawGizmos) Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }

    public bool IsGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
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

    void FixedUpdate()
    {

        // jump
        groundedState = IsGrounded();
        if (playerBody.linearVelocityY < 0)
        {
            playerBody.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (playerBody.linearVelocityY > 0 && !jumpPressed)
        {
            playerBody.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        playerBody.linearVelocity = new Vector2(moveDirection.x * speed, playerBody.linearVelocityY);
        animator.SetFloat("xVelocity", Math.Abs(playerBody.linearVelocityX));
        animator.SetFloat("yVelocity", playerBody.linearVelocityY);
        CheckJump();
    }
}

