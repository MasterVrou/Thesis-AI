using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;

public class DataManagement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Text score;

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
    private int playerWins;
    private int bossWins;
    public int sameMoveCounter;

    private bool updateOnce;
    private bool someoneAlive;
    private bool bossDied;
    private bool playerDied;
    private bool actionOnce;
    private bool trainning;

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
    private string lastMove;

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

        hitPlayerReward = 5;
        blockPlayerReward = 7;
        goodBlockReward = 7;
        winReward = 10;
        missPlayerPunishment = -2;
        missBlockPunishment = -2;
        losePunishment = -10;

        epsilon = 0.1f;
        //epsilonDecay = 0.9998f;
        learningRate = 0.9f;
        discount = 0.1f;

        currentQ = 0;
        newQ = 0;

        //REST
        pController = player.GetComponent<PlayerController>();
        pAnimController = player.GetComponent<PlayerAnimationController>();
        bController = GetComponent<BossController>();
        bAnimController = GetComponent<BossAnimationController>();

        GN = GetComponent<GenAlgorithm>();
        NN = GetComponent<NNetwork>();
        NN.Initialise(1, 10);

        updateOnce = false;
        someoneAlive = true;
        actionOnce = false;
        bossDied = false;
        playerDied = false;
        trainning = true;

        currentState.lightAttack = Vector2Int.zero;
        currentState.heavyAttack = Vector2Int.zero;
        currentState.rangeAttack = Vector2Int.zero;
        currentState.jump = Vector2Int.zero;
        currentState.dodge = Vector2Int.zero;
        currentState.parry = Vector2Int.zero;
        currentState.distance = 1;

        playerWins = 0;
        bossWins = 0;
        sameMoveCounter = 0;

        lastMove = "lol";

        Qtable = new Dictionary<PlayerState, BossAction>();

        QtableSetUp();
        UpdateCurrentState();

        //Genetic algorithm stuff
        //GN.ResetNetwork(overallFitness);

        score.text = playerWins.ToString() + " : " + bossWins.ToString();
    }

    private void Update()
    {
        CheckGeneration();
        UpdateDistanceLabel();
        UpdateCurrentState();
        //NN_Training();
        Q_Training();
        //LogPrint();

        score.text = playerWins.ToString() + " : " + bossWins.ToString();
    }

    private void FixedUpdate()
    {
        
    }

    private void NN_Training()
    {
        if (trainning)
        {
            if (!bossDied && !playerDied)
            {
                if (!bAnimController.GetInAction() && !actionOnce)
                {
                    BossAction a = NN.RunNetwork(currentState);
                    BestAction(a);
                    nextAction = fAction;

                    if(lastMove == nextAction)
                    {
                        sameMoveCounter++;

                        if(sameMoveCounter > 5)
                        {
                            bossDied = true;
                            sameMoveCounter = 0;
                        }
                    }
                    else
                    {
                        sameMoveCounter = 0;
                    }

                    //Debug.Log("block " + a.block + ", meleeAttack " + a.meleeAttack + ", fireAttack" + a.fireAttack);

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
                    else if (boss == "Warlock")
                    {
                        bController.SetWarlockAction(nextAction);
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

            lastMove = nextAction;
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
            bossWins++;
        }
        if (bController.GetCurrentHealth() <= 0)
        {
            reward = losePunishment;
            bossDied = true;
            playerWins++;
        }
        //////////////////////////////////////////////////////////////Update Qtable/////////////////////////
        newQ = reward;
        UpdateQtable();

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
                else if(boss == "Warlock")
                {
                    bController.SetWarlockAction(nextAction);
                }

                //give different reward for hook

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
                    bossWins++;
                }
                else if(bController.GetCurrentHealth() <= 0)
                {
                    newQ = losePunishment;
                    bossDied = true;
                    playerWins++;
                }
                else
                {

                    //newQvalue = (1 - learningRate) * currentMaxQvalue + learningRate * (stepReward + discount * nextMaxQvalue);
                    //newQ = currentQ + learningRate * (stepReward + discount * fMax - currentQ);
                }
                newQ = currentQ + learningRate * (stepReward + discount * fMax - currentQ);
                //newQ = (float)System.Math.Tanh(newQ);
                Debug.Log("newQ = " + newQ);
            }

            //if step is over
            if(bAnimController.GetInAction() && actionOnce)
            {
                episodeReward += newQ;
                actionOnce = false;
                UpdateQtable();
                stepCounter++;
                //Debug.Log("STEP " + stepCounter);
                //epsilon *= epsilonDecay;
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
        else if(nextAction == "Hook")
        {
            currentQ = Qtable[currentState].hook;
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
        else if (nextAction == "Hook")
        {
            a.hook = newQ;
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
        else if (boss != "Warlock" && (nextAction == "Hook"))
        {
            bController.BossSwap("Warlock");
            boss = "Warlock";
        }
    }

    
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

            for (int liA = 0; liA < 4; liA++)
            {
                for (int heA = 0; heA < 4; heA++)
                {
                    for (int parry = 0; parry < 4; parry++)
                    {
                        for (int dodge = 0; dodge < 4; dodge++)
                        {
                            for (int offJ = 0; offJ < 4; offJ++)//rangeAttack
                            {
                                for (int defJ = 0; defJ < 4; defJ++)
                                {
                                    for (int deL = 0; deL < 3; deL++)
                                    {
                                        ps.lightAttack = SkillEntry(liA);
                                        ps.heavyAttack = SkillEntry(heA);
                                        ps.parry = SkillEntry(parry);
                                        ps.dodge = SkillEntry(dodge);
                                        ps.rangeAttack = SkillEntry(offJ);
                                        ps.jump = SkillEntry(defJ);
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

    private void CheckGeneration()
    {
        if (GN.GetGeneration() == 200)
        {
            trainning = false;
            SaveToJSON();
            Application.Quit();
        }
    }


    public ReadLoadList readLoadList = new ReadLoadList();
    public static string fileName = "/text.json";
    private void SaveToJSON()
    {
        readLoadList.rlList.Clear();
        ReadLoad rl;

        foreach (KeyValuePair<PlayerState, BossAction> entry in Qtable)
        {
            rl.lightAttack1 = entry.Key.lightAttack.x;
            rl.lightAttack2 = entry.Key.lightAttack.y;
            rl.heavyAttack1 = entry.Key.heavyAttack.x;
            rl.heavyAttack2 = entry.Key.heavyAttack.y;
            rl.rangeAttack1 = entry.Key.rangeAttack.x;
            rl.rangeAttack2 = entry.Key.rangeAttack.y;
            rl.jump1 = entry.Key.jump.x;
            rl.jump2 = entry.Key.jump.y;
            rl.dodge1 = entry.Key.dodge.x;
            rl.dodge2 = entry.Key.dodge.y;
            rl.parry1 = entry.Key.parry.x;
            rl.parry2 = entry.Key.parry.y;
            rl.distance = entry.Key.distance;

            rl.meleeAttack = entry.Value.meleeAttack;
            rl.fireAttack = entry.Value.fireAttack;
            rl.block = entry.Value.block;
            rl.charge = entry.Value.charge;
            rl.firepillar = entry.Value.firepillar;
            rl.fireball = entry.Value.fireball;
            rl.hook = entry.Value.hook;

            readLoadList.rlList.Add(rl);
        }

        string strOutput = JsonConvert.SerializeObject(readLoadList, Formatting.Indented);

        

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
                                for (int deL = 0; deL < 3; deL++)
                                {
                                    
                                    PlayerState ps;
                                    ps.lightAttack = new Vector2Int(readLoadList.rlList[iterator].lightAttack1, readLoadList.rlList[iterator].lightAttack2);
                                    ps.heavyAttack = new Vector2Int(readLoadList.rlList[iterator].heavyAttack1, readLoadList.rlList[iterator].heavyAttack2);
                                    ps.rangeAttack = new Vector2Int(readLoadList.rlList[iterator].rangeAttack1, readLoadList.rlList[iterator].rangeAttack2);
                                    ps.jump = new Vector2Int(readLoadList.rlList[iterator].jump1, readLoadList.rlList[iterator].jump2);
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
                                    bs.hook = readLoadList.rlList[iterator].hook;

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
        a.hook = Random.Range(-5, 5);

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
            return 0;
        }
        else if(i == 1)
        {
            return 1;
        }
        else
        {
            return 2;
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
            ps.rangeAttack = Vector2Int.zero;
            ps.jump = Vector2Int.zero;
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

    //0 = close 1 = mid 2 = far, sign shows direction
    private void UpdateDistanceLabel()
    {
        playerPos = player.transform.position.x;

        float distance = this.transform.position.x - playerPos;

        if (Mathf.Abs(distance) < 2.5)
        {
            distanceLabel = 0;
            return;
        }
        else if(Mathf.Abs(distance) < 5.5)
        {
            distanceLabel = 1;
        }
        else
        {
            distanceLabel = 2;
        }

        //if (Mathf.Abs(distance) < 2.5)
        //{
        //    if (distance >= 0)
        //    {
        //        distanceLabel = 1;
        //    }
        //    else
        //    {
        //        distanceLabel = -1;
        //    }
        //    return;
        //}
        //else
        //{
        //    if (distance >= 0)
        //    {
        //        distanceLabel = 2;
        //    }
        //    else
        //    {
        //        distanceLabel = -2;
        //    }
        //    return;
        //}

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
        else if (pAnimController.GetLastMove() == "range")
        {
            currentState.rangeAttack.x = 1;
        }
        else if(pAnimController.GetLastMove() == "jump")
        {
            currentState.jump.x = 1;
            //float bossPos = this.transform.position.x;
            //float sDist = pAnimController.GetSJump();
            //float fDist = pAnimController.GetFJump();

            //if(Mathf.Abs(bossPos - sDist) > Mathf.Abs(bossPos - fDist))
            //{
            //    currentState.offJump.x = 1;
            //}
            //else
            //{
            //    currentState.defJump.x = 1;
            //}
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

        currentState.rangeAttack.y = currentState.rangeAttack.x;
        currentState.rangeAttack.x = 0;

        currentState.jump.y = currentState.jump.x;
        currentState.jump.x = 0;
    }

    private bool EqualsState(PlayerState a, PlayerState b)
    {
        if(a.jump==b.jump && a.rangeAttack==b.rangeAttack && a.distance==b.distance && a.dodge==b.dodge && a.heavyAttack==b.heavyAttack && a.lightAttack == b.lightAttack && a.parry==b.parry)
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
        if(fMax < a.hook)
        {
            fAction = "Hook";
        }
    }

    private string RandomAction()
    {
        int a = Random.Range(0, 7);
        
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
        else if(a == 5)
        {
            return "Firepillar";
        }
        else
        {
            return "Hook";
        }
    }

    //Getters
    public int GetDistanceLabel()
    {
        return distanceLabel;
    }

    public string GetBoss()
    {
        return boss;
    }

    public float GetPlayerHP()
    {
        return pAnimController.GetCurrentHP();
    }
}