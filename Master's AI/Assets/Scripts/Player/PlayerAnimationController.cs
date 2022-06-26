using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerController PC;

    private void Start()
    {
        PC = GetComponent<PlayerController>();
    }

    private void WalkingEnabled()
    {
        PC.setCanWalk(true);
    }

    private void WalkingDisabled()
    {
        PC.setCanWalk(false);
    }

    private void FlipEnabled()
    {
        PC.setCanFlip(true);
    }
    private void FlipDisabled()
    {
        PC.setCanFlip(false);
    }
}
