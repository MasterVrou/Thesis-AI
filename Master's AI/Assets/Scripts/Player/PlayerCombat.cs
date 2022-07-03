using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerAnimationController pAnimController;
    private PlayerController PC;
    private Animator anim;

    private AttackDetails attackDetails;

    private float inputTimer;
    private float heavyTimer;
    private float lightAttackDamage;
    private float lightAttackRadius;
    private float heavyAttackDamage;
    private float heavyAttackRadius;

    private bool gotInput;
    private bool gotLightInput;
    private bool gotHeavyInput;
    private bool isAttacking;
    private bool isLightAttacking;
    private bool isHeavyAttacking;
    private bool isShielded;

    private bool startingAnim;

    private void Start()
    {
        PC = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        pAnimController = GetComponent<PlayerAnimationController>();

        inputTimer = 0.2f;

        isAttacking = false;
        isLightAttacking = false;
        isHeavyAttacking = false;
        isShielded = false;

        startingAnim = false;
    }

    private void Update()
    {
        UpdateAnimations();
        CheckCombatInput();
    }

    void NoShield()
    {
        isShielded = false;
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isLightAttacking", isLightAttacking);
        anim.SetBool("isHeavyAttacking", isHeavyAttacking);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("isShielded", isShielded);
    }

    private void CheckCombatInput()
    {
        if (Input.GetMouseButtonDown(0) && PC.GetGrounded())
        {
            LightAttack();
        }

        if (Input.GetMouseButtonDown(1) && PC.GetGrounded())
        {
            HeavyAttack();
        }

        if (Input.GetKeyDown(KeyCode.E) && !isShielded)
        {
            Parry();
        }
    }

    public void Parry()
    {
        if (!isAttacking)
        {
            isShielded = true;
            //call NoShield 3 seconds later
            Invoke("NoShield", 0.3f);
            //////////////////////////////////////////////////////////inAnim
            pAnimController.SetInAnimation(true);
        }
        
    }

    public void LightAttack()
    {
        if (!isLightAttacking && !isAttacking)
        {
            gotInput = false;
            gotLightInput = false;
            isAttacking = true;
            isLightAttacking = true;
            //////////////////////////////////////////////////////////inAnim
            pAnimController.SetInAnimation(true);
        }
    }

    public void HeavyAttack()
    {
        if (!isHeavyAttacking && !isAttacking)
        {
            gotInput = false;
            gotHeavyInput = false;
            isAttacking = true;
            isHeavyAttacking = true;
            //////////////////////////////////////////////////////////inAnim
            pAnimController.SetInAnimation(true);
        }
    }

    //Getters
    public bool GetIsShielded()
    {
        return isShielded;
    }

    //Setters
    public void SetIsAttacking(bool b)
    {
        isAttacking = b;
    }

    public void SetIsLightAttacking(bool b)
    {
        isLightAttacking = b;
    }

    public void SetIsHeavyAttacking(bool b)
    {
        isHeavyAttacking = b;
    }
}
