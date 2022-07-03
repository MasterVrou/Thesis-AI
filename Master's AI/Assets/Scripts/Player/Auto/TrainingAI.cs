using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAI : MonoBehaviour
{
    private PlayerController pController;
    private PlayerCombat pCombat;

    private float random;
    private float nextActionTime;
    private float period;

    private void Start()
    {
        pController = GetComponent<PlayerController>();
        pCombat = GetComponent<PlayerCombat>();

        nextActionTime = 0;
        period = 0.1f;
    }

    private void Update()
    {
        if(Time.time > nextActionTime)
        {
            nextActionTime += period;

            Auto();
        }
    }

    private void Auto()
    {
        if (Random.Range(0, 10) <= 3)
        {
            float r = Random.Range(0, 2);

            if (r == 0)
            {
                pController.Jump();
            }
            else if (r == 1)
            {
                pController.Dodge();
            }
            else
            {
                pCombat.Parry();
            }
        }
        else
        {
            float r = Random.Range(0, 1);

            if (r == 0)
            {
                pCombat.LightAttack();
            }
            else
            {
                pCombat.HeavyAttack();
            }
        }
    }

    private void Deffensive()
    {

    }
}
