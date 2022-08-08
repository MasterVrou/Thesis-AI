using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishHook : MonoBehaviour
{
    private bool isAOEing = false;
    [SerializeField]
    private Transform AOEHitBoxPos;
    [SerializeField]
    private LayerMask whatIsPlayer;
    private Animator anim;

    private AttackDetails attackDetails;

    public float AOERadius = 0.22f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void FinishAOE()
    {
        //if (isAOEing && AOEStartTime + AOEDuration < Time.time)
        //{
        //    isAOEing = false;
        //}
        isAOEing = false;
        anim.SetBool("isAOEing", isAOEing);
    }

    private void CheckAOEHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(AOEHitBoxPos.position, AOERadius, whatIsPlayer);

        attackDetails.damageAmount = 30;
        attackDetails.position = transform.position;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(AOEHitBoxPos.position, AOERadius);
    }
}
