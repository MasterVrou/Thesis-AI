using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAI : MonoBehaviour
{
    [SerializeField]
    private float timeScale = 1;

    [SerializeField]
    private GameObject boss;

    private PlayerController pController;
    private PlayerCombat pCombat;
    private DataManagement bData;

    private float random;
    private float nextActionTime;
    private float skillPeriod;
    private float walkPeriod;

    private int distanceLabel;

    private void Start()
    {
        pController = GetComponent<PlayerController>();
        pCombat = GetComponent<PlayerCombat>();
        bData = boss.GetComponent<DataManagement>();

        distanceLabel = bData.GetDistanceLabel();
        nextActionTime = 0;

        skillPeriod = 0.8f;
        walkPeriod = 0.1f;
        Time.timeScale = timeScale;
    }
    
    private void Update()
    {
        CheckSprites();
        MakeDicision();
    }

    private void CheckSprites()
    {
        if (boss.transform.position.x > this.transform.position.x)
        {
            if (!pController.GetIsFacingRight())
            {
                pController.AutoFlip();
            }
        }
        else
        {
            if (pController.GetIsFacingRight())
            {
                pController.AutoFlip();
            }
        }
    }
    private void MakeDicision()
    {
        distanceLabel = bData.GetDistanceLabel();

        if (Mathf.Abs(distanceLabel) == 1)
        {
            if (Time.time > (nextActionTime * timeScale))
            {
                nextActionTime += skillPeriod / timeScale;
                pController.SetIsWalking(false);
                ChooseSkill();
            }
        }
        else
        {
            pController.SetIsWalking(true);
            CloseDistance();
        }
    }

    private void CloseDistance()
    {
        //if(boss.transform.position.x > this.transform.position.x)
        //{
        //    if (!pController.GetIsFacingRight())
        //    {
        //        pController.AutoFlip();
        //    }

        //        pController.SetMovementDirection(1);
        //}
        //else
        //{
        //    if (pController.GetIsFacingRight())
        //    {
        //        pController.AutoFlip();
        //    }

        //    pController.SetMovementDirection(-1);
        //}

        if (pController.GetIsFacingRight())
        {
            pController.SetMovementDirection(1);
        }
        else
        {
            pController.SetMovementDirection(-1);
        }
    }

    private void ChooseSkill()
    {
        
        if (Random.Range(0, 10) <= 3)
        {
            float r = Random.Range(0, 3);

            if (r == 0)
            {
                pController.Jump();
            }
            else if (r == 1)
            {
                pController.Dodge();
            }
            else
            {
                pCombat.Parry();
            }
        }
        else
        {
            float r = Random.Range(0, 2);

            if (r == 0)
            {
                pCombat.LightAttack();
            }
            else
            {
                pCombat.HeavyAttack();
            }
        }
    }
}
