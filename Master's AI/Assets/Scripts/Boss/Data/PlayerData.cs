using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private BossController bController;
    private PlayerController pController;
    private PlayerAnimationController pAnimController;

    private float playerPos;

    private string lastMove;

    private int distanceLabel;

    private bool flLight;
    private bool flHeavy;
    private bool flDodge;
    private bool flParry;
    private bool flJump;

    private bool checkOnce;

    private void Start()
    {
        pController = player.GetComponent<PlayerController>();
        pAnimController = player.GetComponent<PlayerAnimationController>();

        flLight = false;
        flHeavy = false;
        flDodge = false;
        flParry = false;
        flJump = false;

        checkOnce = false;
    }

    private void Update()
    {
        UpdateDistanceLabel();
        CheckLastMove();
    }
    
    //go to player scripts and make variables that hold info for the skills used, or use the is.. booleans

    //1 = close 2 = mid 3 = far, sign shows direction
    private void UpdateDistanceLabel()
    {
        playerPos = player.transform.position.x;

        float distance = this.transform.position.x - playerPos;

        if (Mathf.Abs(distance) < 1.8)
        {
            if(distance >= 0)
            {
                distanceLabel = 1;
            }
            else
            {
                distanceLabel = -1;
            }
            return;
        }
        else if(Mathf.Abs(distance) < 3.6)
        {
            if(distance >= 0)
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

    private void CheckLastMove()
    {
        if (pAnimController.GetInAnimation() && !checkOnce)
        {
            checkOnce = true;

            if (pAnimController.GetLastMove() == "light")
            {
                lastMove = "light";
            }

            //if (flLight != pAnimController.GetFlLight())
            //{
            //    lastMove = "light";
            //    flLight = !flLight;
            //}
            //else if (flHeavy != pAnimController.GetFlHeavy())
            //{
            //    lastMove = "heavy";
            //    flHeavy = !flHeavy;
            //}
        }

        

        if (!pAnimController.GetInAnimation())
        {
            checkOnce = false;
        }
    }

    //Getters
    public int GetDistanceLabel()
    {
        return distanceLabel;
    }

    public string GetLastMove()
    {
        return lastMove;
    }

}
