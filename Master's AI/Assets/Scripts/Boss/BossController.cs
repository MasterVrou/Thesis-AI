using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    public Transform groundCheck;

    public LayerMask whatIsGround;

    private bool isDying;
    private bool isGrounded;

    private float maxHealth;
    public float currentHealth;
    public float groundCheckRadius;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        isDying = false;

        maxHealth = 50;
        currentHealth = maxHealth;
        groundCheckRadius = 0.4f;
    }

    private void Update()
    {
        CheckHealth();
        CheckSurroundings();
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        anim.SetTrigger("Death");
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    //Getters
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    //Setters
    public void SetCurrentHealth(float hp)
    {
        currentHealth = hp;
        
    }

    
}
