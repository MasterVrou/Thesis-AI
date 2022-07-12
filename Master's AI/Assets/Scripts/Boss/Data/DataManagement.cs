using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManagement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    Dictionary<PlayerState, BossAction> Qtable;

    private PlayerState currentState;
    private PlayerState lastState;

    private BossController bController;
    private BossAnimationController bAnimController;
    private PlayerController pController;
    private PlayerAnimationController pAnimController;

    private float playerPos;

    private int distanceLabel;

    
    private bool updateOnce;
    private bool someoneAlive;
    private bool actionOnce;

    /// <summary>
    /// /////////////////////////////////Q-LEARNING VARS///////////////////////////////////////////////
    /// </summary>
    private int totalEpisodes;
    private int totalSteps;
    private int stepCounter;
    private int episodeCounter;

    private float stepTimer;
    private float stepPeriod;
    
    private float epsilon;
    private float epsilonDecay;
    private float learningRate;
    private float discount;

    private float hitPlayerReward;
    private float blockPlayerReward;
    private float winReward;
    private float stepReward;

    private float fMax;
    private float currentQ;
    private float newQ;


    private float missPlayerPunishment;
    private float losePunishment;

    private string fAction;

    //maybe add reward for walking closer to player

    private string nextAction;

    private void Start()
    {
        //Qlearning
        stepCounter = 0;
        episodeCounter = 0;
        totalEpisodes = 100;
        totalSteps = 100;
        stepTimer = 0;
        stepPeriod = 1;

        hitPlayerReward = 10;
        blockPlayerReward = 10;
        winReward = 100;
        missPlayerPunishment = -10;

        epsilon = 0.8f;
        epsilonDecay = 0.9998f;
        learningRate = 0.1f;
        discount = 0.95f;

        currentQ = 0;
        newQ = 0;

        //REST
        pController = player.GetComponent<PlayerController>();
        pAnimController = player.GetComponent<PlayerAnimationController>();
        bController = GetComponent<BossController>();
        bAnimController = GetComponent<BossAnimationController>();

        updateOnce = false;
        someoneAlive = true;
        actionOnce = false;

        currentState.lightAttack = Vector2Int.zero;
        currentState.heavyAttack = Vector2Int.zero;
        currentState.offJump = Vector2Int.zero;
        currentState.defJump = Vector2Int.zero;
        currentState.dodge = Vector2Int.zero;
        currentState.parry = Vector2Int.zero;
        currentState.distance = 1;

        Qtable = new Dictionary<PlayerState, BossAction>();

        QtableSetUp();
        UpdateCurrentState();
    }

    private void Update()
    {
        UpdateDistanceLabel();
        UpdateCurrentState();
        Training();
        //LogPrint();
    }

    private void Training()
    {

        if (someoneAlive)
        {
            if (!bAnimController.GetInAction() && !actionOnce)
            {
                actionOnce = true;

                BestAction(Qtable[currentState]);

                if (Random.Range(0, 1) > epsilon)
                {
                    //fAction in BestAction function
                    nextAction = fAction;
                }
                else
                {
                    nextAction = RandomAction();
                }

                UpdateCurrentQ();

                float hpBefore = pAnimController.GetCurrentHP();

                bController.SetTrigger(nextAction);

                if (pAnimController.GetCurrentHP() < hpBefore)
                {
                    stepReward = hitPlayerReward;
                }
                else
                {
                    stepReward = missPlayerPunishment;
                }

                if (pAnimController.GetCurrentHP() <= 0)
                {
                    newQ = winReward;
                }
                else
                {

                    //newQvalue = (1 - learningRate) * currentMaxQvalue + learningRate * (stepReward + discount * nextMaxQvalue);
                    newQ = currentQ + learningRate * (stepReward + discount * fMax - currentQ);
                }
            }

            //if step is over
            if(bAnimController.GetInAction() && actionOnce)
            {
                actionOnce = false;
                UpdateQtable();
                stepCounter++;
                Debug.Log("STEP " + stepCounter);
            }
            
        }

    }

    private void UpdateCurrentQ()
    {
        if (nextAction == "Block")
        {
            currentQ = Qtable[currentState].block;
        }
        else if (nextAction == "Attack1")
        {
            currentQ = Qtable[currentState].meleeAttack;
        }
        else if (nextAction == "Attack2")
        {
            currentQ = Qtable[currentState].fireAttack;
        }
        else if(nextAction == "Charge")
        {
            currentQ = Qtable[currentState].charge;
        }
    }

    private void UpdateQtable()
    {
        BossAction a = Qtable[currentState];

        if (nextAction == "Block")
        {
            a.block = newQ;
            Qtable[currentState] = a;
        }
        else if (nextAction == "Attack1")
        {
            a.meleeAttack = newQ;
            Qtable[currentState] = a;
        }
        else if (nextAction == "Attack2")
        {
            a.fireAttack = newQ;
            Qtable[currentState] = a;
        }
        else if (nextAction == "Charge")
        {
            a.charge = newQ;
            Qtable[currentState] = a;
        }
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

    private bool EqualsState(PlayerState a, PlayerState b)
    {
        if(a.defJump==b.defJump && a.offJump==b.offJump && a.distance==b.distance && a.dodge==b.dodge && a.heavyAttack==b.heavyAttack && a.lightAttack == b.lightAttack && a.parry==b.parry)
        {
            return true;
        }

        return false;
    }

    private void BestAction(BossAction a)
    {
        fMax = a.block;
        fAction = "Block";
        if(fMax < a.charge)
        {
            fMax = a.charge;
            fAction = "Charge";
        }
        if(fMax < a.fireAttack)
        {
            fMax = a.fireAttack;
            fAction = "Attack2";
        }
        if(fMax < a.meleeAttack)
        {
            fAction = "Attack1";
        }
    }

    private string RandomAction()
    {
        int a = Random.Range(0, 4);
        
        if(a == 0)
        {
            return "Block";
        }
        else if (a == 1)
        {
            return "Attack1";
        }
        else if(a == 2)
        {
            return "Attack2";
        }
        else
        {
            return "Charge";
        }
    }

    //Getters
    public int GetDistanceLabel()
    {
        return distanceLabel;
    }
}


