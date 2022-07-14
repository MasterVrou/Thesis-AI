using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : ParentController
{
    [SerializeField]
    private Transform playerPos;
    [SerializeField]
    private RuntimeAnimatorController MageAnimController;
    [SerializeField]
    private GameObject fireball;
    [SerializeField]
    private LayerMask whatIsPlayer;

    private GameObject firePillar;

    private Projectile projectileScript;

    private BoxCollider2D hitbox;

    
    
    private bool isDying;
    private bool isMeleeAttaking;
    private bool isFireAttacking;
    private bool triggerOnce;
    private bool isCharging;
    private bool isBlocking;
    private bool damageOnce;
    
    
    private float maxHealth;
    private float chargeStartTime;
    private float chargeDuration;
    private float fireballSpeed = 13f;

    public float currentHealth;
    public float chargeSpeed;
    bool once;
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();

        isDying = false;
        isMeleeAttaking = true;
        isFireAttacking = false;
        isCharging = false;
        triggerOnce = false;

        once = false;
        damageOnce = false;

        maxHealth = 200;
        currentHealth = maxHealth;
        groundCheckRadius = 0.4f;
        chargeStartTime = 0;
        chargeDuration = 2f;
        chargeSpeed = 2;
        fireballSpeed = 13;

        firePillar = transform.Find("FirePillar").gameObject;
        firePillar.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        CheckHealth();
        CheckSurroundings();
        CheckDirection();
        //FirstAttack();
        UpdatePillarPos();
        //UpdateOnceBossSwap();
        //Fireball();
        FirePillar();

    }

    private void FixedUpdate()
    {
        CheckCharge();
    }

    private void UpdatePillarPos()
    {
        firePillar.transform.position = new Vector3(playerPos.position.x, -1, playerPos.position.z);
    }

    private void FirePillar()
    {
        firePillar.SetActive(true);
        if (!damageOnce)
        {
            damageOnce = true;
            Vector2 point = new Vector2(firePillar.transform.position.x, firePillar.transform.position.y);
            Vector2 size = new Vector2(2.3f, 0.7f);

            Collider2D pillarHit = Physics2D.OverlapBox(point, size, 90, whatIsPlayer);

            if(pillarHit)
            {
                AttackDetails a;
                a.damageAmount = 20;
                a.position = firePillar.transform.position;
                pillarHit.transform.SendMessage("Damage", a);
            }
        }
    }

    

    private void Fireball()
    {
        Instantiate(fireball, transform.position, Quaternion.identity);
    }

    private void UpdateOnceBossSwap()
    {
        if (!once)
        {
            once = true;
            anim.runtimeAnimatorController = MageAnimController;
            hitbox.size = new Vector2(0.7f, 1.7f);
            hitbox.offset = new Vector2(0, 0.1f);
        }
    }

    public void FirstAttack()
    {
        if (!triggerOnce)
        {
            triggerOnce = true;
            SetTrigger("Attack2");
        }
        
    }

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
        else if (action == "Parry")
        {
            anim.SetTrigger(action);
            isBlocking = true;
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

    public bool GetIsBlocking()
    {
        return isBlocking;
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

    public void SetIsBlocking(bool b)
    {
        isBlocking = b;
    }
}
