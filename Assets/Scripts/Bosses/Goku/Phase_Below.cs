using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Below : PhaseNode
{
    public AudioSource transformEnterAudio;

    private enum States
    {
        IDLE,

        DEATH,

        NONE,
    }
    private States mainState = States.IDLE;
    private States secondaryState = States.NONE;

    public override void OnBegin()
    {
        transformEnterAudio.Play();
    }

    public override void UpdateNode()
    {

    }

    public override void OnHit(int damage)
    {
        if (invincible)
            return;

        manager.boss.currentHealth -= damage;

        manager.boss.healthManager.UpdateHealth(manager.boss.currentHealth, health, 0);

        if (manager.boss.currentHealth <= 0)
        {
            triggered1 = true;
        }
    }
}