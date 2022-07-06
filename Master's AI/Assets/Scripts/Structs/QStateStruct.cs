using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1.572.864 States
public struct PlayerState
{
    public Vector2Int lightAttack;
    public Vector2Int heavyAttack;
    public Vector2Int offJump;
    public Vector2Int defJump;
    public Vector2Int dodge;
    public Vector2Int parry;
    public int distance;//something for position maybe a close/mid/far label

    
    //give info about where the player is facing
}

public struct BossAction
{
    public float meleeAttack;
    public float fireAttack;//undodgeable
    public float block;
    public float charge;//unblockable
}