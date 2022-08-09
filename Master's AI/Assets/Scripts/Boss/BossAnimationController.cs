using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    [SerializeField]
    private Transform meleeAttackHitBoxPos;
    [SerializeField]
    private Transform fireAttackHitBoxPos;
    [SerializeField]
    private Transform AOEHitBoxPos;

    [SerializeField]
    private LayerMask whatIsPlayer;

    

    private BossController bController;

    private AttackDetails attackDetails;

    public float meleeAttackRadius = 0.15f;
    public float fireAttackRadius = 0.15f;
    public float AOERadius = 0.22f;
    private float meleeAttackDamage;
    private float fireAttackDamage;


    private bool inAction;
    private bool checkChargeOnce;
    private bool checkPillarOnce;
    private bool checkFireballOnce;
    private bool damageBlocked;
    private bool checkHookOnce;

    private void Start()
    {
        bController = GetComponent<BossController>();

        meleeAttackDamage = 15;
        fireAttackDamage = 20;

        inAction = false;
        checkChargeOnce = true;
        checkPillarOnce = true;
        checkFireballOnce = true;
        checkHookOnce = true;
    }

    private void Update()
    {
        CheckCharge();
        CheckFirePillar();
        CheckFireball();
        CheckHook();
        CheckFlip();
    }

    private void CheckFirePillar()
    {
        if (bController.GetIsFirepillaring())
        {
            inAction = true;
            checkPillarOnce = false;
        }
        else if(!bController.GetIsFirepillaring() && !checkPillarOnce)
        {
            checkPillarOnce = true;
            inAction = false;
        }
    }
    private void CheckFireball()
    {
        if (bController.GetIsFireballing())
        {
            inAction = true;
            checkFireballOnce = false;
        }
        else if(!bController.GetIsFireballing() && !checkFireballOnce)
        {
            checkFireballOnce = true;
            inAction = false;
        }
    }

    private void CheckHook()
    {
        if (bController.GetIsHooking() || bController.GetIsAOEing())
        {
            inAction = true;
            checkHookOnce = false;
        }
        else if((!bController.GetIsHooking() || !bController.GetIsAOEing())  && !checkHookOnce)
        {
            checkHookOnce = true;
            inAction = false;
        }
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
        else
        {
            damageBlocked = true;
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

    private void CheckAOEHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(AOEHitBoxPos.position, AOERadius, whatIsPlayer);

        attackDetails.damageAmount = 30;
        attackDetails.position = transform.position;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    private void CheckFireAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(fireAttackHitBoxPos.position, fireAttackRadius, whatIsPlayer);

        attackDetails.damageAmount = fireAttackDamage;
        attackDetails.position = transform.position;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    private void CheckFlip()
    {
        if (inAction)
        {
            bController.SetCanFlip(false);
        }
        else
        {
            bController.SetCanFlip(true);
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

    public bool GetDamageBlocked()
    {
        return damageBlocked;
    }

    //SETTERS
    public void SetDamageBlocked(bool b)
    {
        damageBlocked = b;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(meleeAttackHitBoxPos.position, meleeAttackRadius);
        Gizmos.DrawSphere(fireAttackHitBoxPos.position, fireAttackRadius);
        Gizmos.DrawSphere(AOEHitBoxPos.position, AOERadius);
    }
}
