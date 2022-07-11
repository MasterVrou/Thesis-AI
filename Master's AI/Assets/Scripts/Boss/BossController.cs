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
    private bool isCharging;

    private float maxHealth;
    private float chargeStartTime;
    private float chargeDuration;

    public float currentHealth;
    public float chargeSpeed;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        isDying = false;
        isMeleeAttaking = true;
        isFireAttacking = false;
        isCharging = false;
        triggerOnce = false;

        maxHealth = 50;
        currentHealth = maxHealth;
        groundCheckRadius = 0.4f;
        chargeStartTime = 0;
        chargeDuration = 2f;
        chargeSpeed = 2;
    }

    protected override void Update()
    {
        base.Update();

        CheckHealth();
        CheckSurroundings();
        CheckDirection();
        FirstAttack();
        CheckCharge();
    }


    public void FirstAttack()
    {
        if (!triggerOnce)
        {
            triggerOnce = true;
            Charge();
        }
        
    }
    //public void FireAttack()
    //{
    //    anim.SetTrigger("Attack2");
    //}
    //public void Block()
    //{
    //    anim.SetTrigger("Block");
    //}
    //public void Charge()
    //{
    //    anim.SetInteger("AnimState", 1);
    //}

    private void Charge()
    {
        chargeStartTime = Time.time;
        anim.SetInteger("AnimState", 1);
        isCharging = true;

        if (this.transform.position.x < playerPos.position.x)
        {
            rb.velocity = new Vector2(chargeSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-chargeSpeed, rb.velocity.y);
        }
    }

    private void CheckCharge()
    {
        if(Time.time > chargeStartTime + chargeDuration && isCharging)
        {
            anim.SetInteger("AnimState", 0);
            rb.velocity = new Vector2(0, rb.velocity.y);
            isCharging = false;
        }
    }

    public void SetTrigger(string action)
    {
        if (action == "Charge")
        {
            Charge();
        }
        else
        {
            anim.SetTrigger(action);
        }
        
    }

    public void ReSetTrigger(string action)
    {
        if(action == "Charge")
        {
            anim.SetInteger("AnimState", 0);
        }
        else
        {
            anim.ResetTrigger(action);
        }
        
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

    public bool GetIsCharging()
    {
        return isCharging;
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
