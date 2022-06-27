using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentController : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;

    public Transform groundCheck;

    public LayerMask whatIsGround;

    protected bool isFacingRight;
    protected bool isGrounded;

    protected bool canFlip;

    //1 is right, -1 is left
    protected float movementDirection;

    public float movementSpeed;
    public float groundCheckRadius;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        isFacingRight = true;
        isGrounded = false;

        canFlip = true;
    }

    protected virtual void Update()
    {
        CheckSprites();
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void CheckSprites()
    {
        if (((movementDirection == 1 && !isFacingRight) || (movementDirection == -1 && isFacingRight)) && canFlip)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }

    }

    protected virtual void CheckSurroundings()
    {
    }

    protected virtual void OnDrawGizmos()
    {
    }

    //Getters
    public bool getGrounded()
    {
        return isGrounded;
    }

    public bool GetIsFacingRight()
    {
        return isFacingRight;
    }

    //Setters
    public void setCanFlip(bool b)
    {
        canFlip = b;
    }


}
