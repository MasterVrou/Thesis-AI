using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerState
{
    Vector3 lightAttack;
    Vector3 heavyAttack;
    Vector3 offJump;
    Vector3 defJump;
    Vector3 dodge;
    Vector3 parry;
    Vector3 distance;//something for position maybe a close/mid/far label
}

public struct BossAction
{
    float meleeAttack;
    float fireAttack;//undodgeable
    float block;
    float charge;//unblockable
}