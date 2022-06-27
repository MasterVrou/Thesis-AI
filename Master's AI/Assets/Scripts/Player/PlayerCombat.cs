using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    

    private PlayerController PC;
    private Animator anim;

    private AttackDetails attackDetails;

    private float lastInputTime = Mathf.NegativeInfinity;
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

    private void Start()
    {
        PC = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        inputTimer = 0.2f;

        isAttacking = false;
        isLightAttacking = false;
        isHeavyAttacking = false;
        isShielded = false;
    }

    private void Update()
    {
        UpdateAnimations();
        CheckCombatInput();
        CheckAttacks();
        //CheckShield();
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isLightAttacking", isLightAttacking);
        anim.SetBool("isHeavyAttacking", isHeavyAttacking);
        anim.SetBool("isAttacking", isAttacking);
    }

    private void CheckCombatInput()
    {
        if (Input.GetMouseButtonDown(0) && PC.getGrounded())
        {
            gotInput = true;
            gotLightInput = true;
            lastInputTime = Time.time;
        }

        if (Input.GetMouseButtonDown(1) && PC.getGrounded())
        {

            gotInput = true;
            gotHeavyInput = true;
            lastInputTime = Time.time;
        }

    }

    private void CheckAttacks()
    {
        if (gotInput)
        {

            if (gotLightInput)
            {
                if (!isLightAttacking)
                {
                    gotInput = false;
                    gotLightInput = false;
                    isAttacking = true;
                    isLightAttacking = true;
                }
            }

            if (gotHeavyInput)
            {
                if (!isHeavyAttacking)
                {
                    gotInput = false;
                    gotHeavyInput = false;
                    isAttacking = true;
                    isHeavyAttacking = true;
                }
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            //wait for input
            gotInput = false;
        }

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
