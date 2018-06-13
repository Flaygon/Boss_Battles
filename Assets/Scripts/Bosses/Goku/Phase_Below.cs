using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Below : PhaseNode
{
    private enum States
    {
        IDLE,

        DEATH,

        NONE,
    }
    private States mainState = States.IDLE;
    private States secondaryState = States.NONE;

    public override void UpdateNode()
    {

    }

    public override void OnHit(int damage)
    {
        if (invincible)
            return;

        manager.boss.currentHealth -= damage;

        manager.boss.SetHealth(manager.boss.currentHealth);

        if (manager.boss.currentHealth <= 0)
        {
            triggered1 = true;
        }
    }
}