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
    private PlayerController pController;
    private PlayerAnimationController pAnimController;

    private float playerPos;

    private int distanceLabel;
    
    

    private bool updateOnce;
    private bool someoneAlive;

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

    private float missPlayerPunishment;
    private float losePunishment;
    
    
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

        //REST
        pController = player.GetComponent<PlayerController>();
        pAnimController = player.GetComponent<PlayerAnimationController>();

        updateOnce = false;
        someoneAlive = true;

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

        lastState = currentState;
    }

    private void Update()
    {
        UpdateDistanceLabel();
        UpdateCurrentState();

        //LogPrint();
    }

    private void Training()
    {
        //episodes
        if (someoneAlive)
        {
            //!EqualsState(currentState, lastState) will change to (if boss not in animation)
            //steps
            if (!EqualsState(currentState, lastState) || stepTimer < Time.time)
            {
                stepTimer = Time.time + stepPeriod;
                stepCounter++;
                Debug.Log("step: " + stepCounter);

                float currentMaxQvalue;
                if(Random.Range(0,1) > epsilon)
                {
                    nextAction = BestAction(Qtable[currentState]);
                    
                }
                else
                {
                    nextAction = RandomAction();
                }
                currentMaxQvalue = maxQ;
                float hpBefore = pAnimController.GetCurrentHP();

                //call function that activates action

                if (pAnimController.GetCurrentHP() < hpBefore)
                {
                    stepReward = hitPlayerReward;
                }
                else
                {
                    stepReward = missPlayerPunishment;
                }

                float newQvalue;

                if(pAnimController.GetCurrentHP() <= 0)
                {
                    newQvalue = winReward;
                }
                else
                {
                    //this part feels weird
                    //I need to wait and see his next action before updating the Qtable
                    UpdateCurrentState();
                    string a = BestAction(Qtable[currentState]);//need to make BestAction void
                    float maxNextQvalue = maxQ;

                    newQvalue = (1 - learningRate) * currentMaxQvalue + learningRate * (stepReward + discount * maxNextQvalue);
                }
                //make an update qtable fuction
                //Qtable[currentState].
            }

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

    private bool EqualsState(PlayerState a, PlayerState b)
    {
        if(a.defJump==b.defJump && a.offJump==b.offJump && a.distance==b.distance && a.dodge==b.dodge && a.heavyAttack==b.heavyAttack && a.lightAttack == b.lightAttack && a.parry==b.parry)
        {
            return true;
        }

        return false;
    }

    private float maxQ;
    private string BestAction(BossAction a)
    {
        maxQ = a.block;
        string action = "block";
        if(maxQ < a.charge)
        {
            maxQ = a.charge;
            action = "charge";
        }
        if(maxQ < a.fireAttack)
        {
            maxQ = a.fireAttack;
            action = "fAttack";
        }
        if(maxQ < a.meleeAttack)
        {
            action = "mAttack";
        }
        return action;
    }

    private string RandomAction()
    {
        int a = Random.Range(0, 4);
        
        if(a == 0)
        {
            return "block";
        }
        else if (a == 1)
        {
            return "mAttack";
        }
        else if(a == 2)
        {
            return "fAttack";
        }
        else
        {
            return "charge";
        }
    }

    //Getters
    public int GetDistanceLabel()
    {
        return distanceLabel;
    }
}
