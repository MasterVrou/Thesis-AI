using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    [SerializeField]
    private Transform meleeAttackHitBoxPos;

    [SerializeField]
    private LayerMask whatIsPlayer;

    

    private BossController bController;

    private AttackDetails attackDetails;

    private float meleeAttackRadius;
    private float meleeAttackDamage;

    private void Start()
    {
        bController = GetComponent<BossController>();

        meleeAttackDamage = 15;
    }

    private void Damage(AttackDetails ad)
    {
        float hp = bController.GetCurrentHealth();

        hp -= ad.damageAmount;

        bController.SetCurrentHealth(hp);
    }

    private void CheckMeleeAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(meleeAttackHitBoxPos.position, meleeAttackRadius, whatIsPlayer);

        attackDetails.damageAmount = meleeAttackDamage;
        attackDetails.position = transform.position;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    private void FinishMeleeAttack()
    {
        bController.ResetTriggerOnce(false);
    }

}
