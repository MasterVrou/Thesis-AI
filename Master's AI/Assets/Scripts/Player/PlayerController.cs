using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ParentController
{
    [SerializeField]
    private Transform bossPos;


    private PlayerAnimationController pAnimController;
    private BoxCollider2D hitbox;

    private bool isWalking;
    private bool isDodging;
    private bool canWalk;
    private bool startingAnim;
    private bool isHooked;

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
        pAnimController = GetComponent<PlayerAnimationController>();
        hitbox = GetComponent<BoxCollider2D>();

        isDodging = false;
        startingAnim = false;
        canWalk = true;
        isHooked = false;

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
        //////////////////////////////////////////////uncomment after training
        CheckInput();
        CheckSurroundings();
    }

    protected override void FixedUpdate()
    {
        //Training Methods
        //AutoWalk();

        //////////////////////////////////////////////uncomment after training
        Walk();
        CheckDodge();
        CheckHook();
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

    public void Jump()
    {
        if (isGrounded && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            //////////////////////////////////////////////////////////inAnim
            pAnimController.SetInAnimation(true);
        }
    }

    public void Dodge()
    {
        if (isGrounded)
        {
            isDodging = true;
            dodgeStartTime = Time.time;
            //////////////////////////////////////////////////////////inAnim
            pAnimController.SetInAnimation(true);
            //hitbox.enabled = false;
            //rb.gravityScale = 0;
        }
    }

    private void CheckDodge()
    {
        if (Time.time >= dodgeStartTime + dodgeDuration && isDodging)
        {
            isDodging = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
            //hitbox.enabled = true;
            //rb.gravityScale = 4;
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
    
    public void Hooked()
    {
        isHooked = true;
    }

    private void CheckHook()
    {
        if (isHooked)
        {
            if(bossPos.position.x < this.transform.position.x)
            {
                rb.velocity = new Vector2(-7, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(7, rb.velocity.y);
            }
        }

        if(Mathf.Abs(bossPos.position.x - this.transform.position.x) < 0.5)
        {
            isHooked = false;
        }
    }

    private void Walk()
    {
        if (canWalk && !isDodging && !isHooked)
        {
            rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
        }
        else if (!isDodging && isGrounded)
        {
            rb.velocity = new Vector2(0, 0);
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f && !isDodging && !isHooked)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    //For training
    public void AutoWalk()
    {
        if (isWalking)
        {
            rb.velocity = new Vector2(movementSpeed * movementDirection, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
      
        
            
    }

    //For training
    public void AutoFlip()
    {
        if (!isDodging)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        
    }

    protected override void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

    }


    //Getters

    public int GetDirection()
    {
        if (isFacingRight)
        {
            return 1;
        }
        else
        {
            return -1;
        }
        
    }

    public bool GetIsDodging()
    {
        return isDodging;
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    //Setters
    public void setCanWalk(bool b)
    {
        canWalk = b;
    }

    public void SetIsWalking(bool b)
    {
        isWalking = b;
    }

    public void SetMovementDirection(int i)
    {
        movementDirection = i;
    }


    protected override void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}
