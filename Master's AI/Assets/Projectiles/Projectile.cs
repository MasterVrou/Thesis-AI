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

    private AttackDetails attackDetails;

    private float speed;
    private float xStartPos;
    private int direction;

    bool once = false; 
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        xStartPos = transform.position.x;

        attackDetails.position = transform.position;
        attackDetails.damageAmount = 10;
        speed = 7;
        direction = 1;
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
            DamageEnemy(projectileHit);
        }
        else if (this.transform.name == "Hook(Clone)")
        {

        }

        
    }

    private void HookEnemy(Collider2D projectileHit)
    {
        //if()
        //{

        //}
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

        if (Mathf.Abs(xStartPos - transform.position.x) >= 20)
        {
            Destroy(gameObject);
        }
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);
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
