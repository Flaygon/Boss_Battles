using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Forest : PhaseNode
{
    public override void UpdateNode()
    {
        
    }

    public override void OnHit(int damage)
    {
        manager.boss.currentHealth -= damage;

        manager.boss.healthManager.UpdateHealth(manager.boss.currentHealth, health, 0);

        if (manager.boss.currentHealth <= 0)
        {
            triggered1 = true;
        }
    }
}