////is it connected to the action?
//private void NextBestState()
//{
//    List<PlayerState> ps = new List<PlayerState>();

//    if (currentState.lightAttack.x == 1)
//    {
//        for(int i=0; i<6; i++)
//        {
//            PlayerState tempState = currentState;
//            tempState.lightAttack.y = 1;
//            tempState.heavyAttack.y = 0;
//            tempState.parry.y = 0;
//            tempState.dodge.y = 0;
//            tempState.defJump.y = 0;
//            tempState.offJump.y = 0;
//            if (i == 0)
//            {
//                tempState.lightAttack.x = 1;
//                tempState.heavyAttack.x = 0;
//                tempState.parry.x = 0;
//                tempState.dodge.x = 0;
//                tempState.defJump.x = 0;
//                tempState.offJump.x = 0;

//            }
//            else if(i == 1)
//            {
//                tempState.lightAttack.x = 0;
//                tempState.heavyAttack.x = 1;
//                tempState.parry.x = 0;
//                tempState.dodge.x = 0;
//                tempState.defJump.x = 0;
//                tempState.offJump.x = 0;
//            }
//            else if(i == 2)
//            {
//                //tempState.offJump.x = 1
//            }
//        }
//    }
//    else if (currentState.heavyAttack.x == 1)
//    {

//    }
//    else if(currentState.offJump.x == 1)
//    {

//    }
//    else if(currentState.defJump.x == 1)
//    {

//    }
//    else if(currentState.dodge.x == 1)
//    {

//    }
//}


//private void Training2()
//{
//    //episodes
//    if (someoneAlive)
//    {
//        //when animation ends, step ends
//        if (!EqualsState(currentState, lastState) || stepTimer < Time.time)
//        {
//            stepTimer = Time.time + stepPeriod;
//            stepCounter++;
//            Debug.Log("step: " + stepCounter);

//            BestAction(Qtable[currentState]);

//            if (Random.Range(0,1) > epsilon)
//            {
//                //fAction in BestAction function
//                nextAction = fAction;
//            }
//            else
//            {
//                nextAction = RandomAction();
//            }

//            float currentQ = 0;
//            if (nextAction == "Block")
//            {
//                currentQ = Qtable[currentState].block;
//            }
//            else if(nextAction == "Attack1")
//            {
//                currentQ = Qtable[currentState].meleeAttack;
//            }
//            else if(nextAction == "Attack2")
//            {
//                currentQ = Qtable[currentState].fireAttack;
//            }


//            float hpBefore = pAnimController.GetCurrentHP();

//            bController.SetTrigger(nextAction);

//            if (pAnimController.GetCurrentHP() < hpBefore)
//            {
//                stepReward = hitPlayerReward;
//            }
//            else
//            {
//                stepReward = missPlayerPunishment;
//            }

//            float newQvalue;

//            if(pAnimController.GetCurrentHP() <= 0)
//            {
//                newQvalue = winReward;
//            }
//            else
//            {

//                //newQvalue = (1 - learningRate) * currentMaxQvalue + learningRate * (stepReward + discount * nextMaxQvalue);
//                newQvalue = currentQ + learningRate * (stepReward + discount * fMax - currentQ);
//            }
//        }


//        //Update Qtable

//    }
//}
