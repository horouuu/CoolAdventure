using UnityEngine;

public class MushroomMovement : MonoBehaviour
{
    // movement
    private float anchorX;
    private float maxTravelX = 5.0f;
    private float patrolTime = 1.5f;
    private int moveRight = -1;
    private Vector2 velocity;
    private float changeDirDelay = 0.5f;
    private float changeDirNext = 0f;
    public bool stun = false;

    // states
    private bool facingRightState = false;

    // rigidbody
    private Rigidbody2D mushroomBody;

    // animator
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        mushroomBody = GetComponent<Rigidbody2D>();
        anchorX = transform.position.x;
        ComputeVelocity();
    }

    public void Stun()
    {
        stun = !stun;
        animator.SetBool("isStunned", stun);
    }

    void ComputeVelocity()
    {
        velocity = new Vector2(moveRight * maxTravelX / patrolTime, 0);
    }

    void MoveMushroom()
    {
        animator.SetFloat("xVelocity", Mathf.Abs(velocity.x));
        mushroomBody.MovePosition(mushroomBody.position + velocity * Time.fixedDeltaTime);

        // anims
        if (facingRightState && velocity.x < 0)
        {
            transform.Rotate(0f, 180f, 0f);
            facingRightState = false;
        }

        if (!facingRightState && velocity.x > 0)
        {
            transform.Rotate(0f, -180f, 0f);
            facingRightState = true;
        }
    }

    void Update()
    {
        if (Mathf.Abs(mushroomBody.position.x - anchorX) > maxTravelX && Time.time > changeDirNext && !stun)
        {
            changeDirNext = Time.time + changeDirDelay;
            moveRight *= -1;
            ComputeVelocity();
            MoveMushroom();
        }
        else if (!stun)
        {
            MoveMushroom();
        }
    }
}
