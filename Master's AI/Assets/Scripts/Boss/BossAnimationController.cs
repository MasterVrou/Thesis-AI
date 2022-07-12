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

    private bool inAction;
    private bool checkChargeOnce;

    private void Start()
    {
        bController = GetComponent<BossController>();

        meleeAttackDamage = 15;

        inAction = false;
        checkChargeOnce = true;
    }

    private void Update()
    {
        CheckCharge();
    }

    private void CheckCharge()
    {
        if (bController.GetIsCharging())
        {
            inAction = true;
            checkChargeOnce = false;
        }
        
        if(!bController.GetIsCharging() && !checkChargeOnce)
        {
            checkChargeOnce = true;
            inAction = false;
        }
    }
    
    private void Damage(AttackDetails ad)
    {
        float hp = bController.GetCurrentHealth();

        if (!bController.GetIsBlocking())
        {
            hp -= ad.damageAmount;
        }

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
        bController.ReSetTrigger("Attack1");
    }

    private void FinishFireAttack()
    {
        bController.ReSetTrigger("Attack2");
    }

    private void FinishBlock()
    {
        bController.ReSetTrigger("Block");
        bController.SetIsBlocking(false);
    }

    private void FinishCharge()
    {
        bController.ReSetTrigger("Charge");
    }

    private void ActionStarting()
    {
        inAction = true;
    }

    private void ActionFinished()
    {
        inAction = false;
    }

    //GETTERS
    public bool GetInAction()
    {
        return inAction;
    }
}
