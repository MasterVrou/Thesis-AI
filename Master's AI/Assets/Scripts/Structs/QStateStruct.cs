using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1.572.864 States
[System.Serializable]
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

[System.Serializable]
public struct BossAction
{
    public float meleeAttack;
    public float fireAttack;//undodgeable
    public float block;
    public float charge;//unblockable
    public float firepillar;
    public float fireball;
    public float hook;
}
[System.Serializable]
public struct ReadLoad
{
    public int lightAttack1;
    public int lightAttack2;
    public int heavyAttack1;
    public int heavyAttack2;
    public int offJump1;
    public int offJump2;
    public int defJump1;
    public int defJump2;
    public int dodge1;
    public int dodge2;
    public int parry1;
    public int parry2;
    public int distance;

    public float meleeAttack;
    public float fireAttack;//undodgeable
    public float block;
    public float charge;//unblockable
    public float firepillar;
    public float fireball;
    public float hook;
}

[System.Serializable]
public struct ReadLoadList
{
    public List<ReadLoad> rlList;
}