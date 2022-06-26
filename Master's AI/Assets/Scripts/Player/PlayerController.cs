using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    public Transform groundCheck;

    public LayerMask whatIsGround;

    private bool isFacingRight;
    private bool isWalking;
    private bool isDodging;
    private bool isGrounded;

    private bool canMove;
    private bool canFlip;

    //1 is right, -1 is left
    private float movementDirection;

    public float movementSpeed;
    public float jumpSpeed;
    public float groundCheckRadius;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        isFacingRight = true;
        isWalking = false;
        isGrounded = false;
        isDodging = false;

        canMove = true;
        canFlip = true;

        movementSpeed = 7;
        jumpSpeed = 12;
        groundCheckRadius = 0.4f;
        movementDirection = 0;
    }

    private void Update()
    {
        UpdateAnimations();
        CheckInput();
        CheckSurroundings();
        CheckSprites();
    }

    private void FixedUpdate()
    {
        Walk();
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void CheckInput()
    {
        movementDirection = Input.GetAxisRaw("Horizontal");
    }

    private void Walk()
    {
        if (canMove && !isDodging)
        {
            rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
        }
        else if (!isDodging)
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

    
    private void CheckSprites()
    {
        if((movementDirection == 1 && !isFacingRight) || (movementDirection == -1 && isFacingRight) && canFlip)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

    }

    public void setCanFlip(bool b)
    {
        canFlip = b;
    }

    public void setCanMove(bool b)
    {
        canMove = b;
    }

}
