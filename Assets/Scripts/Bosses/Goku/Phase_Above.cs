using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Above : Phase
{
    private enum States
    {
        IDLE,

        NONE,
    }
    private States mainState = States.IDLE;
    private States secondaryState = States.NONE;

    public override void OnHit(int damage)
    {
        if (invincible)
            return;

        boss.currentHealth -= damage;

        if (boss.currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}