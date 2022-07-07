using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : ParentController
{
    [SerializeField]
    private Transform playerPos;

    private bool isDying;
    private bool isMeleeAttaking;
    private bool isFireAttacking;
    private bool triggerOnce;

    private float maxHealth;
    public float currentHealth;


    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        isDying = false;
        isMeleeAttaking = false;
        isFireAttacking = false;
        triggerOnce = false;

        maxHealth = 50;
        currentHealth = maxHealth;
        groundCheckRadius = 0.4f;
    }

    protected override void Update()
    {
        base.Update();

        CheckHealth();
        CheckSurroundings();
        CheckDirection();
        //UpdateAnimations();
    }

    //private void UpdateAnimations()
    //{
    //    if (!triggerOnce)
    //    {
    //        triggerOnce = true;

    //        if (isMeleeAttaking)
    //        {
    //            anim.SetTrigger("Attack1");
    //        }
    //        if (isFireAttacking)
    //        {
    //            anim.SetTrigger("Attack2");
    //        }

    //    }

    //}
    public void MeleeAttack()
    {
        anim.SetTrigger("Attack1");
    }
    public void FireAttack()
    {
        anim.SetTrigger("Attack2");
    }
    public void Block()
    {
        anim.SetTrigger("Block");
    }
    public void Charge()
    {

    }

    public void SetTrigger(string action)
    {
        anim.SetTrigger(action);
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

    protected override void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    private void CheckDirection()
    {
        if (playerPos.position.x < this.transform.position.x)
        {
            movementDirection = -1;
        }
        else if (playerPos.position.x > this.transform.position.x)
        {
            movementDirection = 1;
        }
    }

    //Getters
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetPlayerPos()
    {
        return playerPos.position.x;
    }

    //Setters
    public void SetCurrentHealth(float hp)
    {
        currentHealth = hp;
        
    }

    public void ResetTriggerOnce(bool b)
    {
        triggerOnce = b;
    }
}
