using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerState
{
    public Vector3 lightAttack;
    public Vector3 heavyAttack;
    public Vector3 offJump;
    public Vector3 defJump;
    public Vector3 dodge;
    public Vector3 parry;
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