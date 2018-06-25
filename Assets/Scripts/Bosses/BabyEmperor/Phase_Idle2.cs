using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Idle2 : PhaseNode
{
    public float tempWaitUntilWolf;
    private float currentWaitUntilWolf = 0.0f;

    public override void UpdateNode()
    {
        currentWaitUntilWolf += Time.deltaTime;
        if(currentWaitUntilWolf >= tempWaitUntilWolf)
        {
            OnHit(9999);
        }
    }

    public override void OnHit(int damage)
    {
        if (damage >= 9999)
        {
            triggered2 = true;
        }
        else
        {
            manager.boss.currentHealth -= damage;

            manager.boss.healthManager.UpdateHealth(manager.boss.currentHealth, health, 0);

            if (manager.boss.currentHealth <= 0)
            {
                triggered1 = true;
            }
        }
    }
}