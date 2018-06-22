using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase_Death : PhaseNode
{
    public float deathTime;
    protected float currentDeathTime;

    public AudioSource deathAudio;

    public override void OnBegin()
    {
        manager.animator.SetTrigger("DEATH");

        deathAudio.Play();

        manager.boss.SetHealth(0);
    }

    public override void UpdateNode()
    {
        manager.body.velocity = Vector2.zero;
        manager.body.gravityScale = 0.0f;

        currentDeathTime += Time.deltaTime;
        if (currentDeathTime >= deathTime)
        {
            currentDeathTime = 0.0f;

            Destroy(manager.boss.gameObject);
        }
    }
}