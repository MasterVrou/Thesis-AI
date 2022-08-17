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
    private float runAwayDuration;
    private float runAwayStartTime;

    public bool isScared;
    private bool isRunningAway;
    private bool runOnce;

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

        isScared = false;
        isRunningAway = true;
        runOnce = false;


        runAwayDuration = 1f;
    }
    
    private void Update()
    {
        CheckSprites();
        MakeDicision();
        CheckRunning();

        Time.timeScale = timeScale;
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

        if (!isScared)
        {
            if (Mathf.Abs(distanceLabel) == 0)
            {
                if (Time.time > (nextActionTime * timeScale))
                {
                    nextActionTime += skillPeriod / timeScale;
                    pController.SetIsWalking(false);
                    ChooseSkillOffensive();
                }
            }
            else
            {
                pController.SetIsWalking(true);
                CloseDistance();
            }
        }
        else
        {
            StartRunning();
            pController.SetIsWalking(true);
            if (bData.transform.position.x > this.transform.position.x)
            {
                pController.SetMovementDirection(-1);
            }
            else
            {
                pController.SetMovementDirection(1);
            }
        }
            
    }

    private void CloseDistance()
    {
        if (pController.GetIsFacingRight())
        {
            pController.SetMovementDirection(1);
        }
        else
        {
            pController.SetMovementDirection(-1);
        }
    }

    private void StartRunning()
    {
        if (!runOnce)
        {
            runOnce = true;
            runAwayStartTime = Time.time;
            isRunningAway = true;
        }
    }

    private void CheckRunning()
    {
        if((runAwayStartTime + runAwayDuration) * timeScale < Time.time *Time.timeScale && isRunningAway)
        {
            pController.SetIsWalking(false);
            isRunningAway = false;
            pCombat.RangeAttack();
            isScared = false;
            runOnce = false;
            nextActionTime = (Time.time + skillPeriod) / timeScale;
        }
    }

    private void ChooseSkillOffensive()
    {
        
        if (Random.Range(0, 10) <= 3)
        {
            float r = Random.Range(0, 3);

            if (r == 0)
            {
                pController.Jump();
                if (Random.Range(0, 2) == 0)
                {
                    isScared = true;
                }
            }
            else if (r == 1)
            {
                pController.Dodge();
            }
            else
            {
                pCombat.Parry();
                if (Random.Range(0, 2) == 0)
                {
                    isScared = true;
                }
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
                if(Random.Range(0, 2) == 0)
                {
                    isScared = true;
                }
                
            }
        }
    }
}
