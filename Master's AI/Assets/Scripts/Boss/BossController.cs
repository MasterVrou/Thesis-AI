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

    private GameObject parentFirePillar;
    private GameObject pillarWarning;
    private GameObject pillar;

    private Projectile projectileScript;

    private BoxCollider2D hitbox;

    
    
    private bool isDying;
    private bool isMeleeAttaking;
    private bool isFireAttacking;
    private bool triggerOnce;
    private bool isCharging;
    private bool isBlocking;
    private bool damageOnce;
    private bool isFirePillaring;
    private bool pillarTriggered;
    
    private float maxHealth;
    private float chargeStartTime;
    private float chargeDuration;
    private float fireballSpeed = 13f;
    private float firePillarStartTime;
    private float firePillarHitTime;
    private float pillarTriggeredStartTime;
    private float pillarTriggeredDuration;

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
        isFirePillaring = false;
        pillarTriggered = false;

        once = false;
        damageOnce = false;

        maxHealth = 200;
        currentHealth = maxHealth;
        groundCheckRadius = 0.4f;
        chargeStartTime = 0;
        chargeDuration = 2f;
        chargeSpeed = 2;
        fireballSpeed = 13;
        firePillarHitTime = 1;
        pillarTriggeredDuration = 1;

        parentFirePillar = transform.Find("FirePillar").gameObject;
        pillarWarning = parentFirePillar.transform.FindChild("Warning").gameObject;
        pillar = parentFirePillar.transform.FindChild("Pillar").gameObject;
        pillarWarning.SetActive(false);
        pillar.SetActive(false);

        parentFirePillar.transform.position = new Vector3(playerPos.position.x, -1, playerPos.position.z);
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
        CheckFirePillar();
    }

    private void FixedUpdate()
    {
        CheckCharge();
    }

    private void UpdatePillarPos()
    {
        //parentFirePillar.transform.position = new Vector3(playerPos.position.x, -1, playerPos.position.z);

        //if (pillarTriggered && pillarTriggeredStartTime + pillarTriggeredDuration < Time.time)
        //{
        //    damageOnce = false;
        //    isFirePillaring = false;
        //    pillarTriggered = false;
        //    pillarWarning.SetActive(false);
        //    pillar.SetActive(false);
        //}
        if (!isFirePillaring)
        {
            parentFirePillar.transform.position = new Vector3(playerPos.position.x, -1, playerPos.position.z);
        }
        else
        {
            if (pillarTriggered && pillarTriggeredStartTime + pillarTriggeredDuration < Time.time)
            {
                damageOnce = false;
                isFirePillaring = false;
                pillarTriggered = false;
                pillarWarning.SetActive(false);
                pillar.SetActive(false);
            }
        }

    }

    private void FirePillar()
    {
        if (!isFirePillaring)
        {
            isFirePillaring = true;
            firePillarStartTime = Time.time;
            pillarWarning.SetActive(true);
        }
    }

    private void CheckFirePillar()
    {
        if(isFirePillaring && firePillarStartTime + firePillarHitTime < Time.time)
        {
            pillar.SetActive(true);
            pillarTriggered = true;

            if (!damageOnce)
            {
                pillarTriggeredStartTime = Time.time;

                damageOnce = true;
                Vector2 point = new Vector2(parentFirePillar.transform.position.x, parentFirePillar.transform.position.y);
                Vector2 size = new Vector2(2.3f, 0.7f);

                Collider2D pillarHit = Physics2D.OverlapBox(point, size, 90, whatIsPlayer);

                if (pillarHit)
                {
                    AttackDetails a;
                    a.damageAmount = 20;
                    a.position = parentFirePillar.transform.position;
                    pillarHit.transform.SendMessage("Damage", a);
                }
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
