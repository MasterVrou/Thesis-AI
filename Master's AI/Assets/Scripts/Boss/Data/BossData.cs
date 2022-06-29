using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossData : MonoBehaviour
{
    Dictionary<PlayerState, BossAction> Qtable;

    private PlayerState testState;

    private PlayerData playerData;

    private void Start()
    {
        playerData = GetComponent<PlayerData>();
        
        testState.lightAttack = Vector3.zero;
        testState.heavyAttack = Vector3.zero;
        testState.offJump = Vector3.zero;
        testState.defJump = Vector3.zero;
        testState.dodge = Vector3.zero;
        testState.parry = Vector3.zero;

        
    }

    //put a safty to not push right in every update
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StateUpdate();

            Debug.Log("Light Data: " + testState.lightAttack.x + ", " + testState.lightAttack.y + ", " + testState.lightAttack.z + "\n"
                    + "Heavy Data: " + testState.heavyAttack.x + ", " + testState.heavyAttack.y + ", " + testState.heavyAttack.z);
        }

        

    }

    private void StateUpdate()
    {
        PushRight();

        if(playerData.GetLastMove() == "light")
        {
            testState.lightAttack.x = 1;
        }

        if (playerData.GetLastMove() == "heavy")
        {
            testState.heavyAttack.x = 1;
        }
    }

    private void PushRight()//in future put a State parameter 
    {
        testState.lightAttack.y = testState.lightAttack.x;
        testState.lightAttack.z = testState.lightAttack.y;

        testState.heavyAttack.y = testState.heavyAttack.x;
        testState.heavyAttack.z = testState.heavyAttack.y;

    }
}
