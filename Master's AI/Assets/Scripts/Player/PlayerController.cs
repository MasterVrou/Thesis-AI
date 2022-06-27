using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ParentController
{
    private bool isWalking;
    private bool isDodging;

    private bool canWalk;

    //1 is right, -1 is left
    private float dodgeStartTime;
    private float dodgeDuration;
    
    public float jumpSpeed;

    public float dodgeSpeed;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        isDodging = false;

        movementSpeed = 7;
        jumpSpeed = 12;
        groundCheckRadius = 0.4f;
        movementDirection = 0;
        dodgeDuration = 0.3f;
        dodgeSpeed = 15;
    }

    protected override void Update()
    {
        base.Update();

        UpdateAnimations();
        CheckInput();
        CheckSurroundings();
    }

    protected override void FixedUpdate()
    {
        Walk();
        CheckDodge();
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isDodging", isDodging);
    }

    private void CheckInput()
    {
        movementDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Dodge();
        }
    }

    private void Jump()
    {
        if (isGrounded && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    private void Dodge()
    {
        if (isGrounded)
        {
            isDodging = true;
            dodgeStartTime = Time.time;
        }
    }

    private void CheckDodge()
    {
        if (Time.time >= dodgeStartTime + dodgeDuration && isDodging)
        {
            isDodging = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
        else if (isDodging)
        {
            if (isFacingRight)
            {
                rb.velocity = new Vector2(dodgeSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-dodgeSpeed, rb.velocity.y);
            }
            
        }
    }

    private void Walk()
    {
        if (canWalk && !isDodging)
        {
            rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
        }
        else if (!isDodging && isGrounded)
        {
            rb.velocity = new Vector2(0, 0);
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f && !isDodging)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    protected override void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    //Getters

    //Setters
    public void setCanWalk(bool b)
    {
        canWalk = b;
    }
}
