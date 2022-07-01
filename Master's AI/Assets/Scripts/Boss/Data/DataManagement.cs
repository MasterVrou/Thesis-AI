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

    private void Start()
    {
        pController = player.GetComponent<PlayerController>();
        pAnimController = player.GetComponent<PlayerAnimationController>();

        updateOnce = false;

        currentState.lightAttack = Vector3.zero;
        currentState.heavyAttack = Vector3.zero;
        currentState.offJump = Vector3.zero;
        currentState.defJump = Vector3.zero;
        currentState.dodge = Vector3.zero;
        currentState.parry = Vector3.zero;
    }

    private void Update()
    {
        UpdateDistanceLabel();

        LogPrint();
    }

    private void LogPrint()
    {
        if (!pAnimController.GetInAnimation() && !updateOnce)
        {
            updateOnce = true;

            UpdateCurrentState();

            Debug.Log("Light Data: " + currentState.lightAttack.x + ", " + currentState.lightAttack.y + ", " + currentState.lightAttack.z + "\n"
                    + "Heavy Data: " + currentState.heavyAttack.x + ", " + currentState.heavyAttack.y + ", " + currentState.heavyAttack.z);
        }

        if (pAnimController.GetInAnimation())
        {
            updateOnce = false;
        }
    }

    //go to player scripts and make variables that hold info for the skills used, or use the is.. booleans

    //1 = close 2 = mid 3 = far, sign shows direction
    private void UpdateDistanceLabel()
    {
        playerPos = player.transform.position.x;

        float distance = this.transform.position.x - playerPos;

        if (Mathf.Abs(distance) < 1.8)
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
        else if (Mathf.Abs(distance) < 3.6)
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
    }

    private void PushRight()//in future put a State parameter 
    {
        currentState.lightAttack.z = currentState.lightAttack.y;
        currentState.lightAttack.y = currentState.lightAttack.x;
        currentState.lightAttack.x = 0;

        currentState.heavyAttack.z = currentState.heavyAttack.y;
        currentState.heavyAttack.y = currentState.heavyAttack.x;
        currentState.heavyAttack.x = 0;
    }

    //Getters
}
