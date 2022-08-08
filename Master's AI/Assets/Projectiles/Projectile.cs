using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsPlayer;
    [SerializeField]
    private Transform damagePosition;
    [SerializeField]
    private float damageRadius;

    private GameObject player;
    private Rigidbody2D rb;
    private SpriteRenderer sb;

    private AttackDetails attackDetails;

    private float speed;
    private float xStartPos;
    private int direction;

    bool once;
    bool damageOnce;
    bool flipOnce;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        sb = GetComponent<SpriteRenderer>();

        xStartPos = transform.position.x;

        attackDetails.position = transform.position;
        
        speed = 7;
        direction = 1;

        once = false;
        damageOnce = false;
        flipOnce = false;
    }

    private void Update()
    {
        CheckPlayerPos();
        attackDetails.position = transform.position;

    }

    private void FixedUpdate()
    {
        Collider2D projectileHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsPlayer);

        if(this.transform.name == "Fireball(Clone)")
        {
            attackDetails.damageAmount = 10;
            DamageEnemy(projectileHit);
        }
        else if (this.transform.name == "Hook(Clone)")
        { 
            attackDetails.damageAmount = 11;
            HookEnemy(projectileHit);
        }

        rb.velocity = new Vector2(direction * speed, rb.velocity.y);
    }

    private void HookEnemy(Collider2D projectileHit)
    {
        if (projectileHit)
        {
            if(!flipOnce && !damageOnce)
            {
                flipOnce = true;
                damageOnce = true;
                projectileHit.transform.SendMessage("Damage", attackDetails);
                sb.flipX = !sb.flipX;
                if (direction == 1)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
            }
        }

        if (Mathf.Abs(xStartPos - transform.position.x) <= 1 && flipOnce)
        {
            Destroy();
        }

        if (Mathf.Abs(xStartPos - transform.position.x) >= 10)
        {
            if (!flipOnce)
            {
                flipOnce = true;
                sb.flipX = !sb.flipX;
                if (direction == 1)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
            }
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void DamageEnemy(Collider2D projectileHit)
    {
        if (projectileHit)
        {
            projectileHit.transform.SendMessage("Damage", attackDetails);
            Destroy(gameObject);
        }

        if (Mathf.Abs(xStartPos - transform.position.x) >= 10)
        {
            Destroy(gameObject);
        }
    }

    private void CheckPlayerPos()
    {
        if (!once)
        {
            once = true;
            player = GameObject.Find("Player");
            if (player.transform.position.x < transform.position.x)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }
        }
    }
        
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(damagePosition.position, damageRadius);
    }
}
