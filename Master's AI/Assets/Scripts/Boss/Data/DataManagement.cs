using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class DataManagement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    Dictionary<PlayerState, BossAction> Qtable;

    private NNetwork NN;
    private GenAlgorithm GN;

    public TextAsset textJSON;

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
    private bool bossDied;
    private bool playerDied;
    private bool actionOnce;

    private string boss;

    //NNETWORK
    public float overallFitness = 0;

    //Q-LEARNING
    private int totalEpisodes;
    private int totalSteps;
    private int stepCounter;
    private int episodeCounter;
    private float episodeReward;

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
    private float goodBlockReward;

    private float fMax;
    private float currentQ;
    private float newQ;

    private float missBlockPunishment;
    private float missPlayerPunishment;
    private float losePunishment;

    private string fAction;

    //maybe add reward for walking closer to player

    private string nextAction;

    private void Start()
    {
        boss = "Knight";

        //Qlearning
        stepCounter = 0;
        episodeCounter = 0;
        episodeReward = 0;
        totalEpisodes = 100;
        totalSteps = 100;
        stepTimer = 0;
        stepPeriod = 1;

        hitPlayerReward = 10;
        blockPlayerReward = 10;
        goodBlockReward = 20;
        winReward = 100;
        missPlayerPunishment = -10;
        missBlockPunishment = -5;
        losePunishment = -100;

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

        GN = GetComponent<GenAlgorithm>();
        NN = GetComponent<NNetwork>();
        NN.Initialise(2, 10);

        updateOnce = false;
        someoneAlive = true;
        actionOnce = false;
        bossDied = false;
        playerDied = false;

        currentState.lightAttack = Vector2Int.zero;
        currentState.heavyAttack = Vector2Int.zero;
        currentState.offJump = Vector2Int.zero;
        currentState.defJump = Vector2Int.zero;
        currentState.dodge = Vector2Int.zero;
        currentState.parry = Vector2Int.zero;
        currentState.distance = 1;

        Qtable = new Dictionary<PlayerState, BossAction>();

        //QtableSetUp();
        UpdateCurrentState();

        //Genetic algorithm stuff
        //GN.ResetNetwork(overallFitness);
    }

    private void Update()
    {
        UpdateDistanceLabel();
        UpdateCurrentState();
        NN_Training();
        //Q_Training();
        //LogPrint();
    }

    private void FixedUpdate()
    {
        
    }

    private void NN_Training()
    {
        if(!bossDied && !playerDied)
        {
            if(!bAnimController.GetInAction() && !actionOnce)
            {   
                BestAction(NN.RunNetwork(currentState));
                nextAction = fAction;

                float hpBefore = pAnimController.GetCurrentHP();

                UpdateBossForm();
                if (boss == "Knight")
                {
                    bController.SetKnightAction(nextAction);
                }
                else if (boss == "Mage")
                {
                    bController.SetMageAction(nextAction);
                }

                CalculateFitness(hpBefore);

                if (bAnimController.GetInAction() && actionOnce)
                {
                    actionOnce = false;
                }
            }
        }
        else
        {
            bossDied = false;
            playerDied = false;
            bController.Respawn();
            pAnimController.Respawn();
            NN = GN.ResetNetwork(overallFitness);
            overallFitness = 0;
        }
    }

    private void CalculateFitness(float hpBefore)
    {
        float reward = 0;
        if (nextAction == "Block")
        {
            if (bAnimController.GetDamageBlocked())
            {
                reward = goodBlockReward;
            }
            else
            {
                reward = missBlockPunishment;
            }
        }
        else
        {
            if (pAnimController.GetCurrentHP() < hpBefore)
            {
                reward = hitPlayerReward;
            }
            else
            {
                reward = missPlayerPunishment;
            }
        }


        if (pAnimController.GetCurrentHP() <= 0)
        {
            reward = winReward;
            playerDied = true;
        }
        if (bController.GetCurrentHealth() <= 0)
        {
            reward = losePunishment;
            bossDied = true;
        }

        overallFitness += reward/10;
    }

    private void Q_Training()
    {

        if (!bossDied && !playerDied)
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

                UpdateBossForm();
                if(boss == "Knight")
                {
                    bController.SetKnightAction(nextAction);
                }
                else if(boss == "Mage")
                {
                    bController.SetMageAction(nextAction);
                }

                if(nextAction == "Block")
                {
                    if (bAnimController.GetDamageBlocked())
                    {
                        stepReward = goodBlockReward;
                    }
                    else
                    {
                        stepReward = missBlockPunishment;
                    }
                }
                else
                {
                    if (pAnimController.GetCurrentHP() < hpBefore)
                    {
                        stepReward = hitPlayerReward;
                    }
                    else
                    {
                        stepReward = missPlayerPunishment;
                    }
                }
                

                if (pAnimController.GetCurrentHP() <= 0)
                {
                    newQ = winReward;
                    playerDied = true;
                }
                if(bController.GetCurrentHealth() <= 0)
                {
                    newQ = losePunishment;
                    bossDied = true;
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
                episodeReward += newQ;
                actionOnce = false;
                UpdateQtable();
                stepCounter++;
                Debug.Log("STEP " + stepCounter);
                epsilon *= epsilonDecay;
            }
            
        }
        else
        {
            
            //add to the total reward
            bossDied = false;
            playerDied = false;
            bController.Respawn();
            pAnimController.Respawn();
            episodeCounter++;
            Debug.Log("EPISODE NUMBER: " + episodeCounter + "\n" +
                      "EPISODE REWARD: " + episodeReward + "\n" +
                      "EPSILON: " + epsilon);
            //maybe keep all the episode rewards in a list
            episodeReward = 0;
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
        else if(nextAction == "Fireball")
        {
            a.fireball = newQ;
            Qtable[currentState] = a;
        }
        else if(nextAction == "Firepillar")
        {
            a.firepillar = newQ;
            Qtable[currentState] = a;
        }
    }

    private void UpdateBossForm()
    {
        if (boss != "Knight" && (nextAction == "Attack1" || nextAction == "Attack2" || nextAction == "Charge" || nextAction == "Block"))
        {
            bController.BossSwap("Knight");
            boss = "Knight";
        }
        else if (boss != "Mage" && (nextAction == "Firepillar" || nextAction == "Fireball"))
        {
            bController.BossSwap("Mage");
            boss = "Mage";
        }
    }

    public ReadLoadList readLoadList = new ReadLoadList();
    public static string fileName = "/text.json";
    private void QtableSetUp()
    {
        string fullPath = Application.dataPath + "/text.json";
        if (File.Exists(fullPath))
        {
            LoadFromJson();
        }
        else
        {
            PlayerState ps;
            ReadLoad rl;

            for (int liA = 0; liA < 4; liA++)
            {
                for (int heA = 0; heA < 4; heA++)
                {
                    for (int parry = 0; parry < 4; parry++)
                    {
                        for (int dodge = 0; dodge < 4; dodge++)
                        {
                            for (int offJ = 0; offJ < 4; offJ++)
                            {
                                for (int defJ = 0; defJ < 4; defJ++)
                                {
                                    for (int deL = 0; deL < 6; deL++)
                                    {
                                        ps.lightAttack = SkillEntry(liA);
                                        ps.heavyAttack = SkillEntry(heA);
                                        ps.parry = SkillEntry(parry);
                                        ps.dodge = SkillEntry(dodge);
                                        ps.offJump = SkillEntry(offJ);
                                        ps.defJump = SkillEntry(defJ);
                                        ps.distance = DisanceEntry(deL);

                                        BossAction bs = ActionRandomiser();
                                        Qtable.Add(ps, bs);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //write the data to jason for testing
        //SaveToJSON();
        //LoadFromJson();
    }

    private void SaveToJSON()
    {
        string strOutput = JsonConvert.SerializeObject(readLoadList, Formatting.Indented);

        readLoadList.rlList.Clear();

        File.WriteAllText(Application.dataPath + "/text.json", strOutput);
    }
    
    private void LoadFromJson()
    {
        readLoadList = JsonUtility.FromJson<ReadLoadList>(textJSON.text);
        int iterator = 0;

        for (int liA = 0; liA < 4; liA++)
        {
            for (int heA = 0; heA < 4; heA++)
            {
                for (int parry = 0; parry < 4; parry++)
                {
                    for (int dodge = 0; dodge < 4; dodge++)
                    {
                        for (int offJ = 0; offJ < 4; offJ++)
                        {
                            for (int defJ = 0; defJ < 4; defJ++)
                            {
                                for (int deL = 0; deL < 6; deL++)
                                {
                                    
                                    PlayerState ps;
                                    ps.lightAttack = new Vector2Int(readLoadList.rlList[iterator].lightAttack1, readLoadList.rlList[iterator].lightAttack2);
                                    ps.heavyAttack = new Vector2Int(readLoadList.rlList[iterator].heavyAttack1, readLoadList.rlList[iterator].heavyAttack2);
                                    ps.offJump = new Vector2Int(readLoadList.rlList[iterator].offJump1, readLoadList.rlList[iterator].offJump2);
                                    ps.defJump = new Vector2Int(readLoadList.rlList[iterator].defJump1, readLoadList.rlList[iterator].defJump2);
                                    ps.dodge = new Vector2Int(readLoadList.rlList[iterator].dodge1, readLoadList.rlList[iterator].dodge2);
                                    ps.parry = new Vector2Int(readLoadList.rlList[iterator].parry1, readLoadList.rlList[iterator].parry2);
                                    ps.distance = readLoadList.rlList[iterator].distance;

                                    BossAction bs;
                                    bs.meleeAttack = readLoadList.rlList[iterator].meleeAttack;
                                    bs.fireAttack = readLoadList.rlList[iterator].fireAttack;
                                    bs.charge = readLoadList.rlList[iterator].charge;
                                    bs.block = readLoadList.rlList[iterator].block;
                                    bs.fireball = readLoadList.rlList[iterator].fireAttack;
                                    bs.firepillar = readLoadList.rlList[iterator].firepillar;

                                    Qtable.Add(ps, bs);
                                    //Debug.Log("parto: " + Qtable[b]);

                                    iterator++;
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
        a.fireball = Random.Range(-5, 5);
        a.firepillar = Random.Range(-5, 5);

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
        if(fMax < a.fireball)
        {
            fAction = "Fireball";
        }
        if(fMax < a.firepillar)
        {
            fAction = "Firepillar";
        }
    }

    private string RandomAction()
    {
        int a = Random.Range(0, 6);
        
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
        else if(a == 3)
        {
            return "Charge";
        }
        else if(a == 4)
        {
            return "Fireball";
        }
        else
        {
            return "Firepillar";
        }
    }

    //Getters
    public int GetDistanceLabel()
    {
        return distanceLabel;
    }
}