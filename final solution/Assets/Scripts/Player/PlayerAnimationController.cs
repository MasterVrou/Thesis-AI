using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]
    private Transform lightAttackHitBoxPos;
    [SerializeField]
    private Transform heavyAttackHitBoxPos;
    [SerializeField]
    private Text playerHP;

    [SerializeField]
    private LayerMask whatIsDamageable;

    private PlayerController pController;
    private PlayerCombat pCombat;

    private AttackDetails attackDetails;

    private float lightAttackDamage;
    private float heavyAttackDamage;
    private float maxHealth;
    private float sJump;
    private float fJump;

    public float currentHealth;
    public float lightAttackRadius = 0.1f;
    public float heavyAttackRadius = 0.1f;

    private bool inAnimation;
    private bool flLight;
    private bool flHeavy;
    private bool flDodge;
    private bool flParry;
    private bool flJump;

    private string lastMove;

    private void Start()
    {
        pController = GetComponent<PlayerController>();
        pCombat = GetComponent<PlayerCombat>();

        lightAttackDamage = 8;
        heavyAttackDamage = 17;

        maxHealth = 100;
        currentHealth = maxHealth;

        playerHP.text = currentHealth.ToString();

        inAnimation = false;
        
        flLight = false;
        flHeavy = false;
        flDodge = false;
        flParry = false;
        flJump = false;
    }

    private void Update()
    {
        playerHP.text = "playerHP: " + currentHealth.ToString();
    }

    private void WalkingEnabled()
    {
        pController.setCanWalk(true);
    }

    private void WalkingDisabled()
    {
        pController.setCanWalk(false);
    }

    private void FlipEnabled()
    {
        pController.SetCanFlip(true);
    }
    private void FlipDisabled()
    {
        pController.SetCanFlip(false);
    }

    private void CheckLightAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(lightAttackHitBoxPos.position, lightAttackRadius, whatIsDamageable);

        attackDetails.damageAmount = lightAttackDamage;
        attackDetails.position = transform.position;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    private void CheckHeavyAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(heavyAttackHitBoxPos.position, heavyAttackRadius, whatIsDamageable);

        attackDetails.damageAmount = heavyAttackDamage;
        attackDetails.position = transform.position;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    private void FinishLightAttack()
    {
        pCombat.SetIsAttacking(false);
        pCombat.SetIsLightAttacking(false);
    }

    private void FinishRangeAttack()
    {
        pCombat.SetIsAttacking(false);
        pCombat.SetIsRangeAttacking(false);
    }

    private void FinishHeavyAttack()
    {
        pCombat.SetIsAttacking(false);
        pCombat.SetIsHeavyAttacking(false);
    }

    private void Damage(AttackDetails ad)
    {
        //20 damage means that boss used fireattack
        if (pCombat.GetIsShielded() && ad.damageAmount != 20)
        {
            if((ad.position.x > this.transform.position.x && pController.GetIsFacingRight())
                || (ad.position.x < this.transform.position.x && !pController.GetIsFacingRight()))
            {
                return;
            }
        }

        currentHealth -= ad.damageAmount;

        if (ad.damageAmount == 11)
        {
            pController.Hooked();
        }
        

    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        transform.position = new Vector2(-20, -5.84f);
    }

    

    private void UsedLight()
    {
        lastMove = "light";
    }

    private void UsedHeavy()
    {
        lastMove = "heavy";
    }

    private void UsedRanged()
    {
        lastMove = "range";
    }

    private void UsedDodge()
    {
        lastMove = "dodge";
    }

    private void UsedParry()
    {
        lastMove = "parry";
    }

    private void UsedJump()
    {
        sJump = this.transform.position.x;
        lastMove = "jump";
    }

    private void JumpFinish()
    {
        fJump = this.transform.position.x;
    }

    //inAnimation help to register each skill once in DataManagement
    private void AnimationFinished()
    {
        inAnimation = false;
    }

    //Getters
    public float GetCurrentHP()
    {
        return currentHealth;
    }

    public bool GetFlLight()
    {
        return flLight;
    }

    public bool GetFlHeavy()
    {
        return flHeavy;
    }

    public bool GetFlDodge()
    {
        return flDodge;
    }

    public bool GetFlParry()
    {
        return flParry;
    }

    public bool GetFlJump()
    {
        return flJump;
    }

    public bool GetInAnimation()
    {
        return inAnimation;
    }

    public string GetLastMove()
    {
        return lastMove;
    }

    //sJump is the start of the jump
    public float GetSJump()
    {
        return sJump;
    }

    public float GetFJump()
    {
        return fJump;
    }
    //Setters
    public void SetInAnimation(bool b)
    {
        inAnimation = b;
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(lightAttackHitBoxPos.position, lightAttackRadius);
        Gizmos.DrawSphere(heavyAttackHitBoxPos.position, heavyAttackRadius);
    }

}
