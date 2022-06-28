using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private BossController bController;
    private PlayerController pController;

    private float playerPos;

    private string lastMove;
    private string previousMove;
    private string prepreviousMove;

    private int distanceLabel;

    private bool once;

    private void Start()
    {
        pController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateDistanceLabel();

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
}
