using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManagement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    Dictionary<PlayerState, BossAction> Qtable;

    private PlayerState currentState;

    private BossController bController;
    private PlayerController pController;
    private PlayerAnimationController pAnimController;

    private float playerPos;

    private int distanceLabel;

    private bool updateOnce;

    private int counter = 0;
    private float time;
    //private void Awake()
    //{
        

    //    //QtableSetUp();
    //}

    private void Start()
    {
        pController = player.GetComponent<PlayerController>();
        pAnimController = player.GetComponent<PlayerAnimationController>();

        updateOnce = false;

        currentState.lightAttack = Vector2Int.zero;
        currentState.heavyAttack = Vector2Int.zero;
        currentState.offJump = Vector2Int.zero;
        currentState.defJump = Vector2Int.zero;
        currentState.dodge = Vector2Int.zero;
        currentState.parry = Vector2Int.zero;
        currentState.distance = 1;

        Qtable = new Dictionary<PlayerState, BossAction>();

        QtableSetUp();
    }

    private void Update()
    {
        UpdateDistanceLabel();

        LogPrint();
    }

    private void QtableSetUp()
    {
        PlayerState ps;

        for(int liA=0; liA < 4; liA++)
        {
            for(int heA=0; heA < 4; heA++)
            {
                for(int parry=0; parry < 4; parry++)
                {
                    for(int dodge=0; dodge < 4; dodge++)
                    {
                        for(int offJ=0; offJ < 4; offJ++)
                        {
                            for(int defJ=0; defJ < 4; defJ++)
                            {
                                for(int deL=0; deL < 6; deL++)
                                {
                                    ps.lightAttack = SkillEntry(liA);
                                    ps.heavyAttack = SkillEntry(heA);
                                    ps.parry = SkillEntry(parry);
                                    ps.dodge = SkillEntry(dodge);
                                    ps.offJump = SkillEntry(offJ);
                                    ps.defJump = SkillEntry(defJ);
                                    ps.distance = DisanceEntry(deL);

                                    Qtable.Add(ps, ActionRandomiser());

                                    //counter++;
                                    //time += Time.deltaTime;
                                    //if(time > 5)
                                    //{
                                    //    time = 0;
                                    //    Debug.Log(counter);
                                    //}
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private BossAction ActionRandomiser()
    {
        BossAction a;

        a.block = Random.Range(-5, 5);
        a.charge = Random.Range(-5, 5);
        a.fireAttack = Random.Range(-5, 5);
        a.meleeAttack = Random.Range(-5, 5);

        return a;
    }

    private Vector2Int SkillEntry(int e)
    {
        Vector2Int entry = new Vector2Int(1, 1);

        if(e == 0)
        {
            entry = new Vector2Int(0, 0);
        }
        else if(e == 1)
        {
            entry = new Vector2Int(0, 1);
        }
        else if(e == 2)
        {
            entry = new Vector2Int(1, 0);
        }

        return entry;
    }

    private int DisanceEntry(int i)
    {
        if(i == 0)
        {
            return -3;
        }
        else if(i == 1)
        {
            return -2;
        }
        else if(i == 2)
        {
            return -1;
        }
        else if(i == 3)
        {
            return 1;
        }
        else if(i == 4)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    private void LogPrint()
    {
        //if (!pAnimController.GetInAnimation() && !updateOnce)
        //{
        //    updateOnce = true;

        //    UpdateCurrentState();

        //    Debug.Log("Light Data: " + currentState.lightAttack.x + ", " + currentState.lightAttack.y + "\n"
        //            + "Heavy Data: " + currentState.heavyAttack.x + ", " + currentState.heavyAttack.y + "\n"
        //            + "Dodge Data: " + currentState.dodge.x + ", " + currentState.dodge.y + ", " + "\n"
        //            + "Parry Data: " + currentState.parry.x + ", " + currentState.parry.y + ", " + "\n"
        //            + "DefJump Data: " + currentState.defJump.x + ", " + currentState.defJump.y + "\n"
        //            + "OfJump Data: " + currentState.offJump.x + ", " + currentState.offJump.y);
        //}

        //if (pAnimController.GetInAnimation())
        //{
        //    updateOnce = false;
        //}

        if (!updateOnce)
        {
            updateOnce = true;

            PlayerState ps;
            ps.lightAttack = Vector2Int.zero;
            ps.heavyAttack = Vector2Int.zero;
            ps.parry = Vector2Int.zero;
            ps.dodge = Vector2Int.zero;
            ps.offJump = Vector2Int.zero;
            ps.defJump = Vector2Int.zero;
            ps.distance = 1;
            BossAction bs;
            bs.block = 0;
            bs.charge = 0;
            bs.fireAttack = 0;
            bs.meleeAttack = 0;

            //Qtable.Add(ps, bs);

            Debug.Log(Qtable[ps].block);
        }
    }

    //go to player scripts and make variables that hold info for the skills used, or use the is.. booleans

    //1 = close 2 = mid 3 = far, sign shows direction
    private void UpdateDistanceLabel()
    {
        playerPos = player.transform.position.x;

        float distance = this.transform.position.x - playerPos;

        if (Mathf.Abs(distance) < 2.5)
        {
            if (distance >= 0)
            {
                distanceLabel = 1;
            }
            else
            {
                distanceLabel = -1;
            }
            return;
        }
        else if (Mathf.Abs(distance) < 4)
        {
            if (distance >= 0)
            {
                distanceLabel = 2;
            }
            else
            {
                distanceLabel = -2;
            }
            return;
        }
        else
        {
            if (distance >= 0)
            {
                distanceLabel = 3;
            }
            else
            {
                distanceLabel = -3;
            }
        }
    }

    private void UpdateCurrentState()
    {

        PushRight();

        if (pAnimController.GetLastMove() == "light")
        {
            currentState.lightAttack.x = 1;
        }
        else if (pAnimController.GetLastMove() == "heavy")
        {
            currentState.heavyAttack.x = 1;
        }
        else if(pAnimController.GetLastMove() == "dodge")
        {
            currentState.dodge.x = 1;
        }
        else if(pAnimController.GetLastMove() == "parry")
        {
            currentState.parry.x = 1;
        }
        else if(pAnimController.GetLastMove() == "jump")
        {
            float bossPos = this.transform.position.x;
            float sDist = pAnimController.GetSJump();
            float fDist = pAnimController.GetFJump();

            if(Mathf.Abs(bossPos - sDist) > Mathf.Abs(bossPos - fDist))
            {
                currentState.offJump.x = 1;
            }
            else
            {
                currentState.defJump.x = 1;
            }
        }
    }


    private void PushRight()//in future put a State parameter 
    {
        currentState.lightAttack.y = currentState.lightAttack.x;
        currentState.lightAttack.x = 0;

        currentState.heavyAttack.y = currentState.heavyAttack.x;
        currentState.heavyAttack.x = 0;

        currentState.dodge.y = currentState.dodge.x;
        currentState.dodge.x = 0;

        currentState.parry.y = currentState.parry.x;
        currentState.parry.x = 0;

        currentState.defJump.y = currentState.defJump.x;
        currentState.defJump.x = 0;

        currentState.offJump.y = currentState.offJump.x;
        currentState.offJump.x = 0;
    }

    //Getters
    public int GetDistanceLabel()
    {
        return distanceLabel;
    }
}